using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace AniFile3.Contents
{
    public partial class NewSubscriptionFlyout
    {
        private TMDbClient _client;
        private CancellationTokenSource _ctSource;

        private ObservableCollection<SearchResult> _candidate;
        public ObservableCollection<SearchResult> Candidate { get => _candidate; }

        public class SearchResult
        {
            public string Title { get; set; }
            public string ThumbnailUrl { get; set; }

            public SearchResult()
            {
                Title = "Test";
            }
        }

        private void InitializeMethod()
        {
            _client = new TMDbClient("9821beb972254f2129c3af73ca5a4419");
            _candidate = new ObservableCollection<SearchResult>();
            CandidateView.ItemsSource = Candidate;
        }

        public async void ChangedText(string text)
        {
            //for guide to user
            Candidate.Clear();
            Candidate.Add(new SearchResult() { Title = "Searching..." });

            _ctSource?.Cancel();
            _ctSource = new CancellationTokenSource();

            try
            {
                var searchContainer = await _client.SearchMultiAsync(text, cancellationToken: _ctSource.Token);

                Candidate.Clear();

                foreach (var item in searchContainer.Results)
                {
                    string name = string.Empty;
                    switch (item.MediaType)
                    {
                        case TMDbLib.Objects.General.MediaType.Movie:
                            name = (item as TMDbLib.Objects.Search.SearchMovie).OriginalTitle;
                            break;
                        case TMDbLib.Objects.General.MediaType.Tv:
                            name = (item as TMDbLib.Objects.Search.SearchTv).OriginalName;
                            break;
                    }

                    Candidate.Add(new SearchResult()
                    {
                        Title = name
                    });
                }
            }
            catch(OperationCanceledException)
            {}
        }
    }
}
