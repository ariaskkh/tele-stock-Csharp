
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
            if (_textBox.InvokeRequired)
            {
                _textBox.Invoke(new Action(() => Log(message)));
            }
            else
            {
                _textBox.AppendText(message + Environment.NewLine);
            }
        }
    }
}
