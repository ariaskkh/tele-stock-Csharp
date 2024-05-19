using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services;
using System.Windows;

namespace TelegramBot
{
    public class TelegramBot
    {
        TelegramBotClient? _botClient;
        TreasuryStockService _treasuryStockService;
        public TelegramBot()
        {
            _botClient = new TelegramBotClient(PrivateData.ACCESS_TOKEN);
            _treasuryStockService = new TreasuryStockService();
        }

        public async Task<Telegram.Bot.Types.User> GetMeAsync()
        {
            return await _botClient.GetMeAsync();
        }

        public async Task SendMessage()
        {
            var messages = _treasuryStockService.GetMessages();
            foreach (var message in messages)
            {
                await _botClient.SendTextMessageAsync(PrivateData.CHAT_ID, message);
            }
        }
    }
}