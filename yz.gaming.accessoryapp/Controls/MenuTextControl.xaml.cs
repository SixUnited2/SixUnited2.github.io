using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// MenuTextControl.xaml 的交互逻辑
    /// </summary>
    public partial class MenuTextControl : UserControl, INavigationItem
    {
        public delegate void MenuTextSelectedStateChangeHandler(MenuTextControl sender, bool isSelected);

        const double DEFAULT_SIZE = 32;
        const double SELECTED_SIZE = 38;

        static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF));
        static SolidColorBrush SELECTED_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));

        public event MenuTextSelectedStateChangeHandler OnSelectedStateChange;

        public MenuTextControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MenuTextControl), new PropertyMetadata(string.Empty));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                MenuText.FontSize = value ? SELECTED_SIZE : DEFAULT_SIZE;
                MenuText.Foreground = value ? SELECTED_BRUSH : DEFAULT_BRUSH;
                UnderlineImage.Visibility = value ? Visibility.Visible: Visibility.Collapsed;

                //ContainerBorder.BorderBrush = value ? new SolidColorBrush(Color.FromArgb(255, 78, 192, 215)) : new SolidColorBrush(Colors.Transparent);
                //ContainerBorder.BorderThickness = value ? new Thickness(2) : new Thickness(0);
                //if (value)
                //{
                //    ContainerBorder.BorderThickness = new Thickness(5); // 设置边框的粗细为 5
                //}
                //else
                //{
                //    ContainerBorder.BorderThickness = new Thickness(0); // 设置边框的粗细为 0
                //}

                OnSelectedStateChange?.Invoke(this, value);
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MenuTextControl), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(MenuTextControl), new PropertyMetadata(0));

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsSelected = true;
        }
    }
}
