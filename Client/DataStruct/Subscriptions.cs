using MessagePack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions : ObservableCollection<Subscriptions.Node>
    {
        private interface IInput
        {
            void Load(BinaryReader reader, Node node);
            void Save(BinaryWriter writer, Node node);
        }

        public void LoadFrom(string fileName = "datastorage.bin")
        {
            using (var reader = new BinaryReader(new FileStream(fileName, FileMode.Open)))
            {
                foreach (IInput item in Items)
                {
                    item.Load(reader, item as Node);
                }
            }
        }

        public void SaveFrom(string fileName = "datastorage.bin")
        {
            using (var writer = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
            {
                InternalSave(writer, this);
            }
        }

        public class Node : INotifyPropertyChanged, IInput
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

            public Node()
            {
                _children = new ObservableCollection<Node>();
            }

            public virtual void InitializePage(Page page)
            {
                CurrentPage = page;
                
                _children.CollectionChanged += (sender, e) =>
                {
                    Count = _children.Count;
                };
            }

            protected virtual void Load2(BinaryReader reader)
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

            // Interface design
            // ref. https://stackoverflow.com/questions/203616/why-does-c-sharp-not-provide-the-c-style-friend-keyword
            void IInput.Load(BinaryReader reader, Node node)
            {
                // This is mine
                Load2(reader);

                // for children
                int childCount = reader.ReadInt32();
                for (int i = 0; i < childCount; ++i)
                {
                    string childTypeName = reader.ReadString();
                    Type childType = Type.GetType(childTypeName);
                    var instance = Activator.CreateInstance(childType) as Node;

                    this.l

                    InternalLoad(reader, instance);
                    _children.Add(instance);
                }
            }

            void IInput.Save(BinaryWriter writer, Node node)
            {
                Save(writer);

                // children
                writer.Write(node.Children.Count);
                foreach (var child in node.Children)
                {
                    writer.Write(child.GetType().Name);

                    InternalSave(writer, child);
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
                frameUI.Navigate(CurrentPage);
            }
        }

        // '홈'을 위한 노드
        public class HomeNode : Node
        {
            public HomeNode()
            {
                Subject = "홈";
                InitializePage(new Contetns.HomePage());
            }
        }
    }
}
