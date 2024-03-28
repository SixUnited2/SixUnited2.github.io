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
using yz.gaming.accessoryapp.ViewModel.ControllerPage;

namespace yz.gaming.accessoryapp.View.ControllerPage
{
    /// <summary>
    /// GyroPageView.xaml 的交互逻辑
    /// </summary>
    public partial class GyroPageView : Page, IPageViewInterface
    {
        GyroPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;
        public GyroPageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<GyroPageViewModel>();
            DataContext = _viewModel;

            _viewModel.ListItems = new List<IPageListItem>
            {
                EmulateMouse,
                Sensitivities
            };

            EmulateMouse.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Sensitivities.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            EmulateMouse.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Sensitivities.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;

            EmulateMouse.OnClick += _viewModel.OnButtonClick;
            Sensitivities.OnClick += _viewModel.OnButtonClick;

            EmulateMouse.OnCheckedStateChanged += OnCheckedStateChanged;
            Sensitivities.OnSliderListItemValueChanged += OnSliderListItemValueChanged;

            this.Loaded += GyroPageView_Loaded;
        }

        private void GyroPageView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.UnSelectAllItem();
            //EmulateMouse.IsSelected = true;
            EmulateMouse.IsHoved = true;
            Sensitivities.Value = (int)_viewModel.Sensitivities;
        }


        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();
            return this;
        }

        private void OnCheckedStateChanged(IPageListItem sender, bool isChecked)
        {
            _viewModel.EmulateMouse = isChecked;
        }

        private void OnSliderListItemValueChanged(IPageListItem sender, int value)
        {
            _viewModel.Sensitivities = Convert.ToByte(value);
        }
    }
}
