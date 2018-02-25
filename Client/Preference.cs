using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniFile3
{
    [MessagePackObject]
    public class Preference
    {
        public static Preference Instance { get; private set; } = new Preference();

        public const string FileName = "preference.bin";
        public const string SubscriptionsFileName = "datastorage.bin";

        [Key(0)]
        public int DefaultTimeOut { get; set; } = 5000 * 10;
        // unit 'ms'
        [Key(1)]
        public int UpdateSubscriptionInterval { get; set; } = 30 * 1000; // 기본값을 몇시간 정도로 해야될듯
        [Key(2)]
        public string CacheServerUri { get; set; } = "http://localhost:2323";
        //public string CacheServerUri { get; set; } = "http://35.231.110.128:2323";
        [Key(3)]
        public string RootDownloadPath { get; set; } = "data";
        [Key(4)]
        public int CurrentlyTorrentCount { get; set; } = 5;
        [Key(5)]
        public List<string> RSSList { get; set; } = new List<string>();

        // MessagePack 버그인지 모르겠지만, 'StandardResolverAllowPrivate' 이 옵션이 제대로 동작하지 않는것 같다.
        // 일단 임시로 생성자를 public 으로 사용함
        public Preference()
        {
            // Test Code- 테스트용으로 RSS 리스트를 넣어봄
            RSSList.Add("https://torrentkim10.net/bbs/rss.php?k=720p&b=torrent_variety");
        }

        public static string GetAbsoluteDownloadPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Instance.RootDownloadPath);
        }

        public static void Load()
        {
            try
            {
                using (Stream file = new FileStream(FileName, FileMode.Open))
                {
                    Instance = MessagePackSerializer.Deserialize<Preference>(file, StandardResolverAllowPrivate.Instance);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Save()
        {
            using (Stream file = new FileStream(FileName, FileMode.Create))
            {
                var bytes = MessagePackSerializer.Serialize<Preference>(Instance, StandardResolverAllowPrivate.Instance);
                file.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
