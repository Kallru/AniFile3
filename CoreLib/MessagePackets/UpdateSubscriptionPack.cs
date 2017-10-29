using CoreLib.DataStruct;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.MessagePackets
{
    [MessagePackObject]
    public struct UpdateSubscriptionRequest
    {
        [MessagePackObject]
        public struct Request
        {
            [Key(0)]
            public string SubscriptionName { get; set; }
            [Key(1)]
            public int LatestEpisode { get; set; }
        }

        [Key(0)]
        public List<Request> Subscriptions { get; set; }
    }

    [MessagePackObject]
    public class UpdateSubscriptionResponse
    {
        [MessagePackObject]
        public struct Response
        {
            [Key(0)]
            public string Name { get; set; }

            [Key(1)]
            public List<EpisodeInfo> EpisodeInfos;
        }

        [Key(0)]
        public List<Response> Items { get; set; }
    }
}
