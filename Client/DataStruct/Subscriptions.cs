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
            private int _newCount;

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

            public int NewCount
            {
                get => _newCount;
                set { _newCount = value; NotifyPropertyChanged("NewCount"); }
            }

            public ObservableCollection<Node> Children
            {
                get => _children;
                set { _children = value; NotifyPropertyChanged("Children"); }
            }

            public Page CurrentPage { get; private set; }

            public Node(Page page)
            {
                CurrentPage = page;

                _children = new ObservableCollection<Node>();
                _children.CollectionChanged += (sender, e) =>
                {
                    Count = _children.Count;
                };

                // TestCode
                NewCount = 12;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            public virtual void Navigate(Frame frameUI)
            {
                frameUI.Navigate(CurrentPage);
            }
        }

        // '홈'을 위한 노드
        public class HomeNode : Node
        {
            public HomeNode(Page page)
                :base(page)
            { }
        }
    }
}
