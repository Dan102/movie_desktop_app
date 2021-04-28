
using IUR_p07.Model;
using IUR_p07.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace IUR_p07
{
    /// <summary>
    /// Needs to be extended to work properly
    /// </summary>
    public class MediaCardViewModel:ViewModelBase
    {
        // For deserialization into JSON
        public MediaCardViewModel() {}

        public MediaCardViewModel(MainViewModel mainViewModelReference, MainModel mainModelReference, SearchMovie foundMovie) {
            setCommonProperties(mainViewModelReference, mainModelReference, foundMovie);
            Title = foundMovie.Title;
            ReleaseDate = foundMovie.ReleaseDate;
            _mediaType = MyMediaType.Movie;
        }

        public MediaCardViewModel(MainViewModel mainViewModelReference, MainModel mainModelReference, SearchTv foundTvShow) {
            setCommonProperties(mainViewModelReference, mainModelReference, foundTvShow);
            Title = foundTvShow.Name;
            ReleaseDate = foundTvShow.FirstAirDate;
            _mediaType = MyMediaType.TvShow;
            _lastWatchedSeason = 1;
            _lastWatchedEpisode = 1;
        }

        private void setCommonProperties(MainViewModel mainViewModelReference, MainModel mainModelReference,  SearchMovieTvBase searchMovieTvBase) {
            _mainViewModelReference = mainViewModelReference;
            _mainModelReference = mainModelReference;
            Id = searchMovieTvBase.Id;
            Overview = searchMovieTvBase.Overview;
            PosterPath = searchMovieTvBase.PosterPath;
            VoteAverage = searchMovieTvBase.VoteAverage;
            GenreIds = searchMovieTvBase.GenreIds;
            Favourite = false;
            _myRating = 0;
            RecommendedBy = "";
        }


        // Needed update after JSON Reload, if property is public then JSON try to serialize it
        private MainViewModel _mainViewModelReference;
        public void SetMainViewModelReferenceJSONLoadin(MainViewModel MainViewModelReference) { _mainViewModelReference = MainViewModelReference; }
        private MainModel _mainModelReference;
        public void SetMainModelReferenceJSONLoadin(MainModel MainModelReference) { _mainModelReference = MainModelReference; }

        private RelayCommand _toggleToWatchMovieCommand;
        private RelayCommand _toggleWatchedMovieCommand;
        private RelayCommand _fetchMovieTrailerCommand;

        private string _title;
        private string _overview;
        private string _posterPath;
        private string _trailerPath;
        private DateTime? _releaseDate;
        private double _voteAverage;
        private bool _favourite;
        private bool _toWatch;
        private bool _watched;
        private MyMediaType _mediaType;
        private string _recommendedBy;
        private int _lastWatchedSeason;
        private int _lastWatchedEpisode;
        private int _myRating;

        public int Id { get; set; }
        public List<int> GenreIds { get; set; }
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged(); } }
        public string Overview { get { return _overview; } set { _overview = value; OnPropertyChanged(); } }
        public string PosterPath { get { return _posterPath; } set { _posterPath = value; OnPropertyChanged(); } }
        public string TrailerPath { get { return _trailerPath; } set { _trailerPath = value; OnPropertyChanged(); } }
        public DateTime? ReleaseDate { get { return _releaseDate; } set { _releaseDate = value; OnPropertyChanged(); } }
        public double VoteAverage { get { return _voteAverage; } set { _voteAverage = value; OnPropertyChanged(); } }
        public bool Favourite { get { return _favourite; } set { _favourite = value; OnPropertyChanged(); } }
        public bool ToWatch { get { return _toWatch; } set { _toWatch = value; OnPropertyChanged(); } }
        public bool Watched { get { return _watched; } set { _watched = value; OnPropertyChanged(); } }
        public MyMediaType MediaType { get { return _mediaType; } set { _mediaType = value; OnPropertyChanged(); } }
        public string RecommendedBy { get { return _recommendedBy; } set { _recommendedBy = value; OnPropertyChanged(); } }
        public int LastWatchedSeason { get { return _lastWatchedSeason; } set { _lastWatchedSeason = value; OnPropertyChanged(); } }
        public int LastWatchedEpisode { get { return _lastWatchedEpisode; } set { _lastWatchedEpisode = value; OnPropertyChanged(); } }
        public int MyRating { get { return _myRating; } set { _myRating = value; OnPropertyChanged(); } }


        public RelayCommand ToggleToWatchMovieCommand {
            get {
                return _toggleToWatchMovieCommand ?? (_toggleToWatchMovieCommand = new RelayCommand(ToggleToWatchMovie, (object obj) => { return true; }));
            }
        }

        private void ToggleToWatchMovie(object obj) {
            if (_mainModelReference.ToWatchMediaCardsDictionary.ContainsKey(Id)) {
                _mainModelReference.ToWatchMediaCardsDictionary.Remove(Id);
                _mainViewModelReference.ToWatchMediaCardsPart.Remove(this);
            } else {
                _mainModelReference.ToWatchMediaCardsDictionary.Add(Id, this);
            }
        }

        public RelayCommand ToggleWatchedMovieCommand {
            get {
                return _toggleWatchedMovieCommand ?? (_toggleWatchedMovieCommand = new RelayCommand(ToggleWatchedMovie, (object obj) => { return true; }));
            }
        }

        private void ToggleWatchedMovie(object obj) {
            if (_mainModelReference.WatchedMediaCardsDictionary.ContainsKey(Id)) {
                _mainModelReference.WatchedMediaCardsDictionary.Remove(Id);
                _mainViewModelReference.WatchedMediaCardsPart.Remove(this);
            } else {
                _mainModelReference.WatchedMediaCardsDictionary.Add(Id, this);
            }
        }

        public RelayCommand FetchMovieTrailerCommand {
            get {
                return _fetchMovieTrailerCommand ?? (_fetchMovieTrailerCommand = new RelayCommand(FetchMovieTrailer, (object obj) => { return true; }));
            }
        }

        private void FetchMovieTrailer(object obj) {
            TrailerPath = null;
            if (MediaType == MyMediaType.Movie) {
                TrailerPath = _mainModelReference.FetchMovieTrailerPath(Id);
            } else {
                TrailerPath = _mainModelReference.FetchTvShowTrailerPath(Id);
            }
            // No trailer available in the TMDB is marked by "" -> example is movie "Oz"
        }
    }

    public enum MyMediaType {
        Movie,
        TvShow
    }

}
