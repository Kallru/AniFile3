using CoreLib.MessagePackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniFile3.DataStruct
{
    public class EpisodeInfoClient
    {
        private EpisodeInfo _header;

        public string Subject { get => _header.Subject; }
        public int Episode { get => _header.Episode; }
        public string Resolution { get => _header.Resolution; }
        public string Location { get; private set; }
        public float DownloadState { get; private set; }

        public EpisodeInfoClient(EpisodeInfo header)
        {
            _header = header;
        }
    }
}
