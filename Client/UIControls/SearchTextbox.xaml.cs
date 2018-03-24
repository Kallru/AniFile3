using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TMDbLib.Client;

namespace AniFile3.UIControls
{
    /// <summary>
    /// SearchTextbox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SearchTextbox : UserControl
    {
        private TMDbClient _client;
        private CancellationTokenSource _ctSource;
        private Popup _listPopup;
        private ListBox _candidateListBox;

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

        public SearchTextbox()
        {
            InitializeComponent();

            _client = new TMDbClient("9821beb972254f2129c3af73ca5a4419");
            _candidate = new ObservableCollection<SearchResult>();

            _listPopup = new Popup();
            _listPopup.PopupAnimation = PopupAnimation.Fade;
            _listPopup.Placement = PlacementMode.Relative;
            _listPopup.PlacementTarget = this;
            _listPopup.PlacementRectangle = new Rect(0, SerachText.ActualHeight, 30, 30);
            //_listPopup.Width = this.ActualWidth;

            _candidateListBox = new ListBox();
            _candidateListBox.DisplayMemberPath = "Title";
            _candidateListBox.SelectionChanged += CandidateView_SelectionChanged;
            _candidateListBox.MaxHeight = 200;

            _candidateListBox.ItemsSource = Candidate;
            _listPopup.Child = _candidateListBox;

            Candidate.Add(new SearchResult());
            Candidate.Add(new SearchResult());
            Candidate.Add(new SearchResult());
            Candidate.Add(new SearchResult());
            Candidate.Add(new SearchResult() { Title="adsasdasdasdasdasd" });

            ShowPopup();
        }

        public void ShowPopup()
        {
            _listPopup.StaysOpen = true;
            _listPopup.IsOpen = true;
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
            catch (OperationCanceledException)
            { }
        }

        private void CandidateView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as SearchResult;
                //NameField.Text = item.Title;
            }
        }

        private void SerachText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                //Search();
            }
        }
    }
}
