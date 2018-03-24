using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
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
        public class SearchResult
        {
            public string Title { get; set; }
            public string ThumbnailUrl { get; set; }

            public SearchResult()
            {
                Title = "Test";
            }
        }

        private TMDbClient _client;
        private CancellationTokenSource _ctSource;
        private Popup _listPopup;
        private ListBox _candidateListBox;
        private bool _isIgnoreEvent;
        private CancellationTokenSource _waitCtSource;

        private string TextwithoutEvent
        {
            set
            {
                _isIgnoreEvent = true;
                MainTextBox.Text = value;
                _isIgnoreEvent = false;
            }
        }

        private ObservableCollection<SearchResult> _candidate;
        public ObservableCollection<SearchResult> Candidate { get => _candidate; }

        public static readonly RoutedEvent SearchEvent = EventManager.RegisterRoutedEvent(
            "Search"
            , RoutingStrategy.Bubble
            , typeof(RoutedEventHandler)
            , typeof(SearchTextbox));

        public event RoutedEventHandler Search
        {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }

        public SearchTextbox()
        {
            InitializeComponent();

            _client = new TMDbClient("9821beb972254f2129c3af73ca5a4419");
            _candidate = new ObservableCollection<SearchResult>();
            _isIgnoreEvent = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _listPopup = new Popup();
            _listPopup.PopupAnimation = PopupAnimation.Fade;
            _listPopup.Placement = PlacementMode.Relative;
            _listPopup.PlacementTarget = MainTextBox;
            _listPopup.MinWidth = MainTextBox.ActualWidth;
            _listPopup.StaysOpen = false;
            _listPopup.Closed += (s, ev) => _candidateListBox.SelectedIndex = -1;

            _candidateListBox = new ListBox();
            _candidateListBox.DisplayMemberPath = "Title";
            _candidateListBox.GotFocus += CandidateListBox_GotFocus;
            _candidateListBox.KeyUp += CandidateListBox_KeyUp;
            _candidateListBox.MaxHeight = 200;

            _candidateListBox.ItemsSource = Candidate;
            _listPopup.Child = _candidateListBox;
        }

        private void RaiseSearchEvent(string text)
        {
            SearchEventArgs args = new SearchEventArgs(SearchTextbox.SearchEvent, text);
            RaiseEvent(args);
        }

        private void ShowPopup()
        {
            _listPopup.PlacementRectangle = new Rect(0, MainTextBox.ActualHeight, 30, 30);
            _listPopup.IsOpen = true;
        }

        private void HidePopup()
        {
            _listPopup.IsOpen = false;
            _candidateListBox.SelectedIndex = -1;
        }

        private void SelectedCandidate(SearchResult selected)
        {
            TextwithoutEvent = selected.Title;

            HidePopup();
            MainTextBox.Focus();
            MainTextBox.CaretIndex = MainTextBox.Text.Length;
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

#region UI Event
        private void CandidateListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SelectedCandidate(_candidateListBox.SelectedItem as SearchResult);
            }
        }

        private void CandidateListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowPopup();
        }

        private void MainTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                RaiseSearchEvent(MainTextBox.Text);
            }
        }

        private void MainTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            HidePopup();
        }
        
        private void MainTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (_listPopup.IsOpen == false)
                    ShowPopup();

                _candidateListBox.SelectedIndex = 0;
                _candidateListBox.Focus();

                var item = _candidateListBox
                    .ItemContainerGenerator
                    .ContainerFromIndex(0) as ListBoxItem;
                item.Focus();
                e.Handled = true;
            }
        }

        private void MainTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ShowPopup();
        }
        
        private async void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isIgnoreEvent == false)
            {
                _waitCtSource?.Cancel();
                _waitCtSource = new CancellationTokenSource();

                try
                {
                    await Task.Delay(500, _waitCtSource.Token);

                    string text = (e.Source as TextBox).Text;
                    if (string.IsNullOrEmpty(text) == false)
                        ChangedText(text);
                }
                catch (OperationCanceledException)
                { }
            }
        }
    }
#endregion
}
