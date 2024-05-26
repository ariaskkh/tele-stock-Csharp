using Common.Interfaces;
using Telegram.Bot;
using TelegramBot.Services;

namespace TelegramBot
{
    public class TelegramBot
    {
        TelegramBotClient? _botClient;
        TreasuryStockService _treasuryStockService;
        public TelegramBot(ILogger logger)
        {
            _botClient = new TelegramBotClient(PrivateData.ACCESS_TOKEN);
            _treasuryStockService = new TreasuryStockService(logger);
        }

        public async Task Start()
        {
            await _treasuryStockService.UpdateDataAsync();
            //SendMessage();
        }

        private async Task SendMessage()
        {
            var messages = _treasuryStockService.GetMessages();
            foreach (var message in messages)
            {
                await _botClient.SendTextMessageAsync(PrivateData.CHAT_ID, message);
            }
        }
    }
}