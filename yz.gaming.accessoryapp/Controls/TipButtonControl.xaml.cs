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
using yz.gaming.accessoryapp.Utils.Command;

namespace yz.gaming.accessoryapp.Controls
{
    /// <summary>
    /// TipButtonControl.xaml 的交互逻辑
    /// </summary>
    public partial class TipButtonControl : UserControl
    {
        public delegate void TipButtonClickHandler(object sender);
        public event TipButtonClickHandler OnTipButtonClick;

        const string DEFUALT_ICON_PATH = @"pack://SiteOfOrigin:,,,/Resource/Image/None.png";

        TimeSpan _lastPressTime;

        public TipButtonControl()
        {
            InitializeComponent();
            this.DataContext = this;

            _lastPressTime = new TimeSpan(DateTime.Now.Ticks);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TipButtonControl), new PropertyMetadata(string.Empty));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(TipButtonControl), new PropertyMetadata(DEFUALT_ICON_PATH));

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            if (!CheckPress()) return;
            OnTipButtonClick?.Invoke(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!CheckPress()) return;
            OnTipButtonClick?.Invoke(this);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckPress()) return;
            OnTipButtonClick?.Invoke(this);
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
