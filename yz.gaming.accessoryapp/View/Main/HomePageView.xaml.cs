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
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.Main;
using yz.gaming.accessoryapp.View.HomePage;
using yz.gaming.accessoryapp.Model;

namespace yz.gaming.accessoryapp.View.Main
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePageView : Page, IPageViewInterface
    {
        HomePageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public HomePageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            DataContext = _viewModel;

            _viewModel.ChildPageMap = new Dictionary<IPageListItem, IPageViewInterface>()
            {
                { OpenButton, null },
                { MoreButton, new GamePlatformPageView() },
                { DeviceInfoButton, null }
            };

            OpenButton.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            MoreButton.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            DeviceInfoButton.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            OpenButton.OnClick += _viewModel.OpenButtonlick;
            MoreButton.OnClick += _viewModel.OnButtonClick;
            DeviceInfoButton.OnClick += _viewModel.OnButtonClick;

            this.Loaded += MainPage_Loaded;
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.OnChildPageNavigation += navigationParent.ChildPageNavgation;
            _viewModel.OnPlatformChanged += OnPlatformChanged;
            _viewModel.Initialization();

            OpenButton.Text = _viewModel.GetPlatName();
            OpenButton.IconPath = _viewModel.Platform.Icon;
            OpenButton.IsSelected = true;
            return this;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            OpenButton.IsHoved = true;
            OpenButton.IsSelected = true;
        }

        private void OnPlatformChanged()
        {
            OpenButton.Text = _viewModel.GetPlatName();
            OpenButton.IconPath = _viewModel.Platform.Icon;
        }
    }
}
