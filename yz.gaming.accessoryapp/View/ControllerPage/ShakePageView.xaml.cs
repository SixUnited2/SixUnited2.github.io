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
    /// ShakePageView.xaml 的交互逻辑
    /// </summary>
    public partial class ShakePageView : Page, IPageViewInterface
    {
        ShakePageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;
        public ShakePageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<ShakePageViewModel>();
            DataContext = _viewModel;

            _viewModel.ListItems = new List<IPageListItem>
            {
                Shake
            };

            Shake.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Shake.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Shake.OnClick += _viewModel.OnButtonClick;
            Shake.OnSelectElementChanged += OnSelectElementChanged;

            this.Loaded += ShakePageView_Loaded;
        }

        private void OnSelectElementChanged(IPageListItem sender, object element)
        {
            _viewModel.MotorIntensity = (byte)element;
        }

        private void ShakePageView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.UnSelectAllItem();

            _viewModel.Initialization();

            //Shake.IsSelected = true;
            Shake.IsHoved = true;
            Shake.LeftElement = (byte)2;
            Shake.CenterElement = (byte)1;
            Shake.RightElement = (byte)0;
            Shake.SelectElement = _viewModel.MotorIntensity;
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();
            return this;
        }
    }
}
