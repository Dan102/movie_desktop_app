using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace IUR_p07.Model {

    public class MainModel {

        public static readonly string JSON_FILENAME_SAVED_MEDIA = "saved_media.json";

        public TMDbClient client = new TMDbClient("94c8c1b39374b8bbc8f108c0d451b659");

        // Whole collection, which is serialized into file
        public Dictionary<int, MediaCardViewModel> ToWatchMediaCardsDictionary { get; set; } = new Dictionary<int, MediaCardViewModel>();
        public Dictionary<int, MediaCardViewModel> WatchedMediaCardsDictionary { get; set; } = new Dictionary<int, MediaCardViewModel>();

        // Movies and TV Shows have different genres IDs, but they are Disjoint, so we can check media genre for CONTAINS ONE OF THEM
        public Dictionary<MyGenre, int> MovieIdGenreDictionary { get; set; } = new Dictionary<MyGenre, int>() {
            {MyGenre.Action, 28},
            {MyGenre.Adventure, 12},
            {MyGenre.Comedy, 35},
            {MyGenre.Thriller, 53},
            {MyGenre.Fantasy, 14},
            {MyGenre.Scifi, 878},
            {MyGenre.Romance, 10749}
        };
        public Dictionary<MyGenre, int> TVShowIdGenreDictionary { get; set; } = new Dictionary<MyGenre, int>() {
            {MyGenre.Action, 10759},
            {MyGenre.Adventure, 10759},
            {MyGenre.Comedy, 35},
            {MyGenre.Thriller, 80},
            {MyGenre.Fantasy, 10765},
            {MyGenre.Scifi, 10765},
            {MyGenre.Romance, 10766}
        };

        public MainModel(MainViewModel mainViewModelReference) {
            // Load saved media in json
            try {
                string savedJson = File.ReadAllText(JSON_FILENAME_SAVED_MEDIA);
                ConvertAllSavedMediaFromJson(savedJson, mainViewModelReference);
            } catch (Exception ex) {
                Console.WriteLine("Couldn't load saved media from json file due to excpetoion." + Environment.NewLine + ex.Message);
            }
        }

        private string ConvertAllSavedMediaToJson() {
            ObservableCollection<MediaCardViewModel> allSavedMediaCards = new ObservableCollection<MediaCardViewModel>();
            foreach (var media in ToWatchMediaCardsDictionary.Values) {
                media.TrailerPath = null;
                allSavedMediaCards.Add(media);
            }
            foreach (var mediaKey in WatchedMediaCardsDictionary.Keys) {
                if (!ToWatchMediaCardsDictionary.ContainsKey(mediaKey)) {
                    WatchedMediaCardsDictionary[mediaKey].TrailerPath = null;
                    allSavedMediaCards.Add(WatchedMediaCardsDictionary[mediaKey]);
                }
            }
            return JsonConvert.SerializeObject(allSavedMediaCards);
        }

        private void ConvertAllSavedMediaFromJson(string jsonString, MainViewModel mainViewModelReference) {
            ObservableCollection<MediaCardViewModel> allSavedMediaCards = JsonConvert.DeserializeObject<ObservableCollection<MediaCardViewModel>>(jsonString);
            foreach (var media in allSavedMediaCards) {
                media.SetMainModelReferenceJSONLoadin(this);
                media.SetMainViewModelReferenceJSONLoadin(mainViewModelReference);
                if (media.ToWatch) {
                    ToWatchMediaCardsDictionary.Add(media.Id, media);
                }
                if (media.Watched) {
                    WatchedMediaCardsDictionary.Add(media.Id, media);
                }
            }
        }

        public void SaveMediaToJSONFile() {
            string jsonString = ConvertAllSavedMediaToJson();
            File.WriteAllText(JSON_FILENAME_SAVED_MEDIA, jsonString);
        }

        public event KeyWordsMovieSearchUpdatedDelegate KeyWordsMovieSearchUpdated;
        public delegate void KeyWordsMovieSearchUpdatedDelegate(SearchContainer<SearchMovie> results);

        public event KeyWordsTvShowSearchUpdatedDelegate KeyWordsTvShowSearchUpdated;
        public delegate void KeyWordsTvShowSearchUpdatedDelegate(SearchContainer<SearchTv> results);

        public event DiscoverMovieSearchUpdatedDelegate DiscoverMovieSearchUpdated;
        public delegate void DiscoverMovieSearchUpdatedDelegate(SearchContainer<SearchMovie> results);

        public event DiscoverTvShowSearchUpdatedDelegate DiscoverTvShowSearchUpdated;
        public delegate void DiscoverTvShowSearchUpdatedDelegate(SearchContainer<SearchTv> results);

        private MyMediaType _mediaType;

        private string _searchTerm;
        public void SearchDatabaseMovieKeywords(string searchTerm, MyMediaType mediaType) {
            _searchTerm = searchTerm;
            _mediaType = mediaType;
            SearchByKeywords();

        }

        private DiscoverOption _databaseDiscoverOption;        
        public void SearchDatabaseMovieDiscover(DiscoverOption databaseDiscoverOption, MyMediaType mediaType) {
            _databaseDiscoverOption = databaseDiscoverOption;
            _mediaType = mediaType;
            SearchByDiscover();

        }

        // TODO: make all queries async
        private void SearchByKeywords() {
            // Unfortunately, haven't found way how to cast Task<SearchContainer<SearchMovie>> to common parent
            if (_mediaType == MyMediaType.Movie) {
                Task<SearchContainer<SearchMovie>> searchTask = client.SearchMovieAsync(_searchTerm);
                searchTask.Wait();
                KeyWordsMovieSearchUpdated?.Invoke(searchTask.Result);
            } else {
                Task<SearchContainer<SearchTv>> searchTask = client.SearchTvShowAsync(_searchTerm);
                searchTask.Wait();
                KeyWordsTvShowSearchUpdated?.Invoke(searchTask.Result);
            }
        }
        
        // TODO: make all queries async
        private void SearchByDiscover() {
            if (_mediaType == MyMediaType.Movie) {
                Task<SearchContainer<SearchMovie>> searchTask = GetMovieSearchTaskByDiscover();
                searchTask.Wait();
                DiscoverMovieSearchUpdated(searchTask.Result);
            } else {
                Task<SearchContainer<SearchTv>> searchTask = GetTVShowSearchTaskByDiscover();
                searchTask.Wait();
                DiscoverTvShowSearchUpdated(searchTask.Result);
            }
        }

        private Task<SearchContainer<SearchMovie>> GetMovieSearchTaskByDiscover() {
            Task<SearchContainer<SearchMovie>> searchTask = null;
            switch (_databaseDiscoverOption) {
                case DiscoverOption.Popular:
                    searchTask = client.GetMoviePopularListAsync();
                    break;
                case DiscoverOption.TopRated:
                    searchTask = client.GetMovieTopRatedListAsync();
                    break;
                case DiscoverOption.Trending:
                    searchTask = client.GetTrendingMoviesAsync(new TMDbLib.Objects.Trending.TimeWindow());
                    break;
            }
            return searchTask;
        }

        private Task<SearchContainer<SearchTv>> GetTVShowSearchTaskByDiscover() {
            Task<SearchContainer<SearchTv>> searchTask = null;
            switch (_databaseDiscoverOption) {
                case DiscoverOption.Popular:
                    searchTask = client.GetTvShowPopularAsync();
                    break;
                case DiscoverOption.TopRated:
                    searchTask = client.GetTvShowTopRatedAsync();
                    break;
                case DiscoverOption.Trending:
                    searchTask = client.GetTrendingTvAsync(new TMDbLib.Objects.Trending.TimeWindow());
                    break;
            }
            return searchTask;
        }

        public string FetchMovieTrailerPath(int Id) {
            var searchTask = client.GetMovieAsync(Id, MovieMethods.Videos);
            searchTask.Wait();
            Movie movie = searchTask.Result;
            if (movie.Videos != null) {
                foreach (var video in movie.Videos.Results) {
                    if (video.Type == "Trailer") {
                        return video.Key;                        
                    }
                }
            }
            return "";
        }

        public string FetchTvShowTrailerPath(int Id) {
            var searchTask = client.GetTvShowAsync(Id, TvShowMethods.Videos);
            searchTask.Wait();
            TvShow movie = searchTask.Result;
            if (movie.Videos != null) {
                foreach (var video in movie.Videos.Results) {
                    if (video.Type == "Trailer") {
                        return video.Key;
                    }
                }
            }
            return "";
        }

    }
}
