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
using yz.gaming.accessoryapp.View.Main;
using yz.gaming.accessoryapp.Service;
using System.Threading.Tasks;
using System.Threading;

namespace yz.gaming.accessoryapp.View
{
    /// <summary>
    /// MainContainer.xaml 的交互逻辑
    /// </summary>
    public partial class MainContainerView : Page
    {
        MainContainerViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;
        
        public MainContainerView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<MainContainerViewModel>();
            DataContext = _viewModel;

            _viewModel.PageContainer = PageContainer;
            _viewModel.PageList = new List<IPageViewInterface>()
            {
                new NewHomePageView().Init(_viewModel),
                new ControllerPageView().Init(_viewModel),
                //new GuidePage().Init(_viewModel),
                new SettingPageView().Init(_viewModel)
            };

            _viewModel.MenuList = new List<INavigationItem>()
            {
                MainPageText,
                ControllerPageText,
                //GuidePageText,
                SettingPageText
            };

            MainPageText.OnSelectedStateChange += _viewModel.OnSelectedStateChange;
            ControllerPageText.OnSelectedStateChange += _viewModel.OnSelectedStateChange;
            //GuidePageText.OnSelectedStateChange += _viewModel.OnSelectedStateChange;
            SettingPageText.OnSelectedStateChange += _viewModel.OnSelectedStateChange;

            LB_Button.Click += LB_Button_Click;
            RB_Button.Click += RB_Button_Click;

            WinCtr.OnMiniWindows += _viewModel.OnMiniWindows;
            WinCtr.OnHideWindows += YzGamingService.Instance.HideMainWindows;

            _viewModel.InitNavigationData();

            this.Loaded += MainContainer_Loaded;

            YzGamingService.Instance.NavigationToPage = NavigationToPage;
        }

        private void NavigationToPage(Queue<Type> pages)
        {
            YzGamingService.Instance.HideQuickMenu();

            INavigationSupport navigation = _viewModel;
            IChildPageSupport pageVM = null;

            Type t = pages.Dequeue();

            for (int i = 0; i < navigation.PageList.Count; i++)
            {
                if (t.Equals(navigation.PageList[i].GetType()))
                {
                    navigation.NavigationTo(i);
                    if (navigation.CurrentPageViewModel is IChildPageSupport vm)
                    {
                        pageVM = vm;
                    }
                }
            }

            while (pages.Count > 0 && pageVM != null)
            {
                var type = pages.Dequeue();

                foreach (var item in pageVM.ChildPageMap)
                {
                    if (item.Value != null &&
                        type.Equals(item.Value.GetType()) &&
                        pageVM is ChildPageSupportViewModelBase childPageSupport)
                    {
                        childPageSupport.OnButtonClick(item.Key);
                        pageVM = item.Value.ViewModel is IChildPageSupport vm ? vm : null;
                        break;
                    }
                }
            }

            YzGamingService.Instance.ShowMainWindows();
        }

        private void RB_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentPageViewModel is IMenuSupport menuViewModel)
            {
                menuViewModel.MenuSelect(menuViewModel.CurrentIndex + 1);
            }
            else
            {
                _viewModel.NavigationTo(_viewModel.CurrentMenu.Index + 1);
                _viewModel.CurrentMenu.IsSelected = true;
            }
        }

        private void LB_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentPageViewModel is IMenuSupport menuViewModel)
            {
                menuViewModel.MenuSelect(menuViewModel.CurrentIndex - 1);
            }
            else
            {
                _viewModel.NavigationTo(_viewModel.CurrentMenu.Index - 1);
                _viewModel.CurrentMenu.IsSelected = true;
            }
        }

        private void MainContainer_Loaded(object sender, RoutedEventArgs e)
        {
            MainPageText.IsSelected = true;
        }

        private void TipButtonControl_OnTipButtonClick(object sender)
        {
            _viewModel.Back(sender);
        }

        private void ButtonClick(object sender)
        {
            _viewModel.HandleKeyEvent((Api.YzCommonApi.KeyCodeEnum)Convert.ToInt32((sender as Control).Tag), Api.YzCommonApi.KeyPressTypeEnmu.AppClick);
        }

        private void QuickMenuButtonClick(object sender)
        {
            QuickMenuButton.IsEnabled = false;

            Task.Run(async () =>
            {
                await Task.Delay(100);
                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        YzGamingService.Instance.ShowQuickMenu();
                    }
                    finally
                    {
                        QuickMenuButton.IsEnabled = true;
                    }
                }));
            });
            
        }
    }
}
