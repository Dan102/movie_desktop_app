using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IUR_p07.Themes {
    
    public sealed partial class Generic : ResourceDictionary {

        // Scrollviewer in Media Overview handles the scroll event, but we need the scrolling to bubble through
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            if (!e.Handled) {
                ScrollViewer scrollViewer = (ScrollViewer)sender;
                if (e.Delta < 0 && scrollViewer.VerticalOffset != scrollViewer.ScrollableHeight) {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 20);
                } else if (e.Delta > 0 && scrollViewer.VerticalOffset != 0 && scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible) {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 20);
                } else {
                    e.Handled = true;
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                    eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                    eventArg.Source = sender;
                    var parent = ((Control)sender).Parent as UIElement;
                    parent.RaiseEvent(eventArg);
                }
            }
        }


        // Scrollviewer in Media Overview handles the mouse down, but we need the click to bubble through
        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            var mouseClickPositionRelativeToScrollViewer = e.GetPosition(scrollViewer);
            // If scrollbar clicked,
            if (!e.Handled && mouseClickPositionRelativeToScrollViewer.X < scrollViewer.ViewportWidth) {
                e.Handled = true;
                var eventArg = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
                eventArg.RoutedEvent = UIElement.MouseDownEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
                
            }
        }

        private void WebBrowser_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (!e.Handled) {
                e.Handled = true;
                var eventArg = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
                eventArg.RoutedEvent = UIElement.MouseDownEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }
    }
}
