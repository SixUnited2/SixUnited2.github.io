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
using yz.gaming.accessoryapp.Languange;
using static yz.gaming.accessoryapp.Controls.ItemEffect;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// ButtonChcekBoxListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonChcekBoxListItem : UserControl, IPageListItem
    {
        private ItemEffect _itemEffect;

        public delegate void ButtonChcekBoxListItemCheckedStateChangedHandler(IPageListItem sender, bool isChecked);
        public delegate void ButtonChcekBoxListItemClickHandler(IPageListItem sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event ButtonChcekBoxListItemCheckedStateChangedHandler OnCheckedStateChanged;
        public event ButtonChcekBoxListItemClickHandler OnClick;

        public ButtonChcekBoxListItem()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Loaded += CheckBoxListItem_Loaded;

            _itemEffect = new ItemEffect() { Border = Border, MainGrid = MainGrid };
        }

        private void CheckBoxListItem_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonEffect(IsSelected, IsHoved);
            BCheckBox.UnCheckedText = LanguangeManager.Instance.GetString("Off");
            BCheckBox.Content = LanguangeManager.Instance.GetString("On");
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ButtonChcekBoxListItem), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set
            {
                SetValue(IconPathProperty, value);
            }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(ButtonChcekBoxListItem), new PropertyMetadata(DEFUALT_ICON_PATH));

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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ButtonChcekBoxListItem), new PropertyMetadata(false));

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
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(ButtonChcekBoxListItem), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(ButtonChcekBoxListItem), new PropertyMetadata(0));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set
            {
                if (BCheckBox.IsChecked.Value != value)
                {
                    BCheckBox.IsChecked = value;
                }

                SetValue(IsCheckedProperty, value);
            }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ButtonChcekBoxListItem), new PropertyMetadata(false));

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            IsSelected = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsSelected = true;
            IsChecked = !IsChecked;
            OnCheckedStateChanged?.Invoke(this, IsChecked);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                if (!IsSelected)
                {
                    IsSelected = true;
                }
                else
                {
                    IsChecked = !IsChecked;
                    OnCheckedStateChanged?.Invoke(this, IsChecked);
                }
            }
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
                IsChecked = !IsChecked;
                OnCheckedStateChanged?.Invoke(this, IsChecked);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (BCheckBox.IsChecked.Value != IsChecked)
            {
                IsChecked = true;
            }
            OnCheckedStateChanged?.Invoke(this, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (BCheckBox.IsChecked.Value != IsChecked)
            {
                IsChecked = false;
            }
            OnCheckedStateChanged?.Invoke(this, false);
        }
    }
}
