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
using yz.gaming.accessoryapp.Utils;
using static yz.gaming.accessoryapp.Controls.ItemEffect;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// CustomButton_H_V.xaml 的交互逻辑
    /// </summary>
    public partial class DynamicButtonControl : UserControl, IPageListItem, IPageButtonItem
    {
        public delegate void DynamicButtonClickHandler(IPageListItem sender);

        const int BORDER_WIDTH = 4;
        const int SELECTED_EXPEND = 7;
        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
        static SolidColorBrush SELECTED_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event DynamicButtonClickHandler OnClick;

        TimeSpan _lastPressTime;

        public enum IconDisplayTypeEnum
        {
            IconCenter,
            IconLeft,
            IconAndTextCenter,
            HomePage
        }

        public DynamicButtonControl()
        {
            this.DataContext = this;
            InitializeComponent();

            this.Loaded += DynamicButtonControl_Loaded;
            _lastPressTime = new TimeSpan(DateTime.Now.Ticks);
        }

        private void DynamicButtonControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonEffect(IsSelected, IsHoved);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DynamicButtonControl), new PropertyMetadata(string.Empty));

        public string Text1
        {
            get { return (string)GetValue(Text1Property); }
            set { SetValue(Text1Property, value); }
        }

        public static readonly DependencyProperty Text1Property =
            DependencyProperty.Register("Text1", typeof(string), typeof(DynamicButtonControl), new PropertyMetadata(string.Empty));

        public IconDisplayTypeEnum IconDisplayType
        {
            get { return (IconDisplayTypeEnum)GetValue(IconDisplayTypeProperty); }
            set { SetValue(IconDisplayTypeProperty, value); }
        }

        public static readonly DependencyProperty IconDisplayTypeProperty =
            DependencyProperty.Register("IconDisplayType", typeof(IconDisplayTypeEnum), typeof(DynamicButtonControl), new PropertyMetadata(IconDisplayTypeEnum.IconCenter));

        public int BorderWidth
        {
            get { return (int)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(int), typeof(DynamicButtonControl), new PropertyMetadata(BORDER_WIDTH));

        public int SelectedExpend
        {
            get { return (int)GetValue(SelectedExpendProperty); }
            set { SetValue(SelectedExpendProperty, value); }
        }

        public static readonly DependencyProperty SelectedExpendProperty =
            DependencyProperty.Register("SelectedExpend", typeof(int), typeof(DynamicButtonControl), new PropertyMetadata(SELECTED_EXPEND));

        public int BorderCornerRadius
        {
            get { return (int)GetValue(BorderCornerRadiusWidthProperty); }
            set { SetValue(BorderCornerRadiusWidthProperty, value); }
        }

        public static readonly DependencyProperty BorderCornerRadiusWidthProperty =
            DependencyProperty.Register("BorderCornerRadius", typeof(int), typeof(DynamicButtonControl), new PropertyMetadata(0));

        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set
            {
                SetValue(ImagePathProperty, value);
            }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(DynamicButtonControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(DynamicButtonControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                if (IsSelectable)
                {
                    SetValue(IsSelectedProperty, value);
                    ButtonBorder.Background = value ? SELECTED_BRUSH : DEFAULT_BRUSH;

                    SetButtonEffect(value, IsHoved);

                    OnSelectedStateChange?.Invoke(this, value);
                }
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DynamicButtonControl), new PropertyMetadata(false));

        public bool IsHoved
        {
            get { return (bool)GetValue(IsHovedProperty); }
            set
            {
                SetValue(IsHovedProperty, value);
                OnHovedStateChange?.Invoke(this, value);
                IsSelected = value;
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(DynamicButtonControl), new PropertyMetadata(false));


        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(DynamicButtonControl), new PropertyMetadata(0));


        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

        public static readonly DependencyProperty IsSelectableProperty =
            DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DynamicButtonControl), new PropertyMetadata(true));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set
            {
                if (value < 0) value = 0;
                SetValue(ImageWidthProperty, value);
            }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(DynamicButtonControl), new PropertyMetadata(10d));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set
            {
                if (value < 0) value = 0;
                SetValue(ImageHeightProperty, value);
            }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(DynamicButtonControl), new PropertyMetadata(10d));

        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set
            {
                if (value < 0) value = 0;
                SetValue(IconWidthProperty, value);
            }
        }

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(DynamicButtonControl), new PropertyMetadata(10d));

        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set
            {
                if (value < 0) value = 0;
                SetValue(IconHeightProperty, value);
            }
        }

        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(DynamicButtonControl), new PropertyMetadata(0d));

        public double TextWidth
        {
            get { return (double)GetValue(TextWidthProperty); }
            set
            {
                if (value < 0) value = 0;
                SetValue(TextWidthProperty, value);
            }
        }

        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidth", typeof(double), typeof(DynamicButtonControl), new PropertyMetadata(0d));

        public double TextHeight
        {
            get { return (double)GetValue(TextHeightProperty); }
            set
            {
                if (value < 0) value = 0;
                SetValue(TextHeightProperty, value);
            }
        }

        public static readonly DependencyProperty TextHeightProperty =
            DependencyProperty.Register("TextHeight", typeof(double), typeof(DynamicButtonControl), new PropertyMetadata(0d));

        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set
            {
                if (value <= 0) value = 10;
                SetValue(TextFontSizeProperty, value);
            }
        }

        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(double), typeof(DynamicButtonControl), new PropertyMetadata(10d));

        public bool IsHoverSelectable
        {
            get { return (bool)GetValue(IsHoverSelectableProperty); }
            set
            {
                SetValue(IsHoverSelectableProperty, value);
            }
        }

        public static readonly DependencyProperty IsHoverSelectableProperty =
            DependencyProperty.Register("IsHoverSelectable", typeof(bool), typeof(DynamicButtonControl), new PropertyMetadata(true));

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsSelected = true;
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            if (!CheckPress()) return;
            OnClick?.Invoke(this);
            IsSelected = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!CheckPress()) return;
            OnClick?.Invoke(this);
            IsSelected = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                if (!CheckPress()) return;
                OnClick?.Invoke(this);
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (IsHoverSelectable) IsSelected = true;
        }

        public void SetButtonEffect(bool isSelected, bool isHoved)
        {
            double scaling = SystemUtils.Instance.GetScreenScalingFactor();
            double actualBorderWidth = BorderWidth / scaling;
            double actualSelectedExpend = SelectedExpend / scaling;

            ImageWidth = isSelected ? ActualWidth - actualBorderWidth * 2 : ActualWidth - actualBorderWidth * 2 - actualSelectedExpend * 4;
            ImageHeight = isSelected ? ActualHeight - actualBorderWidth * 4 : ActualHeight - actualBorderWidth * 2 - actualSelectedExpend * 4;

            switch (IconDisplayType)
            {
                case IconDisplayTypeEnum.IconLeft:
                    IconLeft.Visibility = Visibility.Visible;
                    IconHeight = ImageHeight / 2;
                    IconWidth = IconHeight;
                    TextWidth = ImageWidth / 3;
                    break;
                case IconDisplayTypeEnum.IconAndTextCenter:
                    IconAndTextCenter.Visibility = Visibility.Visible;
                    IconWidth = ImageHeight / 2.5;
                    IconHeight = IconWidth;
                    TextHeight = ImageHeight / 4;
                    ImageHeight = ImageHeight - 2;
                    break;
                case IconDisplayTypeEnum.IconCenter:
                    IconCenter.Visibility = Visibility.Visible;
                    IconWidth = ImageWidth / 2;
                    IconHeight = IconWidth;
                    TextHeight = ImageHeight / 4;
                    break;
                case IconDisplayTypeEnum.HomePage:
                    IconAndTextCenter.Visibility = Visibility.Visible;
                    IconWidth = ImageWidth / 3.5;
                    IconHeight = IconWidth;
                    TextHeight = ImageHeight / 4;
                    break;
            }

            TextFontSize = IconWidth / 3;
        }

        public void ConfirmPressed()
        {
            if (!IsSelected || !IsHoved) return;

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
