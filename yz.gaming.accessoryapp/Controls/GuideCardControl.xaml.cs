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
    /// GuidCardControl.xaml 的交互逻辑
    /// </summary>
    public partial class GuideCardControl : UserControl, IPageListItem
    {
        public delegate void GuideCardControlSelectedStateChangeHandler(IPageListItem sender, bool isSelected);
        public delegate void GuideCardControlClickHandler(IPageListItem sender);

        const int SELECTED_EXPEND = 6;
        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        static Thickness DEFAULT_MARGIN_THICKNESS = new Thickness(6);
        static Thickness SELECTED_MARGIN_THICKNESS = new Thickness(0);

        static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x25, 0x25, 0x25));
        static SolidColorBrush SELECTED_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x40, 0x40, 0x40));

        public event GuideCardControlSelectedStateChangeHandler OnSelectedStateChange;
        public event GuideCardControlClickHandler OnClick;

        public GuideCardControl()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(GuideCardControl), new PropertyMetadata(DEFUALT_ICON_PATH));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(GuideCardControl), new PropertyMetadata(string.Empty));

        public string TextDescript
        {
            get { return (string)GetValue(TextDescriptProperty); }
            set { SetValue(TextDescriptProperty, value); }
        }

        public static readonly DependencyProperty TextDescriptProperty =
            DependencyProperty.Register("TextDescript", typeof(string), typeof(GuideCardControl), new PropertyMetadata(string.Empty));

        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(string), typeof(GuideCardControl), new PropertyMetadata(string.Empty));


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                SetButtonEffect(value, false);

                OnSelectedStateChange?.Invoke(this, value);
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GuideCardControl), new PropertyMetadata(false));

        public bool IsHoved
        {
            get { return (bool)GetValue(IsHovedProperty); }
            set
            {
                SetValue(IsHovedProperty, value);
                SetButtonEffect(IsSelected, value);
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(GuideCardControl), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(GuideCardControl), new PropertyMetadata(0));

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsSelected = true;
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            OnClick?.Invoke(this);
            IsSelected = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            OnClick?.Invoke(this);
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
        public void SetButtonEffect(bool isSelected, bool isHoved)
        {
            Main.Background = isSelected ? SELECTED_BRUSH : DEFAULT_BRUSH;
            Main.Margin = isSelected ? SELECTED_MARGIN_THICKNESS : DEFAULT_MARGIN_THICKNESS;
        }

        public void ConfirmPressed()
        {
            if (!IsSelected || !IsHoved) return;

            OnClick?.Invoke(this);
        }
    }
}
