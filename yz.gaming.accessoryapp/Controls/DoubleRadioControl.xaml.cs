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
    /// EitherOrControl.xaml 的交互逻辑
    /// </summary>
    public partial class DoubleRadioControl : UserControl
    {
        public delegate void DoubleRadioControlSelectedElementChangedHandler(DoubleRadioControl sender, object element);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";
        const string GROUP_NAME = "EitherOrControl";

        static SolidColorBrush DEFAULT_BACKGROUND_BRUSH = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
        static SolidColorBrush SELECTED_BACKGROUND_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x1A, 0xA7, 0x4F));

        static Thickness DEFAULT_ICON_MARGIN = new Thickness(24, 4, 16, 4);
        static Thickness DEFAULT_TEXT_MARGIN = new Thickness(0, 10, 48, 10);
        static Thickness DEFAULT_OFFSET_MARGIN = new Thickness(0, 0, -16, 0);

        public event DoubleRadioControlSelectedElementChangedHandler OnSelectedElementChanged;

        public DoubleRadioControl()
        {
            InitializeComponent();
            DataContext = this;

            this.Loaded += EitherOrControl_Loaded;
        }

        private void EitherOrControl_Loaded(object sender, RoutedEventArgs e)
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
                IconMargin = DEFAULT_ICON_MARGIN;
                TextMargin = DEFAULT_TEXT_MARGIN;
                OffsetMargin = DEFAULT_OFFSET_MARGIN;
            }
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
            DependencyProperty.Register("LeftIconPath", typeof(string), typeof(DoubleRadioControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string RightIconPath
        {
            get { return (string)GetValue(RightIconPathProperty); }
            set
            {
                SetValue(RightIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty RightIconPathProperty =
            DependencyProperty.Register("RightIconPath", typeof(string), typeof(DoubleRadioControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string LeftText
        {
            get { return (string)GetValue(LeftTextProperty); }
            set
            {
                SetValue(LeftTextProperty, value);
            }
        }

        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register("LeftText", typeof(string), typeof(DoubleRadioControl), new PropertyMetadata(string.Empty));

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set
            {
                SetValue(RightTextProperty, value);
            }
        }

        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register("RightText", typeof(string), typeof(DoubleRadioControl), new PropertyMetadata(string.Empty));

        public object LeftElement
        {
            get { return GetValue(LeftElementProperty); }
            set
            {
                SetValue(LeftElementProperty, value);
            }
        }

        public static readonly DependencyProperty LeftElementProperty =
            DependencyProperty.Register("LeftElement", typeof(object), typeof(DoubleRadioControl), new PropertyMetadata(null));

        public object RightElement
        {
            get { return GetValue(RightElementProperty); }
            set
            {
                SetValue(RightElementProperty, value);
            }
        }

        public static readonly DependencyProperty RightElementProperty =
            DependencyProperty.Register("RightElement", typeof(object), typeof(DoubleRadioControl), new PropertyMetadata(null));

        public object SelectElement
        {
            get { return (object)GetValue(SelectElementProperty); }
            set
            {
                //System.Diagnostics.Debug.WriteLine($"Trace : DoubleRadioControl's SelectElement change to [{value}]");
                var befor = SelectElement;
                SetValue(SelectElementProperty, value);
                SetStyle(value);
                if (value != null && !value.Equals(befor)) OnSelectedElementChanged?.Invoke(this, value);
            }
        }

        public static readonly DependencyProperty SelectElementProperty =
            DependencyProperty.Register("SelectElement", typeof(object), typeof(DoubleRadioControl), new PropertyMetadata(null));

        public int LeftZIndex
        {
            get { return (int)GetValue(LeftZIndexProperty); }
            set
            {
                SetValue(LeftZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty LeftZIndexProperty =
            DependencyProperty.Register("LeftZIndex", typeof(int), typeof(DoubleRadioControl), new PropertyMetadata(0));

        public int RightZIndex
        {
            get { return (int)GetValue(RightZIndexProperty); }
            set
            {
                SetValue(RightZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty RightZIndexProperty =
            DependencyProperty.Register("RightZIndex", typeof(int), typeof(DoubleRadioControl), new PropertyMetadata(1));

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set
            {
                SetValue(GroupNameProperty, value);
            }
        }

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(DoubleRadioControl), new PropertyMetadata(GROUP_NAME));

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set
            {
                SetValue(IconMarginProperty, value);
            }
        }

        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(DoubleRadioControl), new PropertyMetadata(DEFAULT_ICON_MARGIN));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set
            {
                SetValue(TextMarginProperty, value);
            }
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(DoubleRadioControl), new PropertyMetadata(DEFAULT_TEXT_MARGIN));

        public Thickness OffsetMargin
        {
            get { return (Thickness)GetValue(OffsetMarginProperty); }
            set
            {
                SetValue(OffsetMarginProperty, value);
            }
        }

        public static readonly DependencyProperty OffsetMarginProperty =
            DependencyProperty.Register("OffsetMargin", typeof(Thickness), typeof(DoubleRadioControl), new PropertyMetadata(DEFAULT_OFFSET_MARGIN));


        private void Left_Checked(object sender, RoutedEventArgs e)
        {
            SelectElement = LeftElement;
        }

        private void Right_Checked(object sender, RoutedEventArgs e)
        {
            SelectElement = RightElement;
        }

        private void SetStyle(object selectElement)
        {
            if (selectElement == null) return;

            if (selectElement.Equals(LeftElement))
            {
                if (!Left.IsChecked.HasValue || !Left.IsChecked.Value) Left.IsChecked = true;
                LeftZIndex = 1;
                RightZIndex = 0;
                LeftBorder.Background = SELECTED_BACKGROUND_BRUSH;
                RightBorder.Background = DEFAULT_BACKGROUND_BRUSH;
            }
            else if (selectElement.Equals(RightElement))
            {
                if (!Right.IsChecked.HasValue || !Right.IsChecked.Value) Right.IsChecked = true;
                LeftZIndex = 0;
                RightZIndex = 1;
                LeftBorder.Background = DEFAULT_BACKGROUND_BRUSH;
                RightBorder.Background = SELECTED_BACKGROUND_BRUSH;
            }
        }
    }
}
