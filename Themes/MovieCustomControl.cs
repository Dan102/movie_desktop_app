using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace IUR_p07
{
    public class MovieCustomControl : ToggleButton
    {
        static MovieCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MovieCustomControl), new FrameworkPropertyMetadata(typeof(MovieCustomControl)));
        }

        public string Title {
            get {
                return (string)GetValue(TitleProperty);
            }
            set {
                SetValue(TitleProperty, value);
            }
        }

        public string Overview {
            get {
                return (string)GetValue(OverviewProperty);
            }
            set {
                SetValue(OverviewProperty, value);
            }
        }

        public string PosterPath {
            get {
                return (string)GetValue(PosterPathProperty);
            }
            set {
                SetValue(PosterPathProperty, value);
            }
        }

        public string TrailerPath {
            get {
                return (string)GetValue(TrailerPathProperty);
            }
            set {
                SetValue(TrailerPathProperty, value);
            }
        }

        public DateTime? ReleaseDate {
            get {
                return (DateTime)GetValue(ReleaseDateProperty);
            }
            set {
                SetValue(ReleaseDateProperty, value);
            }
        }

        public double VoteAverage {
            get {
                return (double)GetValue(VoteAverageProperty);
            }
            set {
                SetValue(VoteAverageProperty, value);
            }
        }

        public bool Favourite {
            get {
                return (bool)GetValue(FavouriteProperty);
            }
            set {
                SetValue(FavouriteProperty, value);
            }
        }

        public bool ToWatch {
            get {
                return (bool)GetValue(ToWatchProperty);
            }
            set {
                SetValue(ToWatchProperty, value);
            }
        }

        public bool Watched {
            get {
                return (bool)GetValue(WatchedProperty);
            }
            set {
                SetValue(WatchedProperty, value);
            }
        }

        public MyMediaType MediaType {
            get {
                return (MyMediaType)GetValue(MediaTypeProperty);
            }
            set {
                SetValue(MediaTypeProperty, value);
            }
        }

        public string RecommendedBy {
            get {
                return (string)GetValue(RecommendedByProperty);
            }
            set {
                SetValue(RecommendedByProperty, value);
            }
        }

        public int LastWatchedSeason {
            get {
                return (int)GetValue(LastWatchedSeasonProperty);
            }
            set {
                SetValue(LastWatchedSeasonProperty, value);
            }
        }

        public int LastWatchedEpisode {
            get {
                return (int)GetValue(LastWatchedEpisodeProperty);
            }
            set {
                SetValue(LastWatchedEpisodeProperty, value);
            }
        }

        public int MyRating {
            get {
                return (int)GetValue(MyRatingProperty);
            }
            set {
                SetValue(MyRatingProperty, value);
            }
        }

        public ICommand ToggleToWatchMovieCommand {
            get {
                return (ICommand)GetValue(ToggleToWatchMovieCommandProperty);
            }
            set {
                SetValue(ToggleToWatchMovieCommandProperty, value);
            }
        }

        public ICommand ToggleWatchedMovieCommand {
            get {
                return (ICommand)GetValue(ToggleWatchedMovieCommandProperty);
            }
            set {
                SetValue(ToggleWatchedMovieCommandProperty, value);
            }
        }

        public ICommand FetchMovieTrailerCommand {
            get {
                return (ICommand)GetValue(FetchMovieTrailerCommandProperty);
            }
            set {
                SetValue(FetchMovieTrailerCommandProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty =
             DependencyProperty.Register("Title", typeof(string), typeof(MovieCustomControl));

        public static readonly DependencyProperty OverviewProperty =
            DependencyProperty.Register("Overview", typeof(string), typeof(MovieCustomControl));

        public static readonly DependencyProperty PosterPathProperty =
            DependencyProperty.Register("PosterPath", typeof(string), typeof(MovieCustomControl));

        public static readonly DependencyProperty TrailerPathProperty =
            DependencyProperty.Register("TrailerPath", typeof(string), typeof(MovieCustomControl));

        public static readonly DependencyProperty ReleaseDateProperty =
            DependencyProperty.Register("ReleaseDate", typeof(DateTime?), typeof(MovieCustomControl));

        public static readonly DependencyProperty VoteAverageProperty =
            DependencyProperty.Register("VoteAverage", typeof(double), typeof(MovieCustomControl));

        public static readonly DependencyProperty FavouriteProperty =
            DependencyProperty.Register("Favourite", typeof(bool), typeof(MovieCustomControl));

        public static readonly DependencyProperty ToWatchProperty =
            DependencyProperty.Register("ToWatch", typeof(bool), typeof(MovieCustomControl));

        public static readonly DependencyProperty WatchedProperty =
            DependencyProperty.Register("Watched", typeof(bool), typeof(MovieCustomControl));

        public static readonly DependencyProperty MediaTypeProperty =
            DependencyProperty.Register("MediaType", typeof(MyMediaType), typeof(MovieCustomControl));

        public static readonly DependencyProperty RecommendedByProperty =
            DependencyProperty.Register("RecommendedBy", typeof(string), typeof(MovieCustomControl));

        public static readonly DependencyProperty LastWatchedEpisodeProperty =
            DependencyProperty.Register("LastWatchedEpisode", typeof(int), typeof(MovieCustomControl));

        public static readonly DependencyProperty LastWatchedSeasonProperty =
            DependencyProperty.Register("LastWatchedSeason", typeof(int), typeof(MovieCustomControl));

        public static readonly DependencyProperty MyRatingProperty =
            DependencyProperty.Register("MyRating", typeof(int), typeof(MovieCustomControl));

        public static readonly DependencyProperty ToggleToWatchMovieCommandProperty =
            DependencyProperty.Register("ToggleToWatchMovieCommand", typeof(ICommand), typeof(MovieCustomControl));

        public static readonly DependencyProperty ToggleWatchedMovieCommandProperty =
            DependencyProperty.Register("ToggleWatchedMovieCommand", typeof(ICommand), typeof(MovieCustomControl));

        public static readonly DependencyProperty FetchMovieTrailerCommandProperty =
            DependencyProperty.Register("FetchMovieTrailerCommand", typeof(ICommand), typeof(MovieCustomControl));

    }

    public class BoolToVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool parameterBool = bool.Parse(parameter.ToString());
            if ((bool)value ^ parameterBool)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ToggleButtonStateToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            bool isChecked = (bool) value;
            string cardParameter = parameter as string;
            if (!isChecked) {
                if (cardParameter == "First") {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            } else {
                switch (MainViewModel.SELECTED_TAB_INDEX) {
                    case 0:
                        if (cardParameter == "SearchSecond") {
                            return Visibility.Visible;
                        } else {
                            return Visibility.Collapsed;
                        }
                    case 1:
                        if (cardParameter == "ToWatchSecond") {
                            return Visibility.Visible;
                        } else {
                            return Visibility.Collapsed;
                        }
                    case 2:
                        if (cardParameter == "WatchedSecond") {
                            return Visibility.Visible;
                        } else {
                            return Visibility.Collapsed;
                        }
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class DateToStringConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            DateTime? releaseDate = (value as DateTime?);

            if (releaseDate.HasValue) {
                return releaseDate.Value.Year.ToString();
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class PopularityToStringConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            double popularity = (double) value;

            if (popularity == 0) {
                return "Unknown";
            }
            return popularity.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class MediaTypeToVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            MyMediaType mediaType = (MyMediaType)value;

            if (mediaType == MyMediaType.TvShow) {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class TrailerPathToTextBoxVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string trailerPath = (string)value;

            if (trailerPath == "") {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class TrailerPathToButtonVisibilityConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string trailerPath = (string)value;

            if (trailerPath == "") {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class ReducedSizeConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            int size = int.Parse(value.ToString());

            return 0.8 * size;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class TheMovieDBAPIImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fullFilePath = System.IO.Path.GetFullPath(@"..\..\") +  @"Images\image_not_found.jpg";
            if (value as String != "" && value != null) {
                fullFilePath = String.Format("https://image.tmdb.org/t/p/w185_and_h278_bestv2{0}", value);
            }

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BrowserBehavior {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(BrowserBehavior),
            new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d) {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value) {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            WebBrowser wb = d as WebBrowser;
            if (wb != null && e.NewValue != null && ((string) e.NewValue) != "") {
                wb.Visibility = Visibility.Visible;
                Console.WriteLine(MainViewModel.SELECTED_TAB_INDEX);
                string url1 = "https://www.youtube.com/embed/" + (e.NewValue as string) + "?autoplay=1&showinfo=0&controls=0";
                string page =
                 "<html>"
                + "<head><meta http-equiv='X-UA-Compatible' content='IE=11' /></head>"
                + "<body>"
                + "<iframe src=\"" + url1 + "\" display=\"block\" width=\"100%\" height=\"240\" frameborder=\"0\" allow=\"autoplay\" allowfullscreen></iframe>"
                + "</body></html>";
                wb.NavigateToString(page);
                MainWindow.videoStartedRecently = true;
            } else {
                if (wb.IsLoaded) {
                    wb.Navigate("about:blank");
                }
            }
        }
    }

}
