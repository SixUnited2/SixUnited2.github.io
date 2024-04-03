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
    /// QuickMenuThirdRadio.xaml 的交互逻辑
    /// </summary>
    public partial class QuickMenuThirdRadio : UserControl, IQuickMenuControl
    {
        private ItemEffect _itemEffect;

        public delegate void QuickMenuThirdRadioSelectedElementChangedHandler(IQuickMenuControl sender, object value);
        public delegate void QuickMenuThirdRadioClickHandler(IQuickMenuControl sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
        static SolidColorBrush SELECTED_BRUSH = new SolidColorBrush(Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF));

        static Thickness DEFAULT_ICON_MARGIN = new Thickness(8, 8, 0, 8);
        static Thickness DEFAULT_TEXT_MARGIN = new Thickness(20, 16, 0, 16);
        static Thickness DEFAULT_RADIO_MARGIN = new Thickness(0, 8, 0, 8);

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event QuickMenuThirdRadioSelectedElementChangedHandler OnSelectedElementChanged;
        public event QuickMenuThirdRadioClickHandler OnClick;

        public QuickMenuThirdRadio()
        {
            InitializeComponent();

            DataContext = this;
            this.Loaded += QuickMenuThirdRadio_Loaded;

            _itemEffect = new ItemEffect()
            {
                Border = Border,
                MainGrid = MainGrid,
                DefaultBackgroundBrush = DEFAULT_BRUSH,
                SelectedBackgroundBrush = SELECTED_BRUSH,
                SelectedBorderBrush = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00)),
                SelectedBorderThickness = new Thickness(2 / SystemUtils.Instance.GetScreenScalingFactor())
            };

            ThirdRadio.TextMarginDefault = new Thickness(64, 18, 0, 18);
        }

        private void QuickMenuThirdRadio_Loaded(object sender, RoutedEventArgs e)
        {
            ThirdRadio.LeftIconPath = LeftIconPath;
            ThirdRadio.CenterIconPath = CenterIconPath;
            ThirdRadio.RightIconPath = RightIconPath;
            ThirdRadio.LeftText = LeftText;
            ThirdRadio.CenterText = CenterText;
            ThirdRadio.RightText = RightText;
            ThirdRadio.LeftElement = LeftElement;
            ThirdRadio.CenterElement = CenterElement;
            ThirdRadio.RightElement = RightElement;
            ThirdRadio.SelectElement = SelectElement;
            ThirdRadio.GroupName = GroupName;

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
                RadioMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_RADIO_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_RADIO_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_RADIO_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_RADIO_MARGIN.Bottom / scaling))
                };
            }
            else
            {
                IconMargin = DEFAULT_ICON_MARGIN;
                TextMargin = DEFAULT_TEXT_MARGIN;
                RadioMargin = DEFAULT_RADIO_MARGIN;
            }

            SetButtonEffect(IsSelected, IsHoved);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(QuickMenuThirdRadio), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(QuickMenuThirdRadio), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string LeftIconPath
        {
            get { return (string)GetValue(LeftIconPathProperty); }
            set
            {
                SetValue(LeftIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty LeftIconPathProperty =
            DependencyProperty.Register("LeftIconPath", typeof(string), typeof(QuickMenuThirdRadio), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string CenterIconPath
        {
            get { return (string)GetValue(CenterIconPathProperty); }
            set
            {
                SetValue(CenterIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty CenterIconPathProperty =
            DependencyProperty.Register("CenterIconPath", typeof(string), typeof(QuickMenuThirdRadio), new PropertyMetadata(DEFUALT_ICON_PATH));


        public string RightIconPath
        {
            get { return (string)GetValue(RightIconPathProperty); }
            set
            {
                SetValue(RightIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty RightIconPathProperty =
            DependencyProperty.Register("RightIconPath", typeof(string), typeof(QuickMenuThirdRadio), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string LeftText
        {
            get { return (string)GetValue(LeftTextProperty); }
            set
            {
                SetValue(LeftTextProperty, value);
            }
        }

        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register("LeftText", typeof(string), typeof(QuickMenuThirdRadio), new PropertyMetadata(string.Empty));

        public string CenterText
        {
            get { return (string)GetValue(CenterTextProperty); }
            set
            {
                SetValue(CenterTextProperty, value);
            }
        }

        public static readonly DependencyProperty CenterTextProperty =
            DependencyProperty.Register("CenterText", typeof(string), typeof(QuickMenuThirdRadio), new PropertyMetadata(string.Empty));

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set
            {
                SetValue(RightTextProperty, value);
            }
        }

        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register("RightText", typeof(string), typeof(QuickMenuThirdRadio), new PropertyMetadata(string.Empty));

        public object LeftElement
        {
            get { return GetValue(LeftElementProperty); }
            set
            {
                SetValue(LeftElementProperty, value);
                ThirdRadio.LeftElement = value;
            }
        }

        public static readonly DependencyProperty LeftElementProperty =
            DependencyProperty.Register("LeftElement", typeof(object), typeof(QuickMenuThirdRadio), new PropertyMetadata(null));

        public object CenterElement
        {
            get { return (object)GetValue(CenterElementProperty); }
            set
            {
                SetValue(CenterElementProperty, value);
                ThirdRadio.CenterElement = value;
            }
        }

        public static readonly DependencyProperty CenterElementProperty =
            DependencyProperty.Register("CenterElement", typeof(object), typeof(QuickMenuThirdRadio), new PropertyMetadata(null));


        public object RightElement
        {
            get { return GetValue(RightElementProperty); }
            set
            {
                SetValue(RightElementProperty, value);
                ThirdRadio.RightElement = value;
            }
        }

        public static readonly DependencyProperty RightElementProperty =
            DependencyProperty.Register("RightElement", typeof(object), typeof(QuickMenuThirdRadio), new PropertyMetadata(null));

        public object SelectElement
        {
            get { return (object)GetValue(SelectElementProperty); }
            set
            {
                SetValue(SelectElementProperty, value);
                ThirdRadio.SelectElement = value;
            }
        }

        public static readonly DependencyProperty SelectElementProperty =
            DependencyProperty.Register("SelectElement", typeof(object), typeof(QuickMenuThirdRadio), new PropertyMetadata(null));

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set
            {
                SetValue(GroupNameProperty, value);
            }
        }

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(QuickMenuThirdRadio), new PropertyMetadata(string.Empty));

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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(QuickMenuThirdRadio), new PropertyMetadata(false));

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
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(QuickMenuThirdRadio), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(QuickMenuThirdRadio), new PropertyMetadata(0));


        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

        public static readonly DependencyProperty IsSelectableProperty =
            DependencyProperty.Register("IsSelectable", typeof(bool), typeof(QuickMenuThirdRadio), new PropertyMetadata(true));

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set
            {
                SetValue(IconMarginProperty, value);
            }
        }

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(QuickMenuThirdRadio), new PropertyMetadata(DEFAULT_ICON_MARGIN));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set
            {
                SetValue(TextMarginProperty, value);
            }
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(QuickMenuThirdRadio), new PropertyMetadata(DEFAULT_TEXT_MARGIN));

        public Thickness RadioMargin
        {
            get { return (Thickness)GetValue(RadioMarginProperty); }
            set
            {
                SetValue(RadioMarginProperty, value);
            }
        }

        public static readonly DependencyProperty RadioMarginProperty =
            DependencyProperty.Register("RadioMargin", typeof(Thickness), typeof(QuickMenuThirdRadio), new PropertyMetadata(DEFAULT_TEXT_MARGIN));


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
            //ConfirmPressed();
        }

        public void SetButtonEffect(bool isSelected)
        {
            _itemEffect.SetButtonEffect(isSelected, IsHoved);
        }

        public void SetButtonEffect(bool isSelected, bool isHover)
        {
            _itemEffect.SetButtonEffect(isSelected, isHover);
        }

        public void Reload()
        {
            ThirdRadio.LeftText = LeftText;
            ThirdRadio.CenterText = CenterText;
            ThirdRadio.RightText = RightText;
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

        private void LoopValue()
        {
            if (SelectElement.Equals(LeftElement))
            {
                ThirdRadio.SelectElement = CenterElement;
            }
            else if (SelectElement.Equals(CenterElement))
            {
                ThirdRadio.SelectElement = RightElement;
            }
            else if (SelectElement.Equals(RightElement))
            {
                ThirdRadio.SelectElement = LeftElement;
            }
        }

        public void SelectNext()
        {
            if (SelectElement.Equals(LeftElement))
            {
                ThirdRadio.SelectElement = CenterElement;
            }
            else if (SelectElement.Equals(CenterElement))
            {
                ThirdRadio.SelectElement = RightElement;
            }
        }

        public void SelectPrev()
        {
            if (SelectElement.Equals(CenterElement))
            {
                ThirdRadio.SelectElement = LeftElement;
            }
            else if (SelectElement.Equals(RightElement))
            {
                ThirdRadio.SelectElement = CenterElement;
            }
        }

        private void ThirdRadio_OnSelectedElementChanged(ThirdRadioControl sender, object element)
        {
            SelectElement = element;
            OnSelectedElementChanged?.Invoke(this, element);
        }
    }
}
