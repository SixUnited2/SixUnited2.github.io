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
using yz.gaming.accessoryapp.View.Setting;

namespace yz.gaming.accessoryapp.View.Main
{
    /// <summary>
    /// ControllerPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPageView : Page, IPageViewInterface
    {
        SettingPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public SettingPageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<SettingPageViewModel>();
            DataContext = _viewModel;

            _viewModel.ChildPageMap = new Dictionary<IPageListItem, IPageViewInterface>
            {
                //{ DeviceInfo, new DeviceInfoPageView().Init(null) },
                { Language, new LanguagePageView() },
                { SoftwareVersion, new SoftwareVersionPageView() }
            };

            //DeviceInfo.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Language.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            SoftwareVersion.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            //DeviceInfo.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Language.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            SoftwareVersion.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;

            //DeviceInfo.OnClick += _viewModel.OnButtonClick;
            Language.OnClick += _viewModel.OnButtonClick;
            SoftwareVersion.OnClick += _viewModel.OnButtonClick;

            this.Loaded += ControllerPage_Loaded;
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.OnChildPageNavigation += navigationParent.ChildPageNavgation;
            _viewModel.Initialization();

            foreach (var item in _viewModel.ChildPageMap)
            {
                item.Value?.Init(navigationParent);
            }

            return this;
        }

        private void ControllerPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel.PreviousItem == null)
            {
                Language.IsHoved = true;
                Language.IsSelected = true;
            }
        }
    }
}
