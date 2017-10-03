using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.MessagePackets
{
    [MessagePackObject]
    public struct EpisodeInfo
    {
        [Key(0)]
        public readonly string Subject;
        [Key(1)]
        public readonly string Resolution;
        [Key(2)]
        public readonly int Episode;
        [Key(3)]
        public readonly string Magnet;

        public EpisodeInfo(string subject, string resolution, int episode, string magnet)
        {
            Subject = subject;
            Resolution = resolution;
            Episode = episode;
            Magnet = magnet;
        }
    }
}
