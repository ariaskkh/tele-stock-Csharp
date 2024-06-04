using Common.Database;
using Common.Interfaces;
using Common.Models;
using TelegramBot;

namespace TelegramBotClient
{
    public partial class Form1 : Form
    {
        ILogger? _logger;
        TelegramBot.TelegramBot? _telegramBot;
        public Form1()
        {
            InitializeComponent();
            InittializeTelegramBot();
        }

        void InittializeTelegramBot()
        {
            _logger = new Logger(textBox1);
            var db = new TreasuryStockDocument();
            _telegramBot = new TelegramBot.TelegramBot(_logger, db);
        }

        void startButton_Click(object sender, EventArgs e)
        {
            if (_telegramBot != null)
            {
                _telegramBot.Start();
            }
        }
    }
}
