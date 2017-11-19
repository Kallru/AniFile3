using AniFile3.Contetns;
using MessagePack;
using MessagePack.Resolvers;
using System.IO;
using System.Windows.Controls;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions
    {
        public class ContentNode : Node
        {
            private EpisodeCollection _episodes;

            public EpisodeCollection Episodes { get => _episodes; }

            private ContentNode()
            {
                _episodes = new EpisodeCollection();
                _episodes.CollectionChanged += (sender, e) =>
                {
                    Count = _episodes.Count;
                    NewCount = _episodes.Count;

                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    {
                        var myCollection = sender as EpisodeCollection;
                        myCollection[e.NewStartingIndex].Start();
                    }
                };
            }

            protected override void Load(BinaryReader reader)
            {
                base.Load(reader);

                _episodes.Clear();
                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    int size = reader.ReadInt32();
                    var bytes = reader.ReadBytes(size);
                    var episode = MessagePackSerializer.Deserialize<ClientEpisodeInfo>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
                    _episodes.Add(episode);
                }
            }

            protected override void Save(BinaryWriter writer)
            {
                base.Save(writer);

                writer.Write(_episodes.Count);
                foreach (var episode in _episodes)
                {
                    var bytes = MessagePackSerializer.Serialize(episode, ContractlessStandardResolverAllowPrivate.Instance);
                    writer.Write(bytes.Length);
                    writer.Write(bytes);
                }
            }

            public override void Navigate(Frame frameUI)
            {
                var page = CurrentPage as EpisodePage;
                page.LoadEpisode(_episodes);

                base.Navigate(frameUI);
            }

            public void Start()
            {
                // 여기서 어떤걸 시작 시킬지 처리?
                foreach (var item in _episodes)
                {
                    item.Start();
                }
            }
        }
    }
}
