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

    // 일단 여기다 넣고, 나중에 현재 이 파일(ServerEpisodeInfo.cs)를 바꿀지
    // 또는 추가되는 정보마다 cs 파일을 만들지 고민 해보자.(한개에 모든 정보 클래스를 넣을지 or 다 쪼갤지)
    public class RssListInfo
    {
        public ObjectId Id { get; set; }
        public string RssAddress { get; set; }             
    }    
}
