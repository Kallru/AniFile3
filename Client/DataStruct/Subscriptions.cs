using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions : ObservableCollection<Subscriptions.Node>
    {
        public class Node : INotifyPropertyChanged
        {
            private ObservableCollection<Node> _children;
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
                set { _count = value; NotifyPropertyChanged("Count"); }
            }

            public ObservableCollection<Node> Children
            {
                get => _children;
                set { _children = value; NotifyPropertyChanged("Children"); }
            }

            public Page CurrentPage { get; set; }

            public Node()
            {
                _children = new ObservableCollection<Node>();
                _children.CollectionChanged += (sender, e) =>
                {
                    Count = _children.Count;
                };
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
