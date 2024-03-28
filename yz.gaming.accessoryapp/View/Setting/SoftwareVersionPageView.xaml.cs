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

namespace yz.gaming.accessoryapp.View.Setting
{
    /// <summary>
    /// SoftwareVersionPageView.xaml 的交互逻辑
    /// </summary>
    public partial class SoftwareVersionPageView : Page, IPageViewInterface
    {
        SoftwareVersionPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public SoftwareVersionPageView()
        {
            InitializeComponent();
            _viewModel = Ioc.Default.GetRequiredService<SoftwareVersionPageViewModel>();
            DataContext = _viewModel;

            Loaded += SoftwareVersionPageView_Loaded;
            Update.OnClick += Update_OnClick;
        }

        private void SoftwareVersionPageView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsNewVersion)
            {
                _viewModel.Initialization();
            }
        }

        private void Update_OnClick(Controls.ClickEffectGrid sender)
        {
            Update.IsEnabled = false;

            Task.Run(async () =>
            {
                await UpdateUtils.Instance.ApplyUpdateAsync(
                    progress =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            UpdateText.Text = $"{_viewModel.GetString("Downloading")} {progress:0.0}%";
                        });
                    },
                    (isSuccess, msg) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            UpdateText.Text = $"{_viewModel.GetString(msg)}";
                            if (isSuccess) Application.Current.Shutdown();
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
