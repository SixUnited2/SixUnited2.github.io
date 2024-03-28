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
    /// QuickMenuSlider.xaml 的交互逻辑
    /// </summary>
    public partial class QuickMenuSlider : UserControl, IQuickMenuControl, ISliderPageListItem
    {
        private ItemEffect _itemEffect;

        public delegate void QuickMenuSliderValueChangedHandler(IQuickMenuControl sender, int value);
        public delegate void QuickMenuSliderClickHandler(IQuickMenuControl sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
        static SolidColorBrush SELECTED_BRUSH = new SolidColorBrush(Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF));

        static Thickness DEFAULT_ICON_MARGIN = new Thickness(8, 8, 0, 8);
        static Thickness DEFAULT_TEXT_MARGIN = new Thickness(20, 16, 0, 16);

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event QuickMenuSliderValueChangedHandler OnSliderListItemValueChanged;
        public event QuickMenuSliderClickHandler OnClick;

        public QuickMenuSlider()
        {
            InitializeComponent();

            DataContext = this;
            Loaded += QuickMenuSlider_Loaded;

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

        private void QuickMenuSlider_Loaded(object sender, RoutedEventArgs e)
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
            DependencyProperty.Register("Text", typeof(string), typeof(QuickMenuSlider), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(QuickMenuSlider), new PropertyMetadata(DEFUALT_ICON_PATH));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                if (value <= MaxValue && value >= MinValue)
                {
                    SetValue(ValueProperty, value);
                    ValueText = string.IsNullOrEmpty(TextFormat) ? value.ToString() : string.Format(TextFormat, value.ToString());
                }
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(QuickMenuSlider), new PropertyMetadata(0));

        public string ValueText
        {
            get { return (string)GetValue(ValueTextProperty); }
            set
            {
                SetValue(ValueTextProperty, value);
            }
        }

        public static readonly DependencyProperty ValueTextProperty =
            DependencyProperty.Register("ValueText", typeof(string), typeof(QuickMenuSlider), new PropertyMetadata(string.Empty));


        public string TextFormat
        {
            get { return (string)GetValue(TextFormatProperty); }
            set
            {
                SetValue(TextFormatProperty, value);
            }
        }

        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register("TextFormat", typeof(string), typeof(QuickMenuSlider), new PropertyMetadata(string.Empty));


        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(QuickMenuSlider), new PropertyMetadata(0));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(int), typeof(QuickMenuSlider), new PropertyMetadata(0));


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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(QuickMenuSlider), new PropertyMetadata(false));

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
                }
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(QuickMenuSlider), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(QuickMenuSlider), new PropertyMetadata(0));


        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

        public static readonly DependencyProperty IsSelectableProperty =
            DependencyProperty.Register("IsSelectable", typeof(bool), typeof(QuickMenuSlider), new PropertyMetadata(true));

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set
            {
                SetValue(IconMarginProperty, value);
            }
        }

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(QuickMenuSlider), new PropertyMetadata(DEFAULT_ICON_MARGIN));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set
            {
                SetValue(TextMarginProperty, value);
            }
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(QuickMenuSlider), new PropertyMetadata(DEFAULT_TEXT_MARGIN));

        public int BeforValue { get; set; }

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
            //LoopValue();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            //if (e.Key == Key.Enter)
            //{
            //    LoopValue();
            //}
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
            if (!IsSelected || !IsHoved) return;

            if (!IsSelected)
            {
                BeforValue = Value;
                IsSelected = true;
            }
            //else
            //{
            //    LoopValue();
            //}
        }

        public void RollbackValue()
        {
            Value = BeforValue;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Value = Convert.ToInt32(e.NewValue);
            OnSliderListItemValueChanged?.Invoke(this, Convert.ToInt32(e.NewValue));
        }

        private void GridOnTouchHandler(Point point, double width, double height)
        {
            var per = point.X / width;
            Value = Convert.ToInt32(MaxValue * per);
        }

        private void LoopValue()
        {
            var value = 0;

            if (Value < MaxValue)
            {
                value = Value + 1;
            }
            else
            {
                value = MinValue;
            }

            Value = value;
            OnSliderListItemValueChanged?.Invoke(this, value);
        }
    }
}
