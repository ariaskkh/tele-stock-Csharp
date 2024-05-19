
using Common.Interfaces;

namespace TelegramBotClient
{
    public class Logger : ILogger
    {
        private TextBox _textBox;
        public Logger(TextBox textbox)
        {
            _textBox = textbox;
        }

        public void Log(string message)
        {
            _textBox.AppendText(message + Environment.NewLine);
        }
    }
}
