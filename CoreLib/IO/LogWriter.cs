using System.IO;
using System.Text;

namespace RichGrassHopper.Core.IO
{
    public class LogWriter : TextWriter
    {
        private dynamic _textbox;

        public override Encoding Encoding => Encoding.Default;

        public LogWriter(dynamic textbox)
        {
            this._textbox = textbox;
            _textbox.Clear();
        }

        public override void Write(char value) => _textbox.Text += value;
        public override void Write(string value) => _textbox.Text += value;
        public override void Flush() => _textbox.Clear();
        public override void Close() => _textbox = null;
    }
}
