using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Scriping
{
    public partial class DataStorage
    {
        public class Contents
        {
            public ObjectId Id { get; set; }

            public ObjectId NameId { get; set; }
            public string Fullname { get; set; }
            public string Name { get; set; }
            public string Resolution { get; private set; }
            public int Episode { get; private set; }
            public string Magnet { get; set; }

            // 각종 정보를 subject에서 가져와서 생성한다.
            static public Contents Create(string subject, string magnet)
            {
                var instance = new Contents()
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
}
