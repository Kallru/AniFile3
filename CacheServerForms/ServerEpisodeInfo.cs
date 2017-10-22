using CoreLib.DataStruct;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheServerForms
{
    public class ServerEpisodeInfo
    {
        public ObjectId Id { get; set; }
        public ObjectId NameId { get; set; }
        public EpisodeInfo Info { get; set; }
    }
}
