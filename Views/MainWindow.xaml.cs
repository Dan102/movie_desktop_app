using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IUR_p07
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static bool videoStartedRecently = false;

        private void CloseAllPlayingBrowsers() {
            if (videoStartedRecently) {
                foreach (WebBrowser wb in FindVisualChildren<WebBrowser>(this)) {
                    if (wb.IsLoaded) {
                        wb.Navigate("about:blank");
                    }
                    wb.Visibility = Visibility.Hidden;
                    videoStartedRecently = false;
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject {
            if (depObj != null) {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T) {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child)) {
                        yield return childOfChild;
                    }
                }
            }
        }

        // TODO: Make all method trough binding, couldnt solve the scroll event so far
        private void ListBox_MouseWheel(object sender, ScrollChangedEventArgs e) {
            CloseAllPlayingBrowsers();
        }

    }

}
