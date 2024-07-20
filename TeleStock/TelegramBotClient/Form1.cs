using Common.Interfaces;
using Common.Repository;

namespace TelegramBotClient
{
    public partial class Form1 : Form
    {
        private ILogger? _logger;
        private TelegramBot.TelegramBot? _telegramBot;

        public Form1()
        {
            InitializeComponent();
            InittializeTelegramBot();
        }

        void InittializeTelegramBot()
        {
            _logger = new Logger(textBox1);
            // DI 받아 종속성 반전 처리 가능...? 
            //var db = new TreasuryStockRepository(_logger); // local에 JSON파일로 저장
            var db = new TreasuryStockRepositoryDDB(_logger);
            _telegramBot = new TelegramBot.TelegramBot(_logger, db);

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (_telegramBot != null)
            {
                _telegramBot.Start();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (_telegramBot != null)
            {
                _telegramBot.Stop();
            }
        }
    }
}
