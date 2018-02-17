using Scriping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniFile3.MagnetLising
{
    // 이름 짓기 어렵네
    public class MagnetListUp
    {
        private FirstSite _scriper;
        // 여기에 RSS 기능도 추가

        public MagnetListUp()
        {
            // 스크랩퍼 초기화
            _scriper = new Scriping.FirstSite(new System.Windows.Forms.WebBrowser());
            _scriper.InitializeCompleted += async () =>
            {
                // 여기서 검색등 자료를 찾고, 그 자료를 서버로 보낸다.

                // Test
                //var result = await _scriper.SearchBox("아는 형님");

                //foreach(var item in result)
                //{
                //    Console.WriteLine("{0}:{1}", item.Name, item.Episode);
                //}
            };

            _scriper.Initialize();
        }
    }
}
