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
    /// KeyDescriptionPageViewARP31.xaml 的交互逻辑
    /// </summary>
    public partial class KeyDescriptionPageViewARP31 : Page, IPageViewInterface
    {
        KeyDescriptionPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public KeyDescriptionPageViewARP31()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<KeyDescriptionPageViewModel>();
            DataContext = _viewModel;

            ThirdRadio.LeftElement = JoystickMode;
            ThirdRadio.CenterElement = MouKbdMode;
            ThirdRadio.RightElement = CombinationKey;
            ThirdRadio.OnSelectedElementChanged += ThirdRadio_OnSelectedElementChanged;

            _viewModel.MenuList = new List<Grid>()
            {
                JoystickMode,
                MouKbdMode,
                CombinationKey
            };

            _viewModel.OnSelectedMenuChanged += OnSelectedMenuChanged;

            ThirdRadio.SelectElement = ThirdTabControlARP31.SelectElementEnum.LeftElement;

            this.Loaded += KeyDescriptionPageViewARP31_Loaded;
        }

        private void KeyDescriptionPageViewARP31_Loaded(object sender, RoutedEventArgs e)
        {
            ThirdRadio.SelectElement = ThirdTabControlARP31.SelectElementEnum.LeftElement;
            _viewModel.CurrentIndex = 0;
        }

        private void ThirdRadio_OnSelectedElementChanged(ThirdTabControlARP31 sender, object element)
        {
            JoystickMode.Visibility = Visibility.Hidden;
            MouKbdMode.Visibility = Visibility.Hidden;
            CombinationKey.Visibility = Visibility.Hidden;

            _viewModel.CurrentMenu = (Grid)element;
            _viewModel.CurrentIndex = Convert.ToInt32(_viewModel.CurrentMenu.Tag);
            _viewModel.CurrentMenu.Visibility = Visibility.Visible;
            
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();
            return this;
        }

        private void OnSelectedMenuChanged(Grid grid)
        {
            if (grid.Equals(JoystickMode))
            {
                ThirdRadio.SelectElement = ThirdTabControlARP31.SelectElementEnum.LeftElement;
            }
            else if (grid.Equals(MouKbdMode))
            {
                ThirdRadio.SelectElement = ThirdTabControlARP31.SelectElementEnum.CenterElement;
            }
            else if (grid.Equals(CombinationKey))
            {
                ThirdRadio.SelectElement = ThirdTabControlARP31.SelectElementEnum.RightElement;
            }

            
        }
    }
}
