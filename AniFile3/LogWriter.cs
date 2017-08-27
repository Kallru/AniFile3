using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AniFile3
{
    public class LogWriter : TextWriter
    {
        private TextBox _textbox;

        public override Encoding Encoding => Encoding.Default;
        
        public LogWriter(TextBox textbox)
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
