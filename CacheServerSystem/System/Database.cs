using MongoDB.Driver;

namespace CacheServerSystem
{
    public class DataBase
    {
        private MongoClient _mongoClient;
        private IMongoDatabase _db;

        static private DataBase _instance;
        static public DataBase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataBase();
                return _instance;
            }
        }

        public IMongoDatabase Main { get => _db; }
        public IMongoCollection<ServerEpisodeInfo> Collection { get => _db.GetCollection<ServerEpisodeInfo>("content"); }
        public IMongoCollection<ServerEpisodeInfo> Programs { get => _db.GetCollection<ServerEpisodeInfo>("programs"); }

        private DataBase()
        {
            _mongoClient = new MongoClient();
            _db = _mongoClient.GetDatabase("MainContents");
        }
    }
}
