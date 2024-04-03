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
using yz.gaming.accessoryapp.View.ControllerPage;

namespace yz.gaming.accessoryapp.View.Main
{
    /// <summary>
    /// ControllerPage.xaml 的交互逻辑
    /// </summary>
    public partial class ControllerPageView : Page, IPageViewInterface
    {
        ControllerPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public ControllerPageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<ControllerPageViewModel>();
            DataContext = _viewModel;

            _viewModel.ChildPageMap = new Dictionary<IPageListItem, IPageViewInterface>
            {
                { KeyButton, new KeyPageView()},
                //{ LightEffectButton, new LightEffectPageView() },
                { ShakeButton, new ShakePageView() },
                //{ GyroButton, new GyroPageView() },
                { FirmwareButton, new FirmwarePageView() }
            };

            KeyButton.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            //LightEffectButton.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            ShakeButton.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            //GyroButton.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            FirmwareButton.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            KeyButton.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            //LightEffectButton.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            ShakeButton.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            //GyroButton.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            FirmwareButton.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;

            KeyButton.OnClick += _viewModel.OnButtonClick;
            //LightEffectButton.OnClick += _viewModel.OnButtonClick;
            ShakeButton.OnClick += _viewModel.OnButtonClick;
            //GyroButton.OnClick += _viewModel.OnButtonClick;
            FirmwareButton.OnClick += _viewModel.OnButtonClick;

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
                KeyButton.IsHoved = true;
                KeyButton.IsSelected = true;
            }
        }
    }
}
