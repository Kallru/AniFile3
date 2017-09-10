using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scriping
{
    public partial class DataStorage
    {
        public class Contents
        {
            private string _subject;
            private readonly string[] _resolutions =
            {
                "360p",
                "720p",
                "1080p"
            };

            public string Subject
            {
                get { return _subject; }
                set
                {
                    _subject = value;

                    // 이름파싱
                    Resolution = "None";
                    foreach (var item in _resolutions)
                    {
                        if (_subject.Contains(item))
                        {
                            Resolution = item;
                            _subject.Replace(item, "");
                            break;
                        }
                    }

                    // Episode, E로 시작하며 숫자 1~3개까지
                    Regex regex = new Regex(@"E\d{1,3}");
                    var results = regex.Matches(_subject);
                    if (results.Count >= 2)
                    {
                        _subject = results[0].Value;
                        Episode = int.Parse(results[1].Value.Substring(1));
                    }
                    else
                    {
                        // Error
                        Console.WriteLine("[Error] Episode 정규식 매칭 에러!, Episode 추출 실패");
                    }
                }
            }
            public string Resolution { get; private set; }
            public int Episode { get; private set; }
            public string Magnet { get; set; }
        }
    }
}
