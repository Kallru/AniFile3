using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AniFile3
{
    public class Logger : TextWriter
    {
        private TextBox _textbox;

        public override Encoding Encoding => Encoding.Default;

        public Logger(dynamic textbox)
        {
            this._textbox = textbox;
            _textbox.Clear();
        }

        private void ScrollDown()
        {
            _textbox.CaretIndex = _textbox.Text.Length;
            _textbox.ScrollToEnd();
        }

        private void Dispacther(Action doing)
        {
            _textbox.Dispatcher.Invoke(doing);
        }

        public override void Write(char value)
        {
            Dispacther(() =>
            {
                _textbox.Text += value;
                ScrollDown();
            });
        }
        public override void Write(string value)
        {
            Dispacther(() =>
            {
                _textbox.Text += value;
                ScrollDown();
            });
        }
        public override void Flush() => Dispacther(() => _textbox.Clear());
        public override void Close() => _textbox = null;
    }
}
