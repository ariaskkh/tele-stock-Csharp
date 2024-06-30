using Common.Interfaces;
using System.Timers;

namespace Scheduler
{
    public class TreasuryStockScheduler
    {
        private ILogger _logger;
        private static System.Timers.Timer? timer;
        private Func<Task> _action;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public TreasuryStockScheduler(ILogger logger)
        {
            _logger = logger;
        }
        
        public void Start(Func<Task> action)
        {
            _action = action;
            StartScheduler();
        }

        public void Stop()
        {
            if (timer != null)
            {
                _logger.Log("멈춤");
                timer.Stop();
                timer.Dispose();
                cancellationTokenSource.Cancel();
            }
        }

        private void StartScheduler()
        {
            _logger.Log("시작");
            timer = new System.Timers.Timer(2000);
            timer.Elapsed += async (sender, e) => await OnTimedEvent(sender, e);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        // 얘 메인 스레드인지 백그라운드 스레드인지 확인해야함. 아마 백그라운드 스레드 같다.
        private async Task OnTimedEvent(Object? source, ElapsedEventArgs e)
        {
            if (IsWorkingHour())
            {
                await semaphore.WaitAsync();
                {
                    try
                    {
                        if (_action != null)
                        {
                            _logger.Log("데이터 받아오기");
                            await _action();
                        }
                    }
                    catch
                    {
                        _logger.Log("데이터 받아오기 실패");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
                
            }
            else
            {
                if (timer != null)
                {
                    timer.Stop();
                    await SleepUntilWorkingHour();
                    timer.Start();
                }
            }
        }

        private bool IsWorkingHour()
        {
            TimeZoneInfo koreanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");

            DateTime koreanTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, koreanTimeZone);

            TimeSpan workStart = new TimeSpan(9, 0, 0); // 시간
            TimeSpan workEnd = new TimeSpan(15, 30, 0);

            return koreanTimeNow.TimeOfDay >= workStart && koreanTimeNow.TimeOfDay <= workEnd;
        }

        private async Task SleepUntilWorkingHour()
        {
            TimeZoneInfo koreanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");

            DateTime koreanTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, koreanTimeZone);
            DateTime nextStart = koreanTimeNow.Date.AddHours(9); // 시간

            if (koreanTimeNow.TimeOfDay >= new TimeSpan(15, 30, 0))
            {
                nextStart = nextStart.AddDays(1);
            }

            TimeSpan sleepTime = nextStart - DateTime.Now;

            if (sleepTime >= TimeSpan.Zero)
            {
                _logger.Log($"Sleeping for {sleepTime.TotalMinutes} minutes ");
                try
                {
                    await Task.Delay(sleepTime, cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    _logger.Log("Sleep was canceled");
                }
            }
        }
    }
}
