using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AniFile3.UIControls
{
    public class SearchEventArgs : RoutedEventArgs
    {
        public string Text { get; private set; }

        public SearchEventArgs(RoutedEvent routedEvent, string text)
            : base(routedEvent)
        {
            Text = text;
        }
    }
}
