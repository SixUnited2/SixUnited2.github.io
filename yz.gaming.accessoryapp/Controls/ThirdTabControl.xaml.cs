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
    /// ThirdRadioControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThirdTabControl : UserControl
    {
        public delegate void ThirdTabControlSelectedElementChangedHandler(ThirdTabControl sender, object element);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        public event ThirdTabControlSelectedElementChangedHandler OnSelectedElementChanged;

        public enum SelectElementEnum
        {
            LeftElement,
            CenterElement,
            RightElement,
            M1M2Element
        }

        public ThirdTabControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string LeftIconPath
        {
            get { return (string)GetValue(LeftIconPathProperty); }
            set
            {
                SetValue(LeftIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty LeftIconPathProperty =
            DependencyProperty.Register("LeftIconPath", typeof(string), typeof(ThirdTabControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string CenterIconPath
        {
            get { return (string)GetValue(CenterIconPathProperty); }
            set
            {
                SetValue(CenterIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty CenterIconPathProperty =
            DependencyProperty.Register("CenterIconPath", typeof(string), typeof(ThirdTabControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string RightIconPath
        {
            get { return (string)GetValue(RightIconPathProperty); }
            set
            {
                SetValue(RightIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty RightIconPathProperty =
            DependencyProperty.Register("RightIconPath", typeof(string), typeof(ThirdTabControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string M1M2IconPath
        {
            get { return (string)GetValue(M1M2IconPathProperty); }
            set
            {
                SetValue(M1M2IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty M1M2IconPathProperty =
            DependencyProperty.Register("M1M2IconPath", typeof(string), typeof(ThirdTabControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string LeftText
        {
            get { return (string)GetValue(LeftTextProperty); }
            set
            {
                SetValue(LeftTextProperty, value);
            }
        }

        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register("LeftText", typeof(string), typeof(ThirdTabControl), new PropertyMetadata(string.Empty));

        public string CenterText
        {
            get { return (string)GetValue(CenterTextProperty); }
            set
            {
                SetValue(CenterTextProperty, value);
            }
        }

        public static readonly DependencyProperty CenterTextProperty =
            DependencyProperty.Register("CenterText", typeof(string), typeof(ThirdTabControl), new PropertyMetadata(string.Empty));

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set
            {
                SetValue(RightTextProperty, value);
            }
        }

        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register("RightText", typeof(string), typeof(ThirdTabControl), new PropertyMetadata(string.Empty));

        public string M1M2Text
        {
            get { return (string)GetValue(M1M2TextProperty); }
            set
            {
                SetValue(M1M2TextProperty, value);
            }
        }

        public static readonly DependencyProperty M1M2TextProperty =
            DependencyProperty.Register("M1M2Text", typeof(string), typeof(ThirdTabControl), new PropertyMetadata(string.Empty));
        public object LeftElement
        {
            get { return GetValue(LeftElementProperty); }
            set
            {
                SetValue(LeftElementProperty, value);
            }
        }

        public static readonly DependencyProperty LeftElementProperty =
            DependencyProperty.Register("LeftElement", typeof(object), typeof(ThirdTabControl), new PropertyMetadata(null));

        public object CenterElement
        {
            get { return GetValue(CenterElementProperty); }
            set
            {
                SetValue(CenterElementProperty, value);
            }
        }

        public static readonly DependencyProperty CenterElementProperty =
            DependencyProperty.Register("CenterElement", typeof(object), typeof(ThirdTabControl), new PropertyMetadata(null));

        public object RightElement
        {
            get { return GetValue(RightElementProperty); }
            set
            {
                SetValue(RightElementProperty, value);
            }
        }

        public static readonly DependencyProperty RightElementProperty =
            DependencyProperty.Register("RightElement", typeof(object), typeof(ThirdTabControl), new PropertyMetadata(null));

        public object M1M2Element
        {
            get { return GetValue(M1M2ElementProperty); }
            set
            {
                SetValue(M1M2ElementProperty, value);
            }
        }

        public static readonly DependencyProperty M1M2ElementProperty =
            DependencyProperty.Register("M1M2Element", typeof(object), typeof(ThirdTabControl), new PropertyMetadata(null));
        public SelectElementEnum SelectElement
        {
            get { return (SelectElementEnum)GetValue(SelectElementProperty); }
            set
            {
                SetValue(SelectElementProperty, value);
                SetStyle(value);
            }
        }

        public static readonly DependencyProperty SelectElementProperty =
            DependencyProperty.Register("SelectElement", typeof(SelectElementEnum), typeof(ThirdTabControl), new PropertyMetadata(SelectElementEnum.LeftElement));

        public int LeftZIndex
        {
            get { return (int)GetValue(LeftZIndexProperty); }
            set
            {
                SetValue(LeftZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty LeftZIndexProperty =
            DependencyProperty.Register("LeftZIndex", typeof(int), typeof(ThirdTabControl), new PropertyMetadata(0));

        public int CenterZIndex
        {
            get { return (int)GetValue(CenterZIndexProperty); }
            set
            {
                SetValue(CenterZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty CenterZIndexProperty =
            DependencyProperty.Register("CenterZIndex", typeof(int), typeof(ThirdTabControl), new PropertyMetadata(0));

        public int RightZIndex
        {
            get { return (int)GetValue(RightZIndexProperty); }
            set
            {
                SetValue(RightZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty RightZIndexProperty =
            DependencyProperty.Register("RightZIndex", typeof(int), typeof(ThirdTabControl), new PropertyMetadata(1));

        public int M1M2ZIndex
        {
            get { return (int)GetValue(M1M2ZIndexProperty); }
            set
            {
                SetValue(M1M2ZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty M1M2ZIndexProperty =
            DependencyProperty.Register("M1M2Index", typeof(int), typeof(ThirdTabControl), new PropertyMetadata(1));
        private void Left_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectElement != SelectElementEnum.LeftElement)
            {
                SelectElement = SelectElementEnum.LeftElement;
            }
        }

        private void Center_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectElement != SelectElementEnum.CenterElement)
            {
                SelectElement = SelectElementEnum.CenterElement;
            }
        }

        private void Right_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectElement != SelectElementEnum.RightElement)
            {
                SelectElement = SelectElementEnum.RightElement;
            }
        }

        private void M1M2_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectElement != SelectElementEnum.M1M2Element)
            {
                SelectElement = SelectElementEnum.M1M2Element;
            }
        }

        private void SetStyle(SelectElementEnum selectElement)
        {
            switch (selectElement)
            {
                case SelectElementEnum.LeftElement:
                    if (!Left.IsChecked.HasValue || !Left.IsChecked.Value) Left.IsChecked = true;
                    LeftZIndex = 1;
                    CenterZIndex = 0;
                    RightZIndex = 0;
                    M1M2ZIndex = 0;
                    OnSelectedElementChanged?.Invoke(this, LeftElement);
                    break;
                case SelectElementEnum.CenterElement:
                    if (!Center.IsChecked.HasValue || !Center.IsChecked.Value) Center.IsChecked = true;
                    LeftZIndex = 0;
                    CenterZIndex = 1;
                    RightZIndex = 0;
                    M1M2ZIndex = 0;
                    OnSelectedElementChanged?.Invoke(this, CenterElement);
                    break;
                case SelectElementEnum.RightElement:
                    if (!Right.IsChecked.HasValue || !Right.IsChecked.Value) Right.IsChecked = true;
                    LeftZIndex = 0;
                    CenterZIndex = 0;
                    RightZIndex = 1;
                    M1M2ZIndex = 0;
                    OnSelectedElementChanged?.Invoke(this, RightElement);
                    break;
                case SelectElementEnum.M1M2Element:
                    if (!M1M2.IsChecked.HasValue || !M1M2.IsChecked.Value) M1M2.IsChecked = true;
                    LeftZIndex = 0;
                    CenterZIndex = 0;
                    RightZIndex = 0;
                    M1M2ZIndex = 1;
                    OnSelectedElementChanged?.Invoke(this, M1M2Element);
                    break;
            }
        }
    }
}
