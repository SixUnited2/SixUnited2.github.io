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
    /// WindowControl.xaml 的交互逻辑
    /// </summary>
    public partial class WindowControl : UserControl
    {
        public event Action OnMiniWindows;
        public event Action OnHideWindows;

        public WindowControl()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            OnHideWindows?.Invoke();
        }

        private void Mini_Click(object sender, RoutedEventArgs e)
        {
            OnMiniWindows?.Invoke();
        }
    }
}
