using AniFile3.Contents;
using MessagePack;
using MessagePack.Resolvers;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions
    {
        public class ContentNode : Node
        {
            private EpisodeCollection _episodes;

            public EpisodeCollection Episodes { get => _episodes; }
            public int LatestEpisode { get; set; }

            private ContentNode()
            {
                _episodes = new EpisodeCollection();
                _episodes.CollectionChanged += (sender, e) =>
                {
                    Count = _episodes.Count;
                    NewCount = _episodes.Count;

                    // 가장 최신 에피소드화수는 가장 큰 값으로 셋팅함
                    int latestInCollection = (_episodes.Count > 0) ? _episodes.Max(info => info.Episode) : 0;
                    LatestEpisode = System.Math.Max(LatestEpisode, latestInCollection);

                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    {
                        var myCollection = sender as EpisodeCollection;
                        myCollection[e.NewStartingIndex].Start(Subject);
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

            public override void Navigate(TabControl control)
            {
                var page = control.Items.GetItemAt(CurrentTabItem) as EpisodeTabItem;
                page.LoadEpisode(_episodes);

                base.Navigate(control);
            }

            public void Start()
            {
                // 여기서 어떤걸 시작 시킬지 처리?
                foreach (var item in _episodes)
                {
                    item.Start(Subject);
                }
            }
        }
    }
}
