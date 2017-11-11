using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions
    {
        public class Node : INotifyPropertyChanged, IBinaryIO
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

            public Page CurrentPage { get; set; }

            protected Node()
            {
                _children = new ObservableCollection<Node>();
                _children.CollectionChanged += (sender, e) =>
                {
                    Count = _children.Count;
                };
            }

            protected virtual void Load(BinaryReader reader)
            {
                _subject = reader.ReadString();
                _count = reader.ReadInt32();
                _newCount = reader.ReadInt32();
            }

            protected virtual void Save(BinaryWriter writer)
            {
                // Mine
                writer.Write(_subject);
                writer.Write(_count);
                writer.Write(_newCount);
            }

            void IBinaryIO.Load(BinaryReader reader)
            {
                // This is mine
                Load(reader);

                // for children
                int childCount = reader.ReadInt32();
                for (int i = 0; i < childCount; ++i)
                {
                    string childTypeName = reader.ReadString();
                    var instance = Subscriptions.CreateNodeFromTypeName(childTypeName);

                    ((IBinaryIO)instance).Load(reader);
                    _children.Add(instance);
                }
            }

            void IBinaryIO.Save(BinaryWriter writer)
            {
                Save(writer);

                // children
                writer.Write(Children.Count);
                foreach (var child in Children)
                {
                    writer.Write(child.GetType().FullName);
                    ((IBinaryIO)child).Save(writer);
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            public virtual void Navigate(Frame frameUI)
            {
                if (CurrentPage != null)
                    frameUI.Navigate(CurrentPage);
            }
        }

        // '홈'을 위한 노드
        public class HomeNode : Node
        {
            private HomeNode()
            {
                Subject = "홈";
                CurrentPage = new Contetns.HomePage();
            }
        }
    }
}
