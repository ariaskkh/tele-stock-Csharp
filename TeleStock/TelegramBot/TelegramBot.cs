using Common.Interfaces;
using Common.Models;
using Common.Repository;
using Scheduler;
using Telegram.Bot;
using TelegramBot.Services;

namespace TelegramBot
{
    public class TelegramBot
    {
        TelegramBotClient? _botClient;
        TreasuryStockService _treasuryStockService;
        TreasuryStockScheduler _scheduler;
        ILogger _logger;

        public TelegramBot(ILogger logger, ITreasuryStockRepository db)
        {
            _botClient = new TelegramBotClient(PrivateData.ACCESS_TOKEN);
            _treasuryStockService = new TreasuryStockService(logger, db);
            _scheduler = new TreasuryStockScheduler(logger);
            _logger = logger;
        }

        public void Start()
        {
            _scheduler.Start(StartTelegramBot);
        }

        public void Stop()
        {
            _scheduler.Stop();
        }

        private async Task StartTelegramBot()
        {
            await _treasuryStockService.UpdateDataAsync();
            await SendMessages();
        }


        private async Task SendMessages()
        {
            var messageList = _treasuryStockService.GetMessages();
            if (messageList?.Any() ?? false)
            {
                foreach (Message message in messageList)
                {
                    await _botClient.SendTextMessageAsync(PrivateData.CHAT_ID, message.MessageContent);
                }
                _logger.Log("send Messages completed!");
            }
        }
    }
}