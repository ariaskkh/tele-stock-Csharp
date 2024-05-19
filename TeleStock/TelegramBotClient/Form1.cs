using Common.Interfaces;
using TelegramBot;

namespace TelegramBotClient
{
    public partial class Form1 : Form
    {
        ILogger _logger;
        TelegramBot.TelegramBot _telegramBot;
        public Form1()
        {
            InitializeComponent();
            InittializeTelegramBot();
        }

        void InittializeTelegramBot()
        {
            _logger = new Logger(textBox1);
            _telegramBot = new TelegramBot.TelegramBot(_logger);
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
