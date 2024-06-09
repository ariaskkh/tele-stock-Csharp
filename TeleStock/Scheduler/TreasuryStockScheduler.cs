using Common.Interfaces;
using System.Timers;

namespace Scheduler
{
    public class TreasuryStockScheduler
    {
        private ILogger _logger;
        private static System.Timers.Timer timer;
        private Func<Task> _action;
        private bool isRunning = false;

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
            _logger.Log("멈춤");
            timer.Stop();
            timer.Dispose();
        }

        private void StartScheduler()
        {
            _logger.Log("시작");
            timer = new System.Timers.Timer(2000);
            timer.Elapsed += async (sender, e) => await OnTimedEvent(sender, e);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private async Task OnTimedEvent(Object? source, ElapsedEventArgs e)
        {
            if (isRunning)
            {
                return;
            };

            try
            {
                _logger.Log("데이터 받아오기");
                if (_action != null)
                {
                    await _action();
                }
            }
            finally
            {
                isRunning = false;
            }
        }
    }
}
