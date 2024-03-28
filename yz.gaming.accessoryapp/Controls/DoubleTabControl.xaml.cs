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
    /// DoubleTabControl.xaml 的交互逻辑
    /// </summary>
    public partial class DoubleTabControl : UserControl
    {
        public delegate void DoubleTabControlSelectedElementChangedHandler(DoubleTabControl sender, object element);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        public event DoubleTabControlSelectedElementChangedHandler OnSelectedElementChanged;

        public enum SelectElementEnum
        {
            LeftElement,
            RightElement
        }

        public DoubleTabControl()
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
            DependencyProperty.Register("LeftIconPath", typeof(string), typeof(DoubleTabControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string CenterIconPath
        {
            get { return (string)GetValue(CenterIconPathProperty); }
            set
            {
                SetValue(CenterIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty CenterIconPathProperty =
            DependencyProperty.Register("CenterIconPath", typeof(string), typeof(DoubleTabControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string RightIconPath
        {
            get { return (string)GetValue(RightIconPathProperty); }
            set
            {
                SetValue(RightIconPathProperty, value);
            }
        }

        public static readonly DependencyProperty RightIconPathProperty =
            DependencyProperty.Register("RightIconPath", typeof(string), typeof(DoubleTabControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string LeftText
        {
            get { return (string)GetValue(LeftTextProperty); }
            set
            {
                SetValue(LeftTextProperty, value);
            }
        }

        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register("LeftText", typeof(string), typeof(DoubleTabControl), new PropertyMetadata(string.Empty));

        public string CenterText
        {
            get { return (string)GetValue(CenterTextProperty); }
            set
            {
                SetValue(CenterTextProperty, value);
            }
        }

        public static readonly DependencyProperty CenterTextProperty =
            DependencyProperty.Register("CenterText", typeof(string), typeof(DoubleTabControl), new PropertyMetadata(string.Empty));

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set
            {
                SetValue(RightTextProperty, value);
            }
        }

        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register("RightText", typeof(string), typeof(DoubleTabControl), new PropertyMetadata(string.Empty));

        public object LeftElement
        {
            get { return GetValue(LeftElementProperty); }
            set
            {
                SetValue(LeftElementProperty, value);
            }
        }

        public static readonly DependencyProperty LeftElementProperty =
            DependencyProperty.Register("LeftElement", typeof(object), typeof(DoubleTabControl), new PropertyMetadata(null));

        public object CenterElement
        {
            get { return GetValue(CenterElementProperty); }
            set
            {
                SetValue(CenterElementProperty, value);
            }
        }

        public static readonly DependencyProperty CenterElementProperty =
            DependencyProperty.Register("CenterElement", typeof(object), typeof(DoubleTabControl), new PropertyMetadata(null));

        public object RightElement
        {
            get { return GetValue(RightElementProperty); }
            set
            {
                SetValue(RightElementProperty, value);
            }
        }

        public static readonly DependencyProperty RightElementProperty =
            DependencyProperty.Register("RightElement", typeof(object), typeof(DoubleTabControl), new PropertyMetadata(null));

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
            DependencyProperty.Register("SelectElement", typeof(SelectElementEnum), typeof(DoubleTabControl), new PropertyMetadata(SelectElementEnum.LeftElement));

        public int LeftZIndex
        {
            get { return (int)GetValue(LeftZIndexProperty); }
            set
            {
                SetValue(LeftZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty LeftZIndexProperty =
            DependencyProperty.Register("LeftZIndex", typeof(int), typeof(DoubleTabControl), new PropertyMetadata(0));

        public int CenterZIndex
        {
            get { return (int)GetValue(CenterZIndexProperty); }
            set
            {
                SetValue(CenterZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty CenterZIndexProperty =
            DependencyProperty.Register("CenterZIndex", typeof(int), typeof(DoubleTabControl), new PropertyMetadata(0));

        public int RightZIndex
        {
            get { return (int)GetValue(RightZIndexProperty); }
            set
            {
                SetValue(RightZIndexProperty, value);
            }
        }

        public static readonly DependencyProperty RightZIndexProperty =
            DependencyProperty.Register("RightZIndex", typeof(int), typeof(DoubleTabControl), new PropertyMetadata(1));

        private void Left_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectElement != SelectElementEnum.LeftElement)
            {
                SelectElement = SelectElementEnum.LeftElement;
            }
        }

        private void Right_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectElement != SelectElementEnum.RightElement)
            {
                SelectElement = SelectElementEnum.RightElement;
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
                    OnSelectedElementChanged?.Invoke(this, LeftElement);
                    break;
                case SelectElementEnum.RightElement:
                    if (!Right.IsChecked.HasValue || !Right.IsChecked.Value) Right.IsChecked = true;
                    LeftZIndex = 0;
                    CenterZIndex = 0;
                    RightZIndex = 1;
                    OnSelectedElementChanged?.Invoke(this, RightElement);
                    break;
            }
        }
    }
}
