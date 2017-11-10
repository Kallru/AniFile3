using AniFile3.Contetns;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions
    {
        public class ContentNode : Node
        {
            private ObservableCollection<ClientEpisodeInfo> _episodes;

            public ObservableCollection<ClientEpisodeInfo> Episodes { get => _episodes; }

            public ContentNode()
            {
                _episodes = new ObservableCollection<ClientEpisodeInfo>();
            }

            protected override void Load(BinaryReader reader)
            {
                base.Load(reader);

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    int size = reader.ReadInt32();
                    var bytes = reader.ReadBytes(size);
                    var episode = MessagePackSerializer.Deserialize<ClientEpisodeInfo>(bytes);
                    _episodes.Add(episode);
                }
            }

            protected override void Save(BinaryWriter writer)
            {
                base.Save(writer);

                writer.Write(_episodes.Count);
                foreach (var episode in _episodes)
                {
                    var bytes = MessagePackSerializer.Serialize(episode);
                    writer.Write(bytes.Length);
                    writer.Write(bytes);
                }
            }

            public override void InitializePage(Page page)
            {
                base.InitializePage(page);
                
                _episodes.CollectionChanged += (sender, e) =>
                {
                    Count = _episodes.Count;
                    NewCount = _episodes.Count;
                };
            }

            public override void Navigate(Frame frameUI)
            {
                var page = CurrentPage as EpisodePage;
                page.LoadEpisode(_episodes);

                base.Navigate(frameUI);
            }
        }
    }
}
