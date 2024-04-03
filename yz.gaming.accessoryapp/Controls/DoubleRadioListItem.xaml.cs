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
using static yz.gaming.accessoryapp.Controls.DoubleRadioControl;
using static yz.gaming.accessoryapp.Controls.ItemEffect;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// DoubleRadioListItemControl.xaml 的交互逻辑
    /// </summary>
    public partial class DoubleRadioListItem : UserControl, IPageListItem
    {
        private ItemEffect _itemEffect;

        public delegate void DoubleRadioListItemSelectedElementChangedHandler(IPageListItem sender, object value);
        public delegate void DoubleRadioListItemClickHandler(IPageListItem sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";
        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event DoubleRadioListItemSelectedElementChangedHandler OnSelectedElementChanged;
        public event DoubleRadioListItemClickHandler OnClick;

        public DoubleRadioListItem()
        {
            InitializeComponent();

            DataContext = this;
            this.Loaded += DoubleRadioListItem_Loaded;

            _itemEffect = new ItemEffect() { Border = Border, MainGrid = MainGrid };
        }

        private void DoubleRadioListItem_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonEffect(IsSelected, IsHoved);
            DoubleRadio.LeftIconPath = LeftIconPath;
            DoubleRadio.RightIconPath = RightIconPath;
            DoubleRadio.LeftText = LeftText;
            DoubleRadio.RightText = RightText;
            DoubleRadio.LeftElement = LeftElement;
            DoubleRadio.RightElement = RightElement;
            DoubleRadio.SelectElement = SelectElement;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DoubleRadioListItem), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(DoubleRadioListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DoubleRadioListItem), new PropertyMetadata(false));

        public bool IsHoved
        {
            get { return (bool)GetValue(IsHovedProperty); }
            set
            {
                SetValue(IsHovedProperty, value);
                SetButtonEffect(IsSelected, value);
                OnHovedStateChange?.Invoke(this, value);
                IsSelected = value;
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(DoubleRadioListItem), new PropertyMetadata(false));


        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(DoubleRadioListItem), new PropertyMetadata(0));

        public string LeftIconPath
        {
            get { return (string)GetValue(LeftIconPathProperty); }
            set
            {
                SetValue(LeftIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty LeftIconPathProperty =
            DependencyProperty.Register("LeftIconPath", typeof(string), typeof(DoubleRadioListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string RightIconPath
        {
            get { return (string)GetValue(RightIconPathProperty); }
            set
            {
                SetValue(RightIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty RightIconPathProperty =
            DependencyProperty.Register("RightIconPath", typeof(string), typeof(DoubleRadioListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string LeftText
        {
            get { return (string)GetValue(LeftTextProperty); }
            set
            {
                SetValue(LeftTextProperty, value);
            }
        }

        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register("LeftText", typeof(string), typeof(DoubleRadioListItem), new PropertyMetadata(string.Empty));

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set
            {
                SetValue(RightTextProperty, value);
            }
        }

        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register("RightText", typeof(string), typeof(DoubleRadioListItem), new PropertyMetadata(string.Empty));

        public object LeftElement
        {
            get { return GetValue(LeftElementProperty); }
            set
            {
                SetValue(LeftElementProperty, value);
            }
        }

        public static readonly DependencyProperty LeftElementProperty =
            DependencyProperty.Register("LeftElement", typeof(object), typeof(DoubleRadioListItem), new PropertyMetadata(null));

        public object RightElement
        {
            get { return GetValue(RightElementProperty); }
            set
            {
                SetValue(RightElementProperty, value);
            }
        }

        public static readonly DependencyProperty RightElementProperty =
            DependencyProperty.Register("RightElement", typeof(object), typeof(DoubleRadioListItem), new PropertyMetadata(null));

        public object SelectElement
        {
            get { return (object)GetValue(SelectElementProperty); }
            set
            {
                //System.Diagnostics.Debug.WriteLine($"Trace : DoubleRadioListItem [{Name}] 's SelectElement change to [{value}]");
                SetValue(SelectElementProperty, value);
                DoubleRadio.SelectElement = value;
            }
        }

        public static readonly DependencyProperty SelectElementProperty =
            DependencyProperty.Register("SelectElement", typeof(object), typeof(DoubleRadioListItem), new PropertyMetadata(null));

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
                if (SelectElement.Equals(LeftElement))
                {
                    SelectElement = RightElement;
                }
                else
                {
                    SelectElement = LeftElement;
                }

                DoubleRadio.SelectElement = SelectElement;
            }
        }

        private void DoubleRadio_OnSelectedElementChanged(DoubleRadioControl sender, object element)
        {
            SelectElement = element;
            OnSelectedElementChanged?.Invoke(this, element);
        }
    }
}
