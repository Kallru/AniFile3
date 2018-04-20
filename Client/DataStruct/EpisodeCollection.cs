using CoreLib.DataStruct;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AniFile3.DataStruct
{
    public class EpisodeCollection : ObservableCollection<ClientEpisodeInfo>
    {
        public bool Contains(EpisodeInfo info)
        {
            foreach (var item in Items)
            {
                if (item.Subject == info.Fullname
                    && item.Episode == info.Episode
                    && item.Resolution == info.Resolution)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddRange(IEnumerable<EpisodeInfo> infos)
        {
            foreach (var episode in infos)
            {
                // 중복 체크해서 클라에 데이터 넣기.
                if (Contains(episode) == false)
                {
                    Items.Add(new ClientEpisodeInfo(episode));
                }
            }

            Polish();
        }

        // 현재 정책에 따라 에피소드 리스트들을 정리한다.
        // 1. Best 화질만 선택한다.
        public void Polish()
        {

        }
    }
}
