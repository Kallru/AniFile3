using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.MessagePackets
{
    [MessagePackObject]
    public struct SearchResult
    {
        [Key(0)]
        public int a;
        //public List<EpisodeInfo> Infos;
    }
}
