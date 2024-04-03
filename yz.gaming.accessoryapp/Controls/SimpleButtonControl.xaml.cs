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
    /// SimpleButtonControl.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleButtonControl : UserControl, IPageListItem
    {
        private ItemEffect _itemEffect;

        public delegate void SimpleButtonControlClickHandler(IPageListItem sender);

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        public event ItemSelectedStateChangeHandler OnSelectedStateChange;
        public event ItemHovedStateChangeHandler OnHovedStateChange;
        public event SimpleButtonControlClickHandler OnClick;

        TimeSpan _lastPressTime;

        public SimpleButtonControl()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += SimpleButtonControl_Loaded;
            
            _itemEffect = new ItemEffect() { Border = Border, MainGrid = MainGrid };
            _itemEffect.DefaultBorderThickness = new Thickness(1);
        }

        private void SimpleButtonControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonEffect(IsSelected, IsHoved);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SimpleButtonControl), new PropertyMetadata(string.Empty));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                SetButtonEffect(value, value ? IsHoved : false);

                OnSelectedStateChange?.Invoke(this, value);
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SimpleButtonControl), new PropertyMetadata(false));

        public bool IsHoved
        {
            get { return (bool)GetValue(IsHovedProperty); }
            set
            {
                SetValue(IsHovedProperty, value);
                OnHovedStateChange?.Invoke(this, value);

                IsSelected = value;
            }
        }

        public static readonly DependencyProperty IsHovedProperty =
            DependencyProperty.Register("IsHoved", typeof(bool), typeof(SimpleButtonControl), new PropertyMetadata(false));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set
            {
                SetValue(IndexProperty, value);
            }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(SimpleButtonControl), new PropertyMetadata(0));

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

            if (!CheckPress()) return;
            OnClick?.Invoke(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!CheckPress()) return;
            OnClick?.Invoke(this);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                if (!CheckPress()) return;
                OnClick?.Invoke(this);
            }
        }

        public void SetButtonEffect(bool isSelected, bool isHoved)
        {
            _itemEffect.SetButtonEffect_GamePlatformButton(isSelected, isHoved);
        }

        public void ConfirmPressed()
        {
            if (!IsSelected || !IsHoved) return;
            IsSelected = true;
            if (!CheckPress()) return;
            OnClick?.Invoke(this);
        }

        private bool CheckPress()
        {
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            if (now.Subtract(_lastPressTime).TotalSeconds < 1) return false;
            _lastPressTime = now;

            return true;
        }
    }
}
