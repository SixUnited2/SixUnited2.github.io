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
    /// ImageListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ImageListItem : UserControl, IPageListItem
    {
        private ItemEffect _itemEffect;

        public delegate void ImageListItemClickHandler(IPageListItem sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event ImageListItemClickHandler OnClick;

        public ImageListItem()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Loaded += ImageListItem_Loaded;

            _itemEffect = new ItemEffect() { Border = Border, MainGrid = MainGrid };
        }

        private void ImageListItem_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonEffect(IsSelected, IsHoved);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ImageListItem), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(ImageListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set
            {
                SetValue(ImagePathProperty, value);
            }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(ImageListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ImageListItem), new PropertyMetadata(false));

        public bool IsHoved
        {
            get { return (bool)GetValue(IsHovedProperty); }
            set
            {
                SetValue(IsHovedProperty, value);
                SetButtonEffect(IsSelected, value);
                OnHovedStateChange?.Invoke(this, value);
                IsSelected = true;
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(ImageListItem), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(ImageListItem), new PropertyMetadata(0));

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

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            if (!_itemEffect.CheckPress()) return;
            OnClick?.Invoke(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!_itemEffect.CheckPress()) return;
            OnClick?.Invoke(this);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                if (!_itemEffect.CheckPress()) return;
                OnClick?.Invoke(this);
            }
        }

        public void SetButtonEffect(bool isSelected, bool isHoved)
        {
            _itemEffect.SetButtonEffect(isSelected, isHoved);
        }

        public void ConfirmPressed()
        {
            if (!IsSelected || !IsHoved) return;

            IsSelected = true;
            OnClick?.Invoke(this);
        }
    }
}
