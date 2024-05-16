using Telegram.Bot;

namespace TelegramBot
{
    public class TelegramBot
    {
        private TelegramBotClient? _botClient;
        public TelegramBot()
        {
            _botClient = new TelegramBotClient(PrivateData.ACCESS_TOKEN);
        }

        public async Task<Telegram.Bot.Types.User> GetMeAsync()
        {
            return await _botClient.GetMeAsync();
            
        }
    }
}
