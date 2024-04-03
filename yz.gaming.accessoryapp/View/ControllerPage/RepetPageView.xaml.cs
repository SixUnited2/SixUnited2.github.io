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
    /// RepetPageView.xaml 的交互逻辑
    /// </summary>
    public partial class RepetPageView : Page, IPageViewInterface
    {
        RepetPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public RepetPageView()
        {
            InitializeComponent();
            _viewModel = Ioc.Default.GetRequiredService<RepetPageViewModel>();
            DataContext = _viewModel;

            _viewModel.ListItems = new List<IPageListItem>
            {
                RepeatMode,
                RepeatMode_AKey,
                RepeatMode_BKey,
                RepeatMode_XKey,
                RepeatMode_YKey,
                RepeatMode_LeftShoulderKey,
                RepeatMode_RightShoulderKey
            };

            RepeatMode.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RepeatMode_AKey.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RepeatMode_BKey.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RepeatMode_XKey.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RepeatMode_YKey.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RepeatMode_LeftShoulderKey.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RepeatMode_RightShoulderKey.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            RepeatMode.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RepeatMode_AKey.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RepeatMode_BKey.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RepeatMode_XKey.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RepeatMode_YKey.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RepeatMode_LeftShoulderKey.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RepeatMode_RightShoulderKey.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;

            RepeatMode.OnCheckedStateChanged += RepeatMode_OnCheckedStateChanged;
            RepeatMode_AKey.OnCheckedStateChanged += RepeatMode_OnCheckedStateChanged;
            RepeatMode_BKey.OnCheckedStateChanged += RepeatMode_OnCheckedStateChanged;
            RepeatMode_XKey.OnCheckedStateChanged += RepeatMode_OnCheckedStateChanged;
            RepeatMode_YKey.OnCheckedStateChanged += RepeatMode_OnCheckedStateChanged;
            RepeatMode_LeftShoulderKey.OnCheckedStateChanged += RepeatMode_OnCheckedStateChanged;
            RepeatMode_RightShoulderKey.OnCheckedStateChanged += RepeatMode_OnCheckedStateChanged;

            this.Loaded += RepetPageView_Loaded;
        }

        private void RepeatMode_OnCheckedStateChanged(IPageListItem sender, bool isChecked)
        {
            switch (sender.Index)
            {
                case 0:
                    _viewModel.TurboOpen = isChecked;
                    break;
                case 1:
                    _viewModel.TurboA = isChecked;
                    break;
                case 2:
                    _viewModel.TurboB = isChecked;
                    break;
                case 3:
                    _viewModel.TurboX = isChecked;
                    break;
                case 4:
                    _viewModel.TurboY = isChecked;
                    break;
                case 5:
                    _viewModel.TurboL1 = isChecked;
                    break;
                case 6:
                    _viewModel.TurboL2 = isChecked;
                    break;
            }
        }

        private void RepetPageView_Loaded(object sender, RoutedEventArgs e)
        {
            RepeatMode.IsSelected = true;
            RepeatMode.IsHoved = true;

            _viewModel.Initialization();

            RepeatMode.IsChecked = _viewModel.TurboOpen;
            RepeatMode_AKey.IsChecked = _viewModel.TurboA;
            RepeatMode_BKey.IsChecked = _viewModel.TurboB;
            RepeatMode_XKey.IsChecked = _viewModel.TurboX;
            RepeatMode_YKey.IsChecked = _viewModel.TurboY;
            RepeatMode_LeftShoulderKey.IsChecked = _viewModel.TurboL1;
            RepeatMode_RightShoulderKey.IsChecked = _viewModel.TurboL2;
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();
            return this;
        }
    }
}
