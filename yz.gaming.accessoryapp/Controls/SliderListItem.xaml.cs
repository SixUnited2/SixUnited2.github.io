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
using static yz.gaming.accessoryapp.Controls.ItemEffect;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// SliderListItem.xaml 的交互逻辑
    /// </summary>
    public partial class SliderListItem : UserControl, ISliderPageListItem
    {
        private ItemEffect _itemEffect;

        public delegate void SliderListItemValueChangedHandler(IPageListItem sender, int value);
        public delegate void SliderListItemClickHandler(IPageListItem sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event SliderListItemValueChangedHandler OnSliderListItemValueChanged;
        public event SliderListItemClickHandler OnClick;

        public SliderListItem()
        {
            InitializeComponent();

            DataContext = this;
            this.Loaded += SliderListItem_Loaded;

            _itemEffect = new ItemEffect()
            {
                Border = Border,
                MainGrid = MainGrid,
                DefaultBorderThickness = new Thickness(2),
                SelectedBorderThickness = new Thickness(0)
            };
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SliderListItem), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(SliderListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

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
            DependencyProperty.Register("Value", typeof(int), typeof(SliderListItem), new PropertyMetadata(0));

        public string ValueText
        {
            get { return (string)GetValue(ValueTextProperty); }
            set
            {
                SetValue(ValueTextProperty, value);
            }
        }

        public static readonly DependencyProperty ValueTextProperty =
            DependencyProperty.Register("ValueText", typeof(string), typeof(SliderListItem), new PropertyMetadata(string.Empty));


        public string TextFormat
        {
            get { return (string)GetValue(TextFormatProperty); }
            set
            {
                SetValue(TextFormatProperty, value);
            }
        }

        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register("TextFormat", typeof(string), typeof(SliderListItem), new PropertyMetadata(string.Empty));


        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(SliderListItem), new PropertyMetadata(0));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(int), typeof(SliderListItem), new PropertyMetadata(0));


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                SetButtonEffect(value, IsHoved);
                OnSelectedStateChange?.Invoke(this, value);
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SliderListItem), new PropertyMetadata(false));

        public bool IsHoved
        {
            get { return (bool)GetValue(IsHovedProperty); }
            set
            {
                SetValue(IsHovedProperty, value);
                SetButtonEffect(IsSelected, value);
                OnHovedStateChange?.Invoke(this, value);
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(SliderListItem), new PropertyMetadata(false));


        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(SliderListItem), new PropertyMetadata(0));

        public int BeforValue { get; set; }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            IsHoved = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!IsSelected)
            {
                IsSelected = true;
            }
            //else
            //{
            //    LoopValue();
            //}
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                if (!IsSelected)
                {
                    IsSelected = true;
                }
                //else
                //{
                //    LoopValue();
                //}
            }
        }

        private void SliderListItem_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonEffect(IsSelected, IsHoved);
        }

        public void SetButtonEffect(bool isSelected, bool isHoved)
        {
            _itemEffect.SetButtonEffect(isSelected, isHoved);
        }

        public void ConfirmPressed()
        {
            if (!IsSelected || !IsHoved) return;
            if (!IsSelected) IsSelected = true;
            if (IsSelected) BeforValue = Value;
            //else
            //{
            //    LoopValue();
            //}
        }

        public void RollbackValue()
        {
            Value = BeforValue;
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Value = Convert.ToInt32(e.NewValue);
            OnSliderListItemValueChanged?.Invoke(this, Convert.ToInt32(e.NewValue));
        }

        private void LoopValue()
        {
            var value = 0;

            if (Value < MaxValue)
            {
                value = Value + 1;
            }

            Value = value;
            OnSliderListItemValueChanged?.Invoke(this, value);
        }
    }
}
