using CoreLib.MessagePackets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions
    {
        public class EpisodeNode : Node
        {
            private ObservableCollection<EpisodeInfo> _episodes;

            public ObservableCollection<EpisodeInfo> Episodes { get => _episodes; }

            public EpisodeNode()
            {
                _episodes = new ObservableCollection<EpisodeInfo>();

                _episodes.CollectionChanged += (sender, e) =>
                {
                    Count = _episodes.Count;
                };
            }
        }
    }
}
