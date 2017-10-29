using AniFile3.Contetns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions
    {
        public class ContentNode : Node
        {
            private ObservableCollection<ClientEpisodeInfo> _episodes;

            public ObservableCollection<ClientEpisodeInfo> Episodes { get => _episodes; }

            public ContentNode(Page page)
                :base(page)
            {
                _episodes = new ObservableCollection<ClientEpisodeInfo>();
                _episodes.CollectionChanged += (sender, e) =>
                {
                    Count = _episodes.Count;
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
