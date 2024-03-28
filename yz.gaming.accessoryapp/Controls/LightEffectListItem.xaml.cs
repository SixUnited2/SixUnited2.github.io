using System;
using System.Collections.Generic;
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
using yz.gaming.accessoryapp.Utils;
using static yz.gaming.accessoryapp.Controls.ItemEffect;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// LightEffectListItem.xaml 的交互逻辑
    /// </summary>
    public partial class LightEffectListItem : UserControl, IArrayPageListItem
    {
        private ItemEffect _itemEffect;

        public delegate void LightEffectListItemValueChangedHandler(IPageListItem sender, int index);
        public delegate void LightEffectListItemClickHandler(IPageListItem sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";
        static SolidColorBrush DEFAULT_BACKGROUND_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x25, 0x25, 0x25));
        static SolidColorBrush SELECTED_BACKGROUND_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x58, 0x58, 0x58));

        static Thickness DEFAULT_EFFECT_MARGIN = new Thickness(0, 8, 30, 8);

        const int DEFAULT_CORNER_RADIUS = 14;
        const int DEFAULT_BORDER_WIDTH = 2;

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event LightEffectListItemValueChangedHandler OnValueChangedHandler;
        public event LightEffectListItemClickHandler OnClick;

        public LightEffectListItem()
        {
            InitializeComponent();

            DataContext = this;
            this.Loaded += LightEffectListItem_Loaded;

            ListItems = new List<IPageListItem>()
            {
                RgbWave,
                RgbLoop,
                RgbBreathe,
                SingleOn,
                SingleBreathe
            };

            RgbWave.OnSelectedStateChange += OnButtonSelectedStateChange;
            RgbLoop.OnSelectedStateChange += OnButtonSelectedStateChange;
            RgbBreathe.OnSelectedStateChange += OnButtonSelectedStateChange;
            SingleOn.OnSelectedStateChange += OnButtonSelectedStateChange;
            SingleBreathe.OnSelectedStateChange += OnButtonSelectedStateChange;

            _itemEffect = new ItemEffect() { Border = Border, MainGrid = MainGrid };
            _itemEffect.DefaultBackgroundBrush = DEFAULT_BACKGROUND_BRUSH;
            _itemEffect.SelectedBackgroundBrush = SELECTED_BACKGROUND_BRUSH;
        }

        List<IPageListItem> ListItems { get; set; }

        public double ButtonContainerHeight
        {
            get { return (double)GetValue(ButtonContainerhProperty); }
            set { SetValue(ButtonContainerhProperty, value); }
        }


        public static readonly DependencyProperty ButtonContainerhProperty =
            DependencyProperty.Register("ButtonContainerHeight", typeof(double), typeof(LightEffectListItem), new PropertyMetadata(0d));

        public double ButtonWidth
        {
            get { return (double)GetValue(ButtonWidthProperty); }
            set { SetValue(ButtonWidthProperty, value); }
        }


        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register("ButtonWidth", typeof(double), typeof(LightEffectListItem), new PropertyMetadata(0d));

        public double ButtonHeight
        {
            get { return (double)GetValue(ButtonHeightProperty); }
            set
            {
                SetValue(ButtonHeightProperty, value);
                ButtonContainerHeight = value - 18;
            }
        }

        public static readonly DependencyProperty ButtonHeightProperty =
            DependencyProperty.Register("ButtonHeight", typeof(double), typeof(LightEffectListItem), new PropertyMetadata(0d));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LightEffectListItem), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(LightEffectListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(LightEffectListItem), new PropertyMetadata(false));

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
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(LightEffectListItem), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);

            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(LightEffectListItem), new PropertyMetadata(0));

        public int SelectIndex
        {
            get { return (int)GetValue(SelectIndexProperty); }
            set
            {
                SetValue(SelectIndexProperty, value);

                if (!ListItems[value].IsSelected)
                {
                    Task.Run(() =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ListItems[value].IsSelected = true;
                            OnValueChangedHandler?.Invoke(this, value);
                        }));
                    });
                }
            }
        }

        public static readonly DependencyProperty SelectIndexProperty =
            DependencyProperty.Register("SelectIndex", typeof(int), typeof(LightEffectListItem), new PropertyMetadata(0));

        public int CornerRadius
        {
            get { return (int)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadiusProperty", typeof(int), typeof(LightEffectListItem), new PropertyMetadata(DEFAULT_CORNER_RADIUS));

        public int BorderWidth
        {
            get { return (int)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(int), typeof(LightEffectListItem), new PropertyMetadata(DEFAULT_BORDER_WIDTH));

        public Thickness EffectMargin
        {
            get { return (Thickness)GetValue(EffectMarginProperty); }
            set
            {
                SetValue(EffectMarginProperty, value);
            }
        }

        public static readonly DependencyProperty EffectMarginProperty =
            DependencyProperty.Register("EffectMargin", typeof(Thickness), typeof(LightEffectListItem), new PropertyMetadata(DEFAULT_EFFECT_MARGIN));


        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            IsHoved = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsSelected = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                OnClick?.Invoke(this);
            }
        }

        private void LightEffectListItem_Loaded(object sender, RoutedEventArgs e)
        {
            double scaling = SystemUtils.Instance.GetScreenScalingFactor();

            if (scaling != 1)
            {
                EffectMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_EFFECT_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_EFFECT_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_EFFECT_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_EFFECT_MARGIN.Bottom / scaling))
                };

            }

            CornerRadius = Convert.ToInt32(Math.Round(DEFAULT_CORNER_RADIUS / scaling));
            BorderWidth = Convert.ToInt32(Math.Round(DEFAULT_BORDER_WIDTH / scaling));

            SetButtonEffect(IsSelected, IsHoved);
            RgbWave.BorderCornerRadius = CornerRadius;
            RgbLoop.BorderCornerRadius = CornerRadius;
            RgbBreathe.BorderCornerRadius = CornerRadius;
            SingleOn.BorderCornerRadius = CornerRadius;
            SingleBreathe.BorderCornerRadius = CornerRadius;

            RgbWave.BorderWidth = BorderWidth;
            RgbLoop.BorderWidth = BorderWidth;
            RgbBreathe.BorderWidth = BorderWidth;
            SingleOn.BorderWidth = BorderWidth;
            SingleBreathe.BorderWidth = BorderWidth;
        }

        public void SetButtonEffect(bool isSelected, bool isHoved)
        {
            _itemEffect.SetButtonEffect(isSelected, isHoved);
        }

        public void ConfirmPressed()
        {
            if (!IsSelected || !IsHoved) return;
            if (!IsSelected)
            {
                IsSelected = true;
            }
            else
            {
                LoopValue();
            }
        }

        private void OnButtonSelectedStateChange(IPageListItem sender, bool isSelected)
        {
            if (isSelected)
            {
                ListItems.ForEach(p =>
                {
                    if (!p.Equals(sender))
                    {
                        p.IsSelected = false;
                    }
                    else
                    {
                        SelectIndex = p.Index;
                        Task.Run(() =>
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                OnValueChangedHandler?.Invoke(this, p.Index);
                            }));
                        });
                    }
                });
            }
        }

        private void LoopValue()
        {
            var value = 0;

            if (SelectIndex < ListItems.Count - 1)
            {
                value = SelectIndex + 1;
            }

            SelectIndex = value;
        }

        public void SelectNext()
        {
            if (SelectIndex < ListItems.Count - 1)
            {
                SelectIndex = SelectIndex + 1;
            }
        }

        public void SelectPrev()
        {
            if (SelectIndex > 0)
            {
                SelectIndex = SelectIndex - 1;
            }
        }
    }
}
