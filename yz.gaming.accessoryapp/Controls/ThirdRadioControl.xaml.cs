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

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// ThirdRadioControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThirdRadioControl : UserControl
    {
        public delegate void ThirdRadioControlSelectedElementChangedHandler(ThirdRadioControl sender, object element);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";
        const string GROUP_NAME = "THIRD_CONTROL";

        static Thickness DEFAULT_TEXT_MARGIN = new Thickness(64, 18, 0, 18);
        static Thickness DEFAULT_OFFSET_MARGIN = new Thickness(0, 0, -16, 0);

        public event ThirdRadioControlSelectedElementChangedHandler OnSelectedElementChanged;

        public ThirdRadioControl()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += ThirdRadioControl_Loaded;
        }

        private void ThirdRadioControl_Loaded(object sender, RoutedEventArgs e)
        {
            double scaling = SystemUtils.Instance.GetScreenScalingFactor();

            if (scaling != 1)
            {
                TextMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(TextMarginDefault.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(TextMarginDefault.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(TextMarginDefault.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(TextMarginDefault.Bottom / scaling))
                };
                OffsetMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_OFFSET_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_OFFSET_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_OFFSET_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_OFFSET_MARGIN.Bottom / scaling))
                };
            }
            else
            {
                TextMargin = TextMarginDefault;
                OffsetMargin = DEFAULT_OFFSET_MARGIN;
            }
        }

        public Thickness TextMarginDefault { set; get; }

        public string LeftIconPath
        {
            get { return (string)GetValue(LeftIconPathProperty); }
            set
            {
                SetValue(LeftIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty LeftIconPathProperty =
            DependencyProperty.Register("LeftIconPath", typeof(string), typeof(ThirdRadioControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string CenterIconPath
        {
            get { return (string)GetValue(CenterIconPathProperty); }
            set
            {
                SetValue(CenterIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty CenterIconPathProperty =
            DependencyProperty.Register("CenterIconPath", typeof(string), typeof(ThirdRadioControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string RightIconPath
        {
            get { return (string)GetValue(RightIconPathProperty); }
            set
            {
                SetValue(RightIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty RightIconPathProperty =
            DependencyProperty.Register("RightIconPath", typeof(string), typeof(ThirdRadioControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string LeftText
        {
            get { return (string)GetValue(LeftTextProperty); }
            set
            {
                SetValue(LeftTextProperty, value);
            }
        }

        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register("LeftText", typeof(string), typeof(ThirdRadioControl), new PropertyMetadata(string.Empty));

        public string CenterText
        {
            get { return (string)GetValue(CenterTextProperty); }
            set
            {
                SetValue(CenterTextProperty, value);
            }
        }

        public static readonly DependencyProperty CenterTextProperty =
            DependencyProperty.Register("CenterText", typeof(string), typeof(ThirdRadioControl), new PropertyMetadata(string.Empty));

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set
            {
                SetValue(RightTextProperty, value);
            }
        }

        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register("RightText", typeof(string), typeof(ThirdRadioControl), new PropertyMetadata(string.Empty));

        public object LeftElement
        {
            get { return GetValue(LeftElementProperty); }
            set
            {
                SetValue(LeftElementProperty, value);
            }
        }

        public static readonly DependencyProperty LeftElementProperty =
            DependencyProperty.Register("LeftElement", typeof(object), typeof(ThirdRadioControl), new PropertyMetadata(null));

        public object CenterElement
        {
            get { return GetValue(CenterElementProperty); }
            set
            {
                SetValue(CenterElementProperty, value);
            }
        }

        public static readonly DependencyProperty CenterElementProperty =
            DependencyProperty.Register("CenterElement", typeof(object), typeof(ThirdRadioControl), new PropertyMetadata(null));

        public object RightElement
        {
            get { return GetValue(RightElementProperty); }
            set
            {
                SetValue(RightElementProperty, value);
            }
        }

        public static readonly DependencyProperty RightElementProperty =
            DependencyProperty.Register("RightElement", typeof(object), typeof(ThirdRadioControl), new PropertyMetadata(null));

        public object SelectElement
        {
            get { return (object)GetValue(SelectElementProperty); }
            set
            {
                object old = SelectElement;
                SetValue(SelectElementProperty, value);
                SetStyle(value);

                if (!value.Equals(old))
                {
                    OnSelectedElementChanged?.Invoke(this, value);
                }
            }
        }

        public static readonly DependencyProperty SelectElementProperty =
            DependencyProperty.Register("SelectElement", typeof(object), typeof(ThirdRadioControl), new PropertyMetadata(null));

        public int LeftZIndex
        {
            get { return (int)GetValue(LeftZIndexProperty); }
            set
            {
                SetValue(LeftZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty LeftZIndexProperty =
            DependencyProperty.Register("LeftZIndex", typeof(int), typeof(ThirdRadioControl), new PropertyMetadata(0));

        public int CenterZIndex
        {
            get { return (int)GetValue(CenterZIndexProperty); }
            set
            {
                SetValue(CenterZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty CenterZIndexProperty =
            DependencyProperty.Register("CenterZIndex", typeof(int), typeof(ThirdRadioControl), new PropertyMetadata(0));

        public int RightZIndex
        {
            get { return (int)GetValue(RightZIndexProperty); }
            set
            {
                SetValue(RightZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty RightZIndexProperty =
            DependencyProperty.Register("RightZIndex", typeof(int), typeof(ThirdRadioControl), new PropertyMetadata(1));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set
            {
                SetValue(TextMarginProperty, value);
            }
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(ThirdRadioControl), new PropertyMetadata(DEFAULT_TEXT_MARGIN));

        public Thickness OffsetMargin
        {
            get { return (Thickness)GetValue(OffsetMarginProperty); }
            set
            {
                SetValue(OffsetMarginProperty, value);
            }
        }

        public static readonly DependencyProperty OffsetMarginProperty =
            DependencyProperty.Register("OffsetMargin", typeof(Thickness), typeof(ThirdRadioControl), new PropertyMetadata(DEFAULT_OFFSET_MARGIN));


        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set
            {
                SetValue(GroupNameProperty, value);
            }
        }

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(ThirdRadioControl), new PropertyMetadata(GROUP_NAME));


        private void Left_Checked(object sender, RoutedEventArgs e)
        {
            SelectElement = LeftElement;
        }

        private void Center_Checked(object sender, RoutedEventArgs e)
        {
            SelectElement = CenterElement;
        }

        private void Right_Checked(object sender, RoutedEventArgs e)
        {
            SelectElement = RightElement;
        }

        private void SetStyle(object selectElement)
        {
            if (selectElement.Equals(LeftElement))
            {
                if (!Left.IsChecked.HasValue || !Left.IsChecked.Value) Left.IsChecked = true;
                LeftZIndex = 1;
                CenterZIndex = 0;
                RightZIndex = 0;
                //OnSelectedElementChanged?.Invoke(this, LeftElement);
            }
            else if (selectElement.Equals(CenterElement))
            {
                if (!Center.IsChecked.HasValue || !Center.IsChecked.Value) Center.IsChecked = true;
                LeftZIndex = 0;
                CenterZIndex = 1;
                RightZIndex = 0;
                //OnSelectedElementChanged?.Invoke(this, CenterElement);
            }
            else if (selectElement.Equals(RightElement))
            {
                if (!Right.IsChecked.HasValue || !Right.IsChecked.Value) Right.IsChecked = true;
                LeftZIndex = 0;
                CenterZIndex = 0;
                RightZIndex = 1;
                //OnSelectedElementChanged?.Invoke(this, RightElement);
            }
        }
    }
}
