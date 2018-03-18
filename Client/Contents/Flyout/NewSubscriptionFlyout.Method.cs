using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace AniFile3.Contents
{
    public partial class NewSubscriptionFlyout
    {
        private ObservableCollection<TestSearchFromTMDB> _candidate;
        public ObservableCollection<TestSearchFromTMDB> Candidate { get => _candidate; }

        public class TestSearchFromTMDB
        {
            public string Title { get; set; }
            public string ThumbnailUrl { get; set; }

            public TestSearchFromTMDB()
            {
                Title = "Test";
            }
        }

        public void ChangedText(string text)
        {
            var client = new TMDbClient("9821beb972254f2129c3af73ca5a4419");
            var searchContainer = client.SearchMultiAsync(text).Result;
        }

        public void Something()
        {
            var a = new TMDbClient("9821beb972254f2129c3af73ca5a4419");
            var dd = a.SearchMultiAsync("무한도전").Result;

            var d2 = a.SearchMultiAsync("무한").Result;

            foreach (TMDbLib.Objects.Search.SearchMovieTvBase item in d2.Results)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
