using Telegram.Bot;
using TelegramBot;

namespace Practice
{
    public partial class Form1 : Form
    {
        private TelegramBot.TelegramBot _telegramBot;
        public Form1()
        {
            InitializeComponent();
            //_telegramBot = new TelegramBot.TelegramBot();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Start_Click(object sender, EventArgs e)
        {
            //textBox1.Text = "Button was clicked!";
            Start_Bot();
        }

        private async Task Start_Bot()
        {
            //var me = await _telegramBot.GetMeAsync();
            //await _telegramBot.SendMessage("가라가라 메세지 !!!");

            //textBox1.Text = ($"Hello, World! I am user {me.Id} and my name is {me.FirstName}");
        }
    }
}
