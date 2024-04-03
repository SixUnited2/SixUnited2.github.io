using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.ControllerPage;

namespace yz.gaming.accessoryapp.View.ControllerPage
{
    /// <summary>
    /// FirmwarePageView.xaml 的交互逻辑
    /// </summary>
    public partial class FirmwarePageView : Page, IPageViewInterface
    {
        FirmwarePageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public FirmwarePageView()
        {
            InitializeComponent();
            _viewModel = Ioc.Default.GetRequiredService<FirmwarePageViewModel>();
            DataContext = _viewModel;

            Loaded += FirmwarePageView_Loaded;
            Update.OnClick += Update_OnClick;
        }

        private void FirmwarePageView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateText.Text = _viewModel.GetString("Update");
            Update.IsEnabled = true;
            _viewModel.Initialization();
        }

        private void Update_OnClick(Controls.ClickEffectGrid sender)
        {
            Update.IsEnabled = false;

            Task.Run(async () =>
            {
                await FwUpdateUtils.Instance.ApplyUpdateAsync(
                    (msg, progress) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            UpdateText.Text = $"{_viewModel.GetString(msg)} {progress:0.0}%";
                        });
                    },
                    (isSuccess, msg) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            UpdateText.Text = $"{_viewModel.GetString(msg)}";
                        });
                    });
            });
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();
            return this;
        }
    }
}
