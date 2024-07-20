using Amazon.DynamoDBv2.Model;
using Common.Interfaces;
using Logger = TelegramBotClient.Logger;

namespace Practice
{
    public partial class Form1 : Form
    {
        private TelegramBot.TelegramBot _telegramBot;
        private ProductService _service;
        private ILogger _logger;
        public Form1()
        {
            InitializeComponent();
            //_telegramBot = new TelegramBot.TelegramBot();
            _logger = new Logger(textBox1);
            _logger.Log("시작");
            Start_DB();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void Start_Click(object sender, EventArgs e)
        {
            await CreateItem();
        }

        private async Task Start_Bot()
        {
            //var me = await _telegramBot.GetMeAsync();
            //await _telegramBot.SendMessage("가라가라 메세지 !!!");

            //textBox1.Text = ($"Hello, World! I am user {me.Id} and my name is {me.FirstName}");
        }

        private void Start_DB()
        {
            _service = new ProductService(_logger);
        }

        private async Task CreateItem()
        {
            //var product = new Product
            //{
            //    Id = 1,
            //    Name = "Test",
            //};

            var product = new PutItemRequest
            {
                TableName = "ProductTable",
                Item = new Dictionary<string, AttributeValue>()
                {
                    { "Id", new AttributeValue {
                          N = "1000"
                      }},
                    { "Name", new AttributeValue {
                          S = "김강호"
                      }},
                    { "Title", new AttributeValue {
                          S = "Book 201 Title"
                      }},
                    { "ISBN", new AttributeValue {
                          S = "11-11-11-11"
                      }},
                    { "Authors", new AttributeValue {
                          SS = new List<string>{"Author1", "Author2" }
                      }},
                    { "Price", new AttributeValue {
                          N = "20.00"
                      }},
                    { "Dimensions", new AttributeValue {
                          S = "8.5x11.0x.75"
                      }},
                    { "InPublication", new AttributeValue {
                          BOOL = false
                      }},
                }

            };

            await _service.PutItem(product);
        }

        private async void GetItem(object sender, EventArgs e)
        {
            await _service.GetItem();
        }
    }
}
