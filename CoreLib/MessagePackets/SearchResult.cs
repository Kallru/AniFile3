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
        public string Subject;
        [Key(1)]
        public string Magnets;
    }
}
