using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreLib.DataStruct
{
    [MessagePackObject]
    public class EpisodeInfo
    {
        [Key(0)]
        public string Fullname { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string Resolution { get; set; }
        [Key(3)]
        public int Episode { get; set; }
        [Key(4)]
        public string Magnet { get; set; }

        public static int Compare(EpisodeInfo left, EpisodeInfo right)
        {
            var result = left.Episode - right.Episode;
            if (result == 0)
            {
                Func<string, int> getResolution = (str) =>
                {
                    int value = -1;
                    int.TryParse(str.Replace('p', ' '), out value);
                    return value;
                };

                return getResolution(left.Resolution) - getResolution(right.Resolution);
            }
            return result;
        }

        // 각종 정보를 subject에서 가져와서 생성한다.
        public static EpisodeInfo Create(string subject, string magnet)
        {
            var instance = new EpisodeInfo()
            {
                Magnet = magnet
            };

            string[] resolutions =
            {
                    "360p",
                    "720p",
                    "1080p"
                };

            // 이름파싱
            instance.Resolution = "None";
            foreach (var item in resolutions)
            {
                if (subject.Contains(item))
                {
                    instance.Resolution = item;
                    //subject.Replace(item, "");
                    break;
                }
            }

            // Episode, E로 시작하며 숫자 1~3개까지
            Regex regex = new Regex(@"E\d{1,3}");
            var results = regex.Matches(subject);
            if (results.Count >= 1)
            {
                //instance.Subject = subject.Replace(results[0].Value, "");
                instance.Fullname = subject;
                instance.Episode = int.Parse(results[0].Value.Substring(1));

                // Name에는 오직 이름 자체만 넣어야 한다.
                // 지금은 덜 파싱된 제목을 넣지만, 순수하게 제목만 있어야함.
                instance.Name = subject.Replace(results[0].Value, "");
            }
            else
            {
                // Error
                //Console.WriteLine("[Error] Episode 정규식 매칭 에러!, Episode 추출 실패");
            }
            return instance;
        }
    }
}
