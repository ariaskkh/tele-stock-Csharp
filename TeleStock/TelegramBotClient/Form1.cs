using TelegramBot;

namespace TelegramBotClient
{
    public partial class Form1 : Form
    {
        TelegramBot.TelegramBot _telegramBot;
        public Form1()
        {
            InitializeComponent();
            InittializeTelegramBot();
        }

        void InittializeTelegramBot()
        {
            _telegramBot = new TelegramBot.TelegramBot();
        }

        void startButton_Click(object sender, EventArgs e)
        {
            SendTelegramMessage();
        }

        async Task SendTelegramMessage()
        {
            await _telegramBot.SendMessage();
        }
    }
}
