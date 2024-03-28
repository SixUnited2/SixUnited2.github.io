using CommunityToolkit.Mvvm.DependencyInjection;
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
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.Setting;

namespace yz.gaming.accessoryapp.View.Setting
{
    /// <summary>
    /// DeviceInfoPage.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceInfoPageView : Page, IPageViewInterface
    {
        DeviceInfoPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public DeviceInfoPageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<DeviceInfoPageViewModel>();
            DataContext = _viewModel;
            Loaded += DeviceInfoPageView_Loaded;
        }

        private void DeviceInfoPageView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Initialization();
            DeviceModel.Text = _viewModel.DeviceModel;
            Processor.Text = _viewModel.Processor;
            GraphicsCard.Text = _viewModel.GraphicsCard;
            Memory.Text = _viewModel.Memory;
            Disk.Text = _viewModel.Disk;
            DisplayhScreen.Text = _viewModel.DisplayhScreen;
            BetteryCapacity.Text = _viewModel.BetteryCapacity;
            WLAN.Text = _viewModel.WLAN;
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();
            return this;
        }
    }
}
