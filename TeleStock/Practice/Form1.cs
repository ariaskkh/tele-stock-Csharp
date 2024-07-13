using Common.Interfaces;
using Logger = TelegramBotClient.Logger;

namespace Practice
{
    public partial class Form1 : Form
    {
        private TelegramBot.TelegramBot _telegramBot;
        private ILogger _logger;
        public Form1()
        {
            InitializeComponent();
            //_telegramBot = new TelegramBot.TelegramBot();
            _logger = new Logger(textBox1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Start_Click(object sender, EventArgs e)
        {
            //textBox1.Text = "Button was clicked!";
            //Start_Bot();
            _logger.Log("시작");
            Task.Run(Start_DB);
        }

        private async Task Start_Bot()
        {
            //var me = await _telegramBot.GetMeAsync();
            //await _telegramBot.SendMessage("가라가라 메세지 !!!");

            //textBox1.Text = ($"Hello, World! I am user {me.Id} and my name is {me.FirstName}");
        }

        private async Task Start_DB()
        {
            var service = new ProductService(_logger);
            await service.CallFunc();
            var product = new Product
            {
                Id = 1,
                Name = "Test",
            };
            _logger.Log("item is created");
            await service.SaveItem(product);
        }

    }
}
