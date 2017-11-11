using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions : ObservableCollection<Subscriptions.Node>
    {
        private IEnumerable<Node> Flatten(IEnumerable<Node> e)
        {
            return e.SelectMany(c => Flatten(c.Children.AsEnumerable())).Concat(e);
        }
        public IEnumerable<T> Flatten<T>() where T : Node
        {
            return Flatten(this).OfType<T>();
        }

        private interface IBinaryIO
        {
            void Load(BinaryReader reader);
            void Save(BinaryWriter writer);
        }

        public bool LoadFromBin()
        {
            var fileName = Preference.SubscriptionsFileName;
            if (File.Exists(fileName) == false)
                return false;

            using (var reader = new BinaryReader(new FileStream(fileName, FileMode.Open)))
            {
                Items.Clear();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    string typeName = reader.ReadString();

                    IBinaryIO item = CreateNodeFromTypeName(typeName) as IBinaryIO;
                    item.Load(reader);
                    Items.Add(item as Node);
                }
            }
            return true;
        }

        public void SaveToBin()
        {
            using (var writer = new BinaryWriter(new FileStream(Preference.SubscriptionsFileName, 
                                                                FileMode.Create)))
            {
                writer.Write(Items.Count);
                foreach (IBinaryIO item in Items)
                {
                    var node = item as Node;
                    writer.Write(node.GetType().FullName);
                    item.Save(writer);
                }
            }
        }
    }
}
