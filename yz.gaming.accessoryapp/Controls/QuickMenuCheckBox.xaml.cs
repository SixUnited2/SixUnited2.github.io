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
    /// QuickMenuCheckBox.xaml 的交互逻辑
    /// </summary>
    public partial class QuickMenuCheckBox : UserControl, IQuickMenuControl
    {
        private ItemEffect _itemEffect;

        public delegate void QuickMenuCheckBoxCheckedStateChangedHandler(IQuickMenuControl sender, bool isChecked);
        public delegate void QuickMenuCheckBoxClickHandler(IQuickMenuControl sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
        static SolidColorBrush SELECTED_BRUSH = new SolidColorBrush(Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF));

        static Thickness DEFAULT_ICON_MARGIN = new Thickness(8, 8, 0, 8);
        static Thickness DEFAULT_TEXT_MARGIN = new Thickness(20, 16, 0, 16);

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event QuickMenuCheckBoxCheckedStateChangedHandler OnCheckedStateChanged;
        public event QuickMenuCheckBoxClickHandler OnClick;

        public QuickMenuCheckBox()
        {
            InitializeComponent();

            DataContext = this;
            Loaded += QuickMenuCheckBox_Loaded;

            _itemEffect = new ItemEffect()
            {
                Border = Border,
                MainGrid = MainGrid,
                DefaultBackgroundBrush = DEFAULT_BRUSH,
                SelectedBackgroundBrush = SELECTED_BRUSH,
                SelectedBorderBrush = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00)),
                SelectedBorderThickness = new Thickness(2 / SystemUtils.Instance.GetScreenScalingFactor())
            };
        }

        private void QuickMenuCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            double scaling = SystemUtils.Instance.GetScreenScalingFactor();

            if (scaling != 1)
            {
                IconMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_ICON_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_ICON_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_ICON_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_ICON_MARGIN.Bottom / scaling))
                };
                TextMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_TEXT_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_TEXT_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_TEXT_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_TEXT_MARGIN.Bottom / scaling))
                };
            }
            else
            {
                IconMargin = DEFAULT_ICON_MARGIN;
                TextMargin = DEFAULT_TEXT_MARGIN;
            }

            SetButtonEffect(IsSelected, IsHoved);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(QuickMenuCheckBox), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(QuickMenuCheckBox), new PropertyMetadata(DEFUALT_ICON_PATH));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set
            {
                SetValue(IsCheckedProperty, value);
            }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(QuickMenuCheckBox), new PropertyMetadata(false));

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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(QuickMenuCheckBox), new PropertyMetadata(false));

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
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(QuickMenuCheckBox), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(QuickMenuCheckBox), new PropertyMetadata(0));


        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

        public static readonly DependencyProperty IsSelectableProperty =
            DependencyProperty.Register("IsSelectable", typeof(bool), typeof(QuickMenuCheckBox), new PropertyMetadata(true));

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set
            {
                SetValue(IconMarginProperty, value);
            }
        }

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(QuickMenuCheckBox), new PropertyMetadata(DEFAULT_ICON_MARGIN));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set
            {
                SetValue(TextMarginProperty, value);
            }
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(QuickMenuCheckBox), new PropertyMetadata(DEFAULT_TEXT_MARGIN));

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            IsHoved = false;
            OnClick?.Invoke(this);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            IsSelected = true;
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            IsHoved = false;
            OnClick?.Invoke(this);
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);

            IsHoved = true;
        }

        protected override void OnTouchEnter(TouchEventArgs e)
        {
            base.OnTouchEnter(e);

            IsSelected = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsHoved = true;
            IsChecked = !IsChecked;
            OnCheckedStateChanged?.Invoke(this, IsChecked);
        }

        public void SetButtonEffect(bool isSelected)
        {
            _itemEffect.SetButtonEffect(isSelected, IsHoved);
        }

        public void SetButtonEffect(bool isSelected, bool isHover)
        {
            _itemEffect.SetButtonEffect(isSelected, isHover);
        }

        public void ConfirmPressed()
        {
            if (!IsHoved) return;

            if (!IsSelected)
            {
                IsSelected = true;
            }
            else
            {
                IsChecked = !IsChecked;
                OnCheckedStateChanged?.Invoke(this, IsChecked);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked = true;
            OnCheckedStateChanged?.Invoke(this, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked = false;
            OnCheckedStateChanged?.Invoke(this, false);
        }
    }
}
