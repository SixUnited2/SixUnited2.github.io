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

namespace yz.gaming.accessoryapp.View.Main
{
    /// <summary>
    /// GuidePage.xaml 的交互逻辑
    /// </summary>
    public partial class GuidePage : Page, IPageViewInterface
    {
        GuidePageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;
        public GuidePage()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<GuidePageViewModel>();
            DataContext = _viewModel;

            _viewModel.ChildPageMap = new Dictionary<IPageListItem, IPageViewInterface>
            {
                { First, null},
                { Second, null },
                { Third, null }
            };

            First.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Second.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Third.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            First.OnClick += _viewModel.OnButtonClick;
            Second.OnClick += _viewModel.OnButtonClick;
            Third.OnClick += _viewModel.OnButtonClick;

            Loaded += GuidePage_Loaded;
        }

        private void GuidePage_Loaded(object sender, RoutedEventArgs e)
        {
            First.IsSelected = true;
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();
            return this;
        }
    }
}
