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
using static yz.gaming.accessoryapp.Controls.ThirdRadioControl;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// ThirdRadioListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ThirdRadioListItem : UserControl, IArrayPageListItem
    {
        private ItemEffect _itemEffect;

        public delegate void ThirdRadioListItemSelectElementChangedHandler(IPageListItem sender, object element);
        public delegate void ThirdRadioListItemClickHandler(IPageListItem sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event ThirdRadioListItemSelectElementChangedHandler OnSelectElementChanged;
        public event ThirdRadioListItemClickHandler OnClick;

        public ThirdRadioListItem()
        {
            InitializeComponent();

            DataContext = this;
            this.Loaded += ThirdRadioListItem_Loaded;

            _itemEffect = new ItemEffect() { Border = Border, MainGrid = MainGrid };
            ThirdRadio.TextMarginDefault = new Thickness(64, 12, 0, 12);
        }

        private void ThirdRadioListItem_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonEffect(IsSelected, IsHoved);
            ThirdRadio.LeftIconPath = LeftIconPath;
            ThirdRadio.CenterIconPath = CenterIconPath;
            ThirdRadio.RightIconPath = RightIconPath;
            ThirdRadio.LeftText = LeftText;
            ThirdRadio.CenterText = CenterText;
            ThirdRadio.RightText = RightText;
            ThirdRadio.LeftElement = LeftElement;
            ThirdRadio.RightElement = RightElement;
            ThirdRadio.SelectElement = SelectElement;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ThirdRadioListItem), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(ThirdRadioListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ThirdRadioListItem), new PropertyMetadata(false));

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
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(ThirdRadioListItem), new PropertyMetadata(false));


        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(ThirdRadioListItem), new PropertyMetadata(0));

        public string LeftIconPath
        {
            get { return (string)GetValue(LeftIconPathProperty); }
            set
            {
                SetValue(LeftIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty LeftIconPathProperty =
            DependencyProperty.Register("LeftIconPath", typeof(string), typeof(ThirdRadioListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string CenterIconPath
        {
            get { return (string)GetValue(CenterIconPathProperty); }
            set
            {
                SetValue(CenterIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty CenterIconPathProperty =
            DependencyProperty.Register("CenterIconPath", typeof(string), typeof(ThirdRadioListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string RightIconPath
        {
            get { return (string)GetValue(RightIconPathProperty); }
            set
            {
                SetValue(RightIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty RightIconPathProperty =
            DependencyProperty.Register("RightIconPath", typeof(string), typeof(ThirdRadioListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string LeftText
        {
            get { return (string)GetValue(LeftTextProperty); }
            set
            {
                SetValue(LeftTextProperty, value);
            }
        }

        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register("LeftText", typeof(string), typeof(ThirdRadioListItem), new PropertyMetadata(string.Empty));

        public string CenterText
        {
            get { return (string)GetValue(CenterTextProperty); }
            set
            {
                SetValue(CenterTextProperty, value);
            }
        }

        public static readonly DependencyProperty CenterTextProperty =
            DependencyProperty.Register("CenterText", typeof(string), typeof(ThirdRadioListItem), new PropertyMetadata(string.Empty));

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set
            {
                SetValue(RightTextProperty, value);
            }
        }

        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register("RightText", typeof(string), typeof(ThirdRadioListItem), new PropertyMetadata(string.Empty));

        public object LeftElement
        {
            get { return GetValue(LeftElementProperty); }
            set
            {
                SetValue(LeftElementProperty, value);
                ThirdRadio.LeftElement = LeftElement;
            }
        }

        public static readonly DependencyProperty LeftElementProperty =
            DependencyProperty.Register("LeftElement", typeof(object), typeof(ThirdRadioListItem), new PropertyMetadata(null));

        public object CenterElement
        {
            get { return (object)GetValue(CenterElementProperty); }
            set
            {
                SetValue(CenterElementProperty, value);
                ThirdRadio.CenterElement = CenterElement;
            }
        }

        public static readonly DependencyProperty CenterElementProperty =
            DependencyProperty.Register("CenterElement", typeof(object), typeof(ThirdRadioListItem), new PropertyMetadata(null));


        public object RightElement
        {
            get { return GetValue(RightElementProperty); }
            set
            {
                SetValue(RightElementProperty, value);
                ThirdRadio.RightElement = RightElement;
            }
        }

        public static readonly DependencyProperty RightElementProperty =
            DependencyProperty.Register("RightElement", typeof(object), typeof(ThirdRadioListItem), new PropertyMetadata(null));

        public object SelectElement
        {
            get { return (object)GetValue(SelectElementProperty); }
            set
            {
                SetValue(SelectElementProperty, value);

                if (value.Equals(LeftElement))
                {
                    ThirdRadio.SelectElement = LeftElement;
                }
                else if (value.Equals(CenterElement))
                {
                    ThirdRadio.SelectElement = CenterElement;
                }
                else if (value.Equals(RightElement))
                {
                    ThirdRadio.SelectElement = RightElement;
                }
            }
        }

        public static readonly DependencyProperty SelectElementProperty =
            DependencyProperty.Register("SelectElement", typeof(object), typeof(ThirdRadioListItem), new PropertyMetadata(null));

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
            OnSelectElementChanged?.Invoke(this, element);
        }
    }
}
