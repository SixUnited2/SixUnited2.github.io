using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace yz.gaming.accessoryapp.Utils
{
    public static class ShowAlertUtils
    {
        public static void ShowAlert(string msg, bool alwaysShow = false)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Window? win = Application.Current.Windows[0];
                Grid grid = win?.Content as Grid;
                //Grid grid = win.TryGetChildPartFromVisualTree<Grid>();
                if (grid == null)
                {
                    return;
                }

                Popup popup = new Popup
                {
                    Placement = PlacementMode.Top,
                    PopupAnimation = PopupAnimation.Fade,
                    AllowsTransparency = true,
                    Margin = new Thickness(10),
                    VerticalOffset = 200,
                    HorizontalOffset = 600,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    IsOpen = true,
                    ToolTip = "Click to close"
                };

                Border border = new Border()
                {
                    Background = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF)),
                    CornerRadius = new CornerRadius(6)
                };

                TextBlock textBlock = new TextBlock
                {
                    Text = msg,
                    Style = (Style)grid.FindResource("TextBlock_Alert"),
                    Foreground = Brushes.White
                };

                border.Child = textBlock;

                textBlock.Loaded += (s, p) =>
                {
                    popup.HorizontalOffset = ((grid.ActualWidth / 2)) - ((textBlock.ActualWidth / 2));
                    popup.VerticalOffset = (grid.ActualHeight * 0.7);
                };

                if (alwaysShow)
                {
                    textBlock.Background = Brushes.Orchid;
                }
                else
                {
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Tick += (s, e) =>
                    {
                        grid.Children.Remove(popup);
                        timer.Stop();
                    };

                    timer.Interval = TimeSpan.FromMilliseconds(1200);
                    timer.Start();
                }

                popup.Child = border;
                grid.Children.Add(popup);
                popup.MouseLeftButtonUp += (s, e) =>
                {
                    grid.Children.Remove(popup);
                };
            }));
        }

    }
}
