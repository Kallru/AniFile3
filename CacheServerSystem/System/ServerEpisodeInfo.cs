using CoreLib.DataStruct;
using MongoDB.Bson;

namespace CacheServerSystem
{
    public class ServerEpisodeInfo
    {
        public ObjectId Id { get; set; }
        public ObjectId NameId { get; set; }
        public EpisodeInfo Info { get; set; }
    }
}
