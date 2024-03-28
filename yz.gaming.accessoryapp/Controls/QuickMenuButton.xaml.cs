using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
using static yz.gaming.accessoryapp.Controls.ItemEffect;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// QuickMenuButton.xaml 的交互逻辑
    /// </summary>
    public partial class QuickMenuButton : UserControl, IPageListItem
    {
        public delegate void QuickMenuButtonSelectedStateChangeHandler(IPageListItem sender, bool isSelected);
        public delegate void QuickMenuButtonClickHandler(IPageListItem sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
        static SolidColorBrush SELECTED_BRUSH = new SolidColorBrush(Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF));

        static SolidColorBrush DEFAULT_ICON_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
        static SolidColorBrush PUSHED_ICON_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x1A, 0xA7, 0x4F));

        static Thickness DEFAULT_THICKNESS = new Thickness(0, 0, 0, 0);
        static Thickness HOVER_THICKNESS = new Thickness(2, 2, 2, 2);

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event QuickMenuButtonClickHandler OnClick;

        TimeSpan _lastPressTime;

        public QuickMenuButton()
        {
            this.DataContext = this;
            InitializeComponent();

            this.Loaded += QuickMenuButton_Loaded;
        }

        private void QuickMenuButton_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonEffect(IsSelected, IsHoved);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(QuickMenuButton), new PropertyMetadata(string.Empty));

        public int ImageCornerRadius
        {
            get { return (int)GetValue(ImageCornerRadiusWidthProperty); }
            set { SetValue(ImageCornerRadiusWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageCornerRadiusWidthProperty =
            DependencyProperty.Register("ImageCornerRadius", typeof(int), typeof(QuickMenuButton), new PropertyMetadata(0));

        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set
            {
                SetValue(ImagePathProperty, value);
            }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(QuickMenuButton), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string SelectedImagePath
        {
            get { return (string)GetValue(SelectedImagePathProperty); }
            set
            {
                SetValue(SelectedImagePathProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedImagePathProperty =
            DependencyProperty.Register("SelectedImagePath", typeof(string), typeof(QuickMenuButton), new PropertyMetadata(DEFUALT_ICON_PATH));


        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(QuickMenuButton), new PropertyMetadata(DEFUALT_ICON_PATH));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                if (IsSelectable)
                {
                    SetValue(IsSelectedProperty, value);
                    SetButtonEffect(value, IsHoved);

                    OnSelectedStateChange?.Invoke(this, value);
                }
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(QuickMenuButton), new PropertyMetadata(false));

        public bool IsHoved
        {
            get { return (bool)GetValue(IsHovedProperty); }
            set
            {
                if (IsSelectable)
                {
                    SetValue(IsHovedProperty, value);
                    SetButtonEffect(IsSelected, value);
                    OnHovedStateChange?.Invoke(this, value);
                    IsSelected = value;
                }
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(QuickMenuButton), new PropertyMetadata(false));

        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set
            {
                SetValue(IsPressedProperty, value);
                SetButtonEffect(IsSelected, IsHoved);
            }
        }

        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(QuickMenuButton), new PropertyMetadata(false));


        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(QuickMenuButton), new PropertyMetadata(0));


        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

        public static readonly DependencyProperty IsSelectableProperty =
            DependencyProperty.Register("IsSelectable", typeof(bool), typeof(QuickMenuButton), new PropertyMetadata(true));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(QuickMenuButton), new PropertyMetadata(0d));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(QuickMenuButton), new PropertyMetadata(0d));

        public double TextWidth
        {
            get { return (double)GetValue(TextWidthProperty); }
            set { SetValue(TextWidthProperty, value); }
        }

        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidth", typeof(double), typeof(QuickMenuButton), new PropertyMetadata(0d));

        public double TextHeight
        {
            get { return (double)GetValue(TextHeightProperty); }
            set { SetValue(TextHeightProperty, value); }
        }

        public static readonly DependencyProperty TextHeightProperty =
            DependencyProperty.Register("TextHeight", typeof(double), typeof(QuickMenuButton), new PropertyMetadata(0d));

        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(double), typeof(QuickMenuButton), new PropertyMetadata(0d));

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsHoved = true;
            IsPressed = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            IsPressed = false;
            if (!CheckPress()) return;
            OnClick?.Invoke(this);
        }


        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            IsHoved = true;
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            IsPressed = false;
            if (!CheckPress()) return;
            OnClick?.Invoke(this);
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);

            IsHoved = true;
            IsPressed = true;
        }

        protected override void OnTouchEnter(TouchEventArgs e)
        {
            base.OnTouchEnter(e);

            IsHoved = true;
        }

        public void SetButtonEffect(bool isSelected)
        {
            MainBorder.Background = isSelected ? SELECTED_BRUSH : DEFAULT_BRUSH;
        }

        public void SetButtonEffect(bool isSelected, bool isHover)
        {
            SetButtonEffect(isSelected);
            MainBorder.Background = isSelected ? SELECTED_BRUSH : DEFAULT_BRUSH;
            ButtonBorder.Margin = IsPressed ? HOVER_THICKNESS : DEFAULT_THICKNESS;
            ButtonBorder.Background = IsPressed ? PUSHED_ICON_BRUSH : DEFAULT_ICON_BRUSH;
        }

        public void ConfirmPressed()
        {
            if (!IsSelected || !IsHoved) return;

            IsPressed = true;

            Task.Run(() =>
            {
                Thread.Sleep(200);
                Application.Current.Dispatcher.BeginInvoke(new Action(() => IsPressed = false));
            });

            if (!CheckPress()) return;
            OnClick?.Invoke(this);
        }

        private bool CheckPress()
        {
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            if (now.Subtract(_lastPressTime).TotalSeconds < 1) return false;
            _lastPressTime = now;

            return true;
        }
    }
}