using Common.Interfaces;
using Common.Models;
using Telegram.Bot;
using TelegramBot.Services;

namespace TelegramBot
{
    public class TelegramBot
    {
        TelegramBotClient? _botClient;
        TreasuryStockService _treasuryStockService;
        ILogger _logger;
        public TelegramBot(ILogger logger)
        {
            _botClient = new TelegramBotClient(PrivateData.ACCESS_TOKEN);
            _treasuryStockService = new TreasuryStockService(logger);
            _logger = logger;
        }

        public async Task Start()
        {
            await _treasuryStockService.UpdateDataAsync();
            await SendMessages();
        }

        private async Task SendMessages()
        {
            var messageList = _treasuryStockService.GetMessages();
            foreach (Message message in messageList)
            {
                await _botClient.SendTextMessageAsync(PrivateData.CHAT_ID, message.MessageContent);
            }
            _logger.Log("send Messages completed!");
        }
    }
}