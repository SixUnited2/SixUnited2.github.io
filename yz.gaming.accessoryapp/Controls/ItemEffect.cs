using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace yz.gaming.accessoryapp.Controls
{
    public class ItemEffect
    {
        public delegate void ItemSelectedStateChangeHandler(IPageListItem sender, bool isSelected);
        public delegate void ItemHovedStateChangeHandler(IPageListItem sender, bool isHoved);

        static SolidColorBrush DEFAULT_BACKGROUND_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x25, 0x25, 0x25));
        static SolidColorBrush SELECTED_BACKGROUND_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x58, 0x58, 0x58));

        static SolidColorBrush DEFAULT_BORDER_BRUSH = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
        static SolidColorBrush SELECTED_BORDER_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));

        static Thickness DEFAULT_BORDER_THICKNESS = new Thickness(0);
        static Thickness SELECTED_BORDER_THICKNESS = new Thickness(2);

        public Border Border { get; set; }
        public Grid MainGrid { get; set; }

        public SolidColorBrush DefaultBackgroundBrush { get; set; } = DEFAULT_BACKGROUND_BRUSH;
        public SolidColorBrush SelectedBackgroundBrush { get; set; } = SELECTED_BACKGROUND_BRUSH;

        public SolidColorBrush DefaultBorderBrush { get; set; } = DEFAULT_BORDER_BRUSH;
        public SolidColorBrush SelectedBorderBrush { get; set; } = SELECTED_BORDER_BRUSH;

        public Thickness DefaultBorderThickness { get; set; } = DEFAULT_BORDER_THICKNESS;
        public Thickness SelectedBorderThickness { get; set; } = SELECTED_BORDER_THICKNESS;

        TimeSpan _lastPressTime;

        public ItemEffect()
        {
            _lastPressTime = new TimeSpan(DateTime.Now.Ticks);
        }

        public void SetButtonEffect(bool isSelected, bool isHoved)
        {
            Border.Background = isSelected ? SelectedBorderBrush : DefaultBorderBrush;
            Border.BorderThickness = isSelected ? SelectedBorderThickness : DefaultBorderThickness;
            MainGrid.Background = isHoved ? SelectedBackgroundBrush : DefaultBackgroundBrush;
        }
        public void SetButtonEffect_GamePlatformButton(bool isSelected, bool isHoved)
        {
            Border.BorderThickness = isSelected ? SelectedBorderThickness : DefaultBorderThickness;
        }

        public bool CheckPress()
        {
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            if (now.Subtract(_lastPressTime).TotalSeconds < 1) return false;
            _lastPressTime = now;

            return true;
        }
    }
}
