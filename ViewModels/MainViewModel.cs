using IUR_p07.Model;
using IUR_p07.Support;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace IUR_p07
{
    public class MainViewModel : ViewModelBase {

        public ObservableCollection<MediaCardViewModel> FoundMediaCards { get; set; } = new ObservableCollection<MediaCardViewModel>();
        // Part of Collection which is being shown to user
        public ObservableCollection<MediaCardViewModel> ToWatchMediaCardsPart { get; set; } = new ObservableCollection<MediaCardViewModel>();
        public ObservableCollection<MediaCardViewModel> WatchedMediaCardsPart { get; set; } = new ObservableCollection<MediaCardViewModel>();

        // For turning off unfocused browsers HOTFIX in MainWindow.xaml
        public bool trailerStartedPlayingRecently = false;

        private MainModel _mainModelReference;

        public MainViewModel() {

            _mainModelReference = new MainModel(this);
            _mainModelReference.KeyWordsMovieSearchUpdated += Model_KeyWordsMovieSearchUpdated;
            _mainModelReference.KeyWordsTvShowSearchUpdated += Model_KeyWordsTvShowSearchUpdated;
            _mainModelReference.DiscoverMovieSearchUpdated += Model_DiscoverMovieSearchUpdated;
            _mainModelReference.DiscoverTvShowSearchUpdated += Model_DiscoverTvShowSearchUpdated;

            // Reset search filters
            SelectedMediaSearch = MediaTypeOption.Movie;
            SelectedDiscover = DiscoverOption.TopRated;
            ResetInternalFilters();
        }

        public MediaTypeOption SelectedMediaSearch { get; set; }

        private string _lastInternalSearch = "";

        private MediaTypeOption _selectedMediaToWatch;
        public MediaTypeOption SelectedMediaToWatch {
            get {
                return _selectedMediaToWatch;
            }
            set {
                _selectedMediaToWatch = value;
                OnPropertyChanged("SelectedMediumToWatch");
                RefreshInternalSearch(ToWatchMediaCardsPart, _mainModelReference.ToWatchMediaCardsDictionary, SelectedGenreToWatch, SelectedMediaToWatch, SelectedSortToWatch);
            }
        }

        private MediaTypeOption _selectedMediaWatched;
        public MediaTypeOption SelectedMediaWatched {
            get {
                return _selectedMediaWatched;
            }
            set {
                _selectedMediaWatched = value;
                OnPropertyChanged("SelectedMediumWatched");
            RefreshInternalSearch(WatchedMediaCardsPart, _mainModelReference.WatchedMediaCardsDictionary, SelectedGenreWatched, SelectedMediaWatched, SelectedSortWatched);
            }
        }

        // Allowing binding to Combobox
        public IEnumerable<MediaTypeOption> MediaTypeOptionValues {
            get {
                return Enum.GetValues(typeof(MediaTypeOption)).Cast<MediaTypeOption>();
            }
        }

        private SortOption _selectedSortToWatch;
        public SortOption SelectedSortToWatch {
            get {
                return _selectedSortToWatch;
            }
            set {
                _selectedSortToWatch = value;
                OnPropertyChanged("SelectedSortToWatch");
                SortCollectionsCards(ToWatchMediaCardsPart, SelectedSortToWatch);
            }
        }

        private SortOption _selectedSortWatched;
        public SortOption SelectedSortWatched {
            get {
                return _selectedSortWatched;
            }
            set {
                _selectedSortWatched = value;
                OnPropertyChanged("ChosenSortWatched");
                SortCollectionsCards(WatchedMediaCardsPart, SelectedSortWatched);
            }
        }

        // Allowing binding to Combobox
        public IEnumerable<SortOption> SortOptionValues {
            get {
                return Enum.GetValues(typeof(SortOption)).Cast<SortOption>();
            }
        }

        private DiscoverOption _selectedDiscover;
        public DiscoverOption SelectedDiscover {
            get {
                return _selectedDiscover;
            }
            set {
                _selectedDiscover = value;
                OnPropertyChanged("SelectedDiscover");
            }
        }

        // Allowing binding to Combobox
        public IEnumerable<DiscoverOption> DiscoverOptionValues {
            get {
                return Enum.GetValues(typeof(DiscoverOption)).Cast<DiscoverOption>();
            }
        }

        private MyGenre _selectedGenreToWatch;
        public MyGenre SelectedGenreToWatch {
            get {
                return _selectedGenreToWatch;
            }
            set {
                _selectedGenreToWatch = value;
                OnPropertyChanged("SelectedGenreToWatch");
                RefreshInternalSearch(ToWatchMediaCardsPart, _mainModelReference.ToWatchMediaCardsDictionary, SelectedGenreToWatch, SelectedMediaToWatch, SelectedSortToWatch);
            }
        }

        private MyGenre _selectedGenreWatched;
        public MyGenre SelectedGenreWatched {
            get {
                return _selectedGenreWatched;
            }
            set {
                _selectedGenreWatched = value;
                OnPropertyChanged("SelectedGenreWatched");
                RefreshInternalSearch(WatchedMediaCardsPart, _mainModelReference.WatchedMediaCardsDictionary, SelectedGenreWatched, SelectedMediaWatched, SelectedSortWatched);
            }
        }

        // Allowing binding to Combobox
        public IEnumerable<MyGenre> MyGenreValues {
            get {
                return Enum.GetValues(typeof(MyGenre)).Cast<MyGenre>();
            }
        }

        // Two Properties for searchTerm to allow different values storing in Searched/Saved Textboxes
        private string _internalSearchTerm;
        public string InternalSearchTerm {
            get {
                return _internalSearchTerm;
            }
            set {
                _internalSearchTerm = value;
                if(value == "" || value == null) {
                    _lastInternalSearch = "";
                }
                OnPropertyChanged("InternalSearchTerm");
            }
        }

        private string _externalSearchTerm;
        public string ExternalSearchTerm {
            get {
                return _externalSearchTerm;
            }
            set {
                _externalSearchTerm = value;
                OnPropertyChanged("ExternalSearchTerm");
            }
        }

        // Not elegant solution for changing second side of card in each tab
        public static int SELECTED_TAB_INDEX { get; set; }

        private int _selectedTabIndex;
        public int SelectedTabIndex {
            get {
                return _selectedTabIndex;
            }
            set {
                SELECTED_TAB_INDEX = _selectedTabIndex = value;
                ResetTrailerPathsInMediaCollection(FoundMediaCards);
                InternalSearchTerm = "";
                // Put all saved cards into display when changing tabs
                switch (value) {
                    case 1:
                        RefreshInternalSearch(ToWatchMediaCardsPart, _mainModelReference.ToWatchMediaCardsDictionary, SelectedGenreToWatch, SelectedMediaToWatch, SelectedSortToWatch);
                        break;
                    case 2:
                        RefreshInternalSearch(WatchedMediaCardsPart, _mainModelReference.WatchedMediaCardsDictionary, SelectedGenreWatched, SelectedMediaWatched, SelectedSortWatched);
                        break;
                }
                OnPropertyChanged("SelectedTabIndex");
            }
        }

        private void ResetInternalFilters() {
            _lastInternalSearch = "";
            InternalSearchTerm = "";
            SelectedSortToWatch = SortOption.Title;
            SelectedSortWatched = SortOption.Title;
            SelectedGenreToWatch = MyGenre.All;
            SelectedGenreWatched = MyGenre.All;
            SelectedMediaToWatch = MediaTypeOption.Both;
            SelectedMediaWatched = MediaTypeOption.Both;
        }

        private RelayCommand _addFoundMediaByKeywordsCommand;

        public RelayCommand AddFoundMediaByKeywordsCommand {
            get {
                return _addFoundMediaByKeywordsCommand ?? (_addFoundMediaByKeywordsCommand = new RelayCommand(AddFoundMediaByKeywordsItems, AddFoundMediaByKeywordsCommandCanExecute));
            }
        }

        private bool AddFoundMediaByKeywordsCommandCanExecute(object obj) {
            return ExternalSearchTerm != null && ExternalSearchTerm != "";
        }

        private void AddFoundMediaByKeywordsItems(object obj) {
            Mouse.OverrideCursor = Cursors.Wait;
            ResetTrailerPathsInMediaCollection(FoundMediaCards);
            FoundMediaCards.Clear();
            if (SelectedMediaSearch == MediaTypeOption.Movie) {
                _mainModelReference.SearchDatabaseMovieKeywords(ExternalSearchTerm, MyMediaType.Movie);
            } else {
                _mainModelReference.SearchDatabaseMovieKeywords(ExternalSearchTerm, MyMediaType.TvShow);
            }
        }

        private void Model_KeyWordsMovieSearchUpdated(SearchContainer<SearchMovie> results) {
            foreach (SearchMovie result in results.Results) {
                AddToFoundMediaCards(result);
            }
            Mouse.OverrideCursor = null;
        }

        private void Model_KeyWordsTvShowSearchUpdated(SearchContainer<SearchTv> results) {
            foreach (SearchTv result in results.Results) {
                AddToFoundMediaCards(result);
            }
            Mouse.OverrideCursor = null;
        }

        private RelayCommand _addFoundMediaByDiscoverCommand;

        public RelayCommand AddFoundMediaByDiscoverCommand {
            get {
                return _addFoundMediaByDiscoverCommand ?? (_addFoundMediaByDiscoverCommand = new RelayCommand(AddFoundMediaByDiscoverItems, (object obj) => { return true; }));
            }
        }

        private void AddFoundMediaByDiscoverItems(object obj) {
            Mouse.OverrideCursor = Cursors.Wait;
            ResetTrailerPathsInMediaCollection(FoundMediaCards);
            FoundMediaCards.Clear();
            if (SelectedMediaSearch == MediaTypeOption.Movie) {
                _mainModelReference.SearchDatabaseMovieDiscover(SelectedDiscover, MyMediaType.Movie);
            } else {
                _mainModelReference.SearchDatabaseMovieDiscover(SelectedDiscover, MyMediaType.TvShow);
            }
        }

        private void Model_DiscoverMovieSearchUpdated(SearchContainer<SearchMovie> results) {
            foreach (SearchMovie result in results.Results) {
                AddToFoundMediaCards(result);
            }
            Mouse.OverrideCursor = null;
        }

        private void Model_DiscoverTvShowSearchUpdated(SearchContainer<SearchTv> results) {
            foreach (SearchTv result in results.Results) {
                AddToFoundMediaCards(result);
            }
            Mouse.OverrideCursor = null;
        }

        // TODO: Visitor Pattern
        private void AddToFoundMediaCards(SearchMovie result) {
            if (_mainModelReference.ToWatchMediaCardsDictionary.ContainsKey(result.Id) || _mainModelReference.WatchedMediaCardsDictionary.ContainsKey(result.Id)) {
                FoundMediaCards.Add(GetSavedMediaFromCollections(result.Id));
            } else {
                FoundMediaCards.Add(new MediaCardViewModel(this, _mainModelReference, result));
            }
        }

        private void AddToFoundMediaCards(SearchTv result) {
            if (_mainModelReference.ToWatchMediaCardsDictionary.ContainsKey(result.Id) || _mainModelReference.WatchedMediaCardsDictionary.ContainsKey(result.Id)) {
                FoundMediaCards.Add(GetSavedMediaFromCollections(result.Id));
            } else {
                FoundMediaCards.Add(new MediaCardViewModel(this, _mainModelReference, result));
            }
        }

        private MediaCardViewModel GetSavedMediaFromCollections(int id) {
            if (_mainModelReference.ToWatchMediaCardsDictionary.ContainsKey(id)) {
                return _mainModelReference.ToWatchMediaCardsDictionary[id];
            } else if (_mainModelReference.WatchedMediaCardsDictionary.ContainsKey(id)) {
                return _mainModelReference.WatchedMediaCardsDictionary[id];
            }
            // Better to throw Exception
            return null;
        }

        private RelayCommand _addToWatchMediaContainsPartCommand;

        public RelayCommand AddToWatchMediaContainPartCommand {
            get {
                return _addToWatchMediaContainsPartCommand ?? (_addToWatchMediaContainsPartCommand = new RelayCommand(AddToWatchMediaContainsPartItems, AddToWatchMediaContainPartCommandCanExecute));
            }
        }

        private bool AddToWatchMediaContainPartCommandCanExecute(object obj) {
            return InternalSearchTerm != null && InternalSearchTerm != "";
        }

        private void AddToWatchMediaContainsPartItems(object obj) {
            _lastInternalSearch = InternalSearchTerm;
            RefreshInternalSearch(ToWatchMediaCardsPart, _mainModelReference.ToWatchMediaCardsDictionary, SelectedGenreToWatch, SelectedMediaToWatch, SelectedSortToWatch);
        }

        private RelayCommand _addWatchedMediaContainsPartCommand;

        public RelayCommand AddWatchedMediaContainsPartCommand {
            get {
                return _addWatchedMediaContainsPartCommand ?? (_addWatchedMediaContainsPartCommand = new RelayCommand(AddWatchedMediaContainsPartItems, AddWatchedMediaContainPartCommandCanExecute));
            }
        }

        private bool AddWatchedMediaContainPartCommandCanExecute(object obj) {
            return InternalSearchTerm != null && InternalSearchTerm != "";
        }

        private void AddWatchedMediaContainsPartItems(object obj) {
            _lastInternalSearch = InternalSearchTerm;
            RefreshInternalSearch(WatchedMediaCardsPart, _mainModelReference.WatchedMediaCardsDictionary, SelectedGenreWatched, SelectedMediaWatched, SelectedSortWatched);
        }

        private void AddAllToCollections(ObservableCollection<MediaCardViewModel> cardsPart, Dictionary<int, MediaCardViewModel> dictionary) {
            cardsPart.Clear();
            foreach (var movie in dictionary.Values) {
                cardsPart.Add(movie);
            }
        }

        private void AddContainsToCollections(ObservableCollection<MediaCardViewModel> cardsPart, Dictionary<int, MediaCardViewModel> dictionary, string searchTerm) {
            cardsPart.Clear();
            string lowerSearchTerm = searchTerm.ToLower();
            foreach (var movie in dictionary.Values) {
                if (movie.Title.ToLower().Contains(lowerSearchTerm)) {
                    cardsPart.Add(movie);
                }

            }
        }

        private void RemoveFromCollectionByGenre(ObservableCollection<MediaCardViewModel> cardsPart, MyGenre genre) {
            if (genre == MyGenre.All) {
                return;
            }
            foreach (var media in cardsPart.ToList()) {
                int targetMovieGendreId = _mainModelReference.MovieIdGenreDictionary[genre];
                int targetTVGendreId = _mainModelReference.TVShowIdGenreDictionary[genre];
                if (!media.GenreIds.Contains(targetMovieGendreId) & !media.GenreIds.Contains(targetTVGendreId)) {
                    cardsPart.Remove(media);
                }                
            }
        }

        private void RemoveFromCollectionsByMediaType(ObservableCollection<MediaCardViewModel> cardsPart, MediaTypeOption mediaType) {
            if (mediaType == MediaTypeOption.Both) {
                return;
            }
            var selectedMediaType = (MyMediaType)(int)mediaType;
            // Collection was modified; enumeration operation may not execute
            foreach (var media in cardsPart.ToList()) {
                if (media.MediaType != selectedMediaType) {
                    cardsPart.Remove(media);
                }
            }
        }

        private void RefreshInternalSearch(ObservableCollection<MediaCardViewModel> cardsPart, Dictionary<int, MediaCardViewModel> dictionary,
                                           MyGenre genre, MediaTypeOption mediaType, SortOption sortOption) {
            if (_lastInternalSearch == "") {
                AddAllToCollections(cardsPart, dictionary);
            } else {
                AddContainsToCollections(cardsPart, dictionary, _lastInternalSearch);
            }
            RefreshMediaChoiceFilters(cardsPart, genre, mediaType, sortOption);
        }

        private void RefreshMediaChoiceFilters(ObservableCollection<MediaCardViewModel> cardsPart,
                                               MyGenre genre, MediaTypeOption mediaType, SortOption sortOption) {
            RemoveFromCollectionByGenre(cardsPart, genre);
            RemoveFromCollectionsByMediaType(cardsPart, mediaType);
            SortCollectionsCards(cardsPart, sortOption);
        }

        // TODO Proper way to sort ObservableCollection is to extend base ObservableCollection and make use of internal CollectionChanged events
        private void SortCollectionsCards(ObservableCollection<MediaCardViewModel> cardsPart, SortOption sortOption) {
            ObservableCollection<MediaCardViewModel> newCollection = new ObservableCollection<MediaCardViewModel>();
            switch (sortOption) {
                case SortOption.Release:
                    newCollection = new ObservableCollection<MediaCardViewModel>(cardsPart.OrderBy(i => i.ReleaseDate));
                    break;
                case SortOption.Rating:
                    newCollection = new ObservableCollection<MediaCardViewModel>(cardsPart.OrderByDescending(i => i.VoteAverage));
                    break;
                case SortOption.Title:
                    newCollection = new ObservableCollection<MediaCardViewModel>(cardsPart.OrderBy(i => i.Title));
                    break;
                case SortOption.MyRating:
                    newCollection = new ObservableCollection<MediaCardViewModel>(cardsPart.OrderByDescending(i => i.MyRating));
                    break;
                case SortOption.Favourite:
                    newCollection = new ObservableCollection<MediaCardViewModel>(cardsPart.OrderByDescending(i => i.Favourite));
                    break;
            }
            // Otherwise, change wont propagate to frontend
            cardsPart.Clear();
            foreach (var media in newCollection) {
                cardsPart.Add(media);
            }
        }

        // Needed to stop playing trailers when switching video
        private void ResetTrailerPathsInMediaCollection(ObservableCollection<MediaCardViewModel> mediCollection) {
            foreach(var media in mediCollection.ToList()) {
                media.TrailerPath = null;
            }
        }

        private RelayCommand _closeWindowCommand;

        public RelayCommand CloseWindowCommand {
            get {
                return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand(CloseWindow, (object obj) => { return true; }));
            }
        }

        private void CloseWindow(object obj) {
            _mainModelReference.SaveMediaToJSONFile();
        }

    }

    public class EnumBooleanConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value.ToString()).Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((bool)value) ? parameter : Binding.DoNothing;
        }

    }

    public enum MediaTypeOption {
        Movie,
        TvShow,
        Both
    }

    public enum SortOption {
        Title,
        Release,
        Rating,
        MyRating,
        Favourite
    }

    public enum MyGenre {
        All,
        Action,
        Adventure,
        Comedy,
        Thriller,
        Fantasy,
        Scifi,
        Romance
    }

    public enum DiscoverOption {
        Popular,
        TopRated,
        Trending
    }

}

