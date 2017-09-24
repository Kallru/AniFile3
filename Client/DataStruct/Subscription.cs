using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniFile3.DataStruct
{
    public class Subscriptions : ObservableCollection<Subscriptions.Node>
    {
        public class Node : INotifyPropertyChanged
        {
            private ObservableCollection<Node> _children = new ObservableCollection<Node>();

            private string _subject;
            private int _count;

            public string Subject
            {
                get => _subject;
                set { _subject = value; NotifyPropertyChanged("Subject"); }
            }

            public int Count
            {
                get => _count;
                set { _count = value; NotifyPropertyChanged("Subject"); }
            }

            public ObservableCollection<Node> Children
            {
                get => _children;
                set { _children = value; NotifyPropertyChanged("Children"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
