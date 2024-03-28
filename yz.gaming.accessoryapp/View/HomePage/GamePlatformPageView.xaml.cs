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
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Languange;
using yz.gaming.accessoryapp.Service;
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.HomePage;

namespace yz.gaming.accessoryapp.View.HomePage
{
    /// <summary>
    /// GamePlatformPage.xaml 的交互逻辑
    /// </summary>
    public partial class GamePlatformPageView : Page, IPageViewInterface
    {
        const string INSTALL_BG = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_V.png";
        const string UNINSTALL_BG = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_V_2.png";

        GamePlatformPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public GamePlatformPageView()
        {
            InitializeComponent();
            _viewModel = Ioc.Default.GetRequiredService<GamePlatformPageViewModel>();
            DataContext = _viewModel;

            _viewModel.ListItems = new List<IPageListItem>
            {
                Steam,
                Epic,
                EA,
                WeGame,
                XBox,
                RockStar,
                GOG,
                Ubi,
                Blizzard
            };

            Steam.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Epic.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            EA.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            WeGame.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            XBox.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RockStar.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            GOG.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Ubi.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Blizzard.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            Steam.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Epic.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            EA.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            WeGame.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            XBox.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RockStar.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            GOG.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Ubi.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Blizzard.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;

            Steam.OnClick += _viewModel.PlatformClick;
            Epic.OnClick += _viewModel.PlatformClick;
            EA.OnClick += _viewModel.PlatformClick;
            WeGame.OnClick += _viewModel.PlatformClick;
            XBox.OnClick += _viewModel.PlatformClick;
            RockStar.OnClick += _viewModel.PlatformClick;
            GOG.OnClick += _viewModel.PlatformClick;
            Ubi.OnClick += _viewModel.PlatformClick;
            Blizzard.OnClick += _viewModel.PlatformClick;

            this.Loaded += GamePlatformPageView_Loaded;
        }

        private void GamePlatformPageView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetButtonSize(ActualWidth);
            Task.Run(async () =>
            {
                GamePlatform.Instance.GetAppList();
                await GamePlatform.Instance.GetUwpApp();
                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _viewModel.ListItems.ForEach(p =>
                    {
                        if (p is DynamicButtonControl item)
                        {
                            item.BorderCornerRadius = _viewModel.CornerRadius;
                            var platform = GamePlatform.Instance.GetPlatformModel((Model.PlatformEnum)item.Index);

                            item.Text1 = platform.IsInstall ? _viewModel.GetString("Open") : _viewModel.GetString("Unload");
                            item.ImagePath = platform.IsInstall ? INSTALL_BG : UNINSTALL_BG;
                        }

                        p.SetButtonEffect(false, false);
                    });

                    Steam.IsHoved = true;
                    Steam.IsSelected = true;
                }));
            });
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.OnTipButtomMapChanged = navigationParent.OnTipButtonMapChanged;
            _viewModel.Initialization();
            return this;
        }
    }
}
