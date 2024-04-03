using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Service;
using yz.gaming.accessoryapp.View.HomePage;
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.Main;
using yz.gaming.accessoryapp.Utils;

namespace yz.gaming.accessoryapp.View.Main
{
    /// <summary>
    /// NewHomePageView.xaml 的交互逻辑
    /// </summary>
    public partial class NewHomePageView : Page, IPageViewInterface
    {
        const string INSTALL_BG = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_LH.png";
        const string UNINSTALL_BG = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_LH_2.png";
        const double COEFFICIENT_WIDTH = (20d / 22d) * (202d / 228d) / 4d;
        const double COEFFICIENT_HEIGHT = 580d / 801d;

        NewHomePageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public NewHomePageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<NewHomePageViewModel>();
            DataContext = _viewModel;

            _viewModel.ChildPageMap = new Dictionary<IPageListItem, IPageViewInterface>()
            {
                { GamePlatformButton, new GamePlatformPageView() }
            };

            _viewModel.TopButton = GamePlatformButton;
            GamePlatformButton.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            GamePlatformButton.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            GamePlatformButton.OnClick += _viewModel.OnButtonClick;

            double width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * COEFFICIENT_WIDTH;
            double height = width * COEFFICIENT_HEIGHT;

            for (int i = 0; i < GamePlatform.Instance.HomePlatforms.Count; i++)
            {
                var platform = GamePlatform.Instance.HomePlatforms[i];

                DynamicButtonControlHV ctr = new DynamicButtonControlHV()
                {
                    BorderCornerRadius = 1,
                    Width = width / SystemUtils.Instance.GetScreenScalingFactor(),
                    Height = height / SystemUtils.Instance.GetScreenScalingFactor(),
                    SelectedExpend = 4,
                    Margin = new Thickness(0),
                    ImagePath = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_LH.png",
                    IconPath = platform.Icon,
                    Index = i,
                    Tag = platform.Platform,
                    Text = platform.Name
                };
                if (platform.Platform == PlatformEnum.STEAM)
                {
                    ctr.ImagePath = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_LH_3.png";
                }
                else
                {
                    ctr.ImagePath = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_LH.png";
                }

                ctr.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
                ctr.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
                ctr.OnClick += _viewModel.PlatformClick;
                _viewModel.ChildPageMap.Add(ctr, null);
                panel.Children.Add(ctr);
            }

            GamePlatform.Instance.OnPlatformAdded += PlatformAdded;
            GamePlatform.Instance.OnPlatformRemoved += PlatformRemoved;
            Loaded += NewHomePageView_Loaded;
        }

        private void NewHomePageView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetButtonSize();

            Task.Run(async () =>
            {
                GamePlatform.Instance.GetAppList();
                await GamePlatform.Instance.GetUwpApp();
                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    int index = 0;

                    double width = PanelGrid.ActualWidth / 5d;  //可以调整主界面初始化框的大小
                    double height = width * COEFFICIENT_HEIGHT;

                    _viewModel.ListItems.ForEach(p =>
                    {
                        p.Index = index++;

                        if (p is DynamicButtonControlHV item)
                        {
                            item.Width = width;
                            item.Height = height;
                            item.BorderCornerRadius = _viewModel.CornerRadius;
                            item.SetButtonEffect(item.IsSelected, item.IsHoved);

                            if (int.TryParse(item.Tag.ToString(), out int tag))
                            {
                                var platform = GamePlatform.Instance.GetPlatformModel((PlatformEnum)tag);

                                if (platform != null)
                                {
                                    item.Text = platform.IsInstall ? platform.Name : $"{platform.Name}({_viewModel.GetString("Unload")})";
                                    item.ImagePath = platform.IsInstall ? INSTALL_BG : UNINSTALL_BG;
                                }
                            }
                        }
                    });
                }));
            });
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.OnChildPageNavigation += navigationParent.ChildPageNavgation;
            _viewModel.OnTipButtomMapChanged = navigationParent.OnTipButtonMapChanged;
            _viewModel.Initialization();

            _viewModel.ChildPageMap[GamePlatformButton].Init(navigationParent);

            return this;
        }

        private void PlatformAdded(PlatformEnum platformEnum)
        {
            var platform = GamePlatform.Instance.GetPlatformModel(platformEnum);

            double width = PanelGrid.ActualWidth / 4d;
            double height = width * COEFFICIENT_HEIGHT;

            DynamicButtonControlHV ctr = new DynamicButtonControlHV()
            {
                BorderCornerRadius = 1,
                Width = width,
                Height = height,
                SelectedExpend = 4,
                Margin = new Thickness(0),
                ImagePath = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_LH.png",
                IconPath = platform.Icon,
                Index = _viewModel.ChildPageMap.Count - 1,
                Tag = platform.Platform,
                Text = platform.Name
            };

            if (platform.Platform == PlatformEnum.STEAM)
            {
                ctr.ImagePath = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_LH_3.png";
            }
            else
            {
                ctr.ImagePath = "pack://SiteOfOrigin:,,,/Resource/Image/Button_bg_LH.png";
            }

            ctr.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            ctr.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            ctr.OnClick += _viewModel.PlatformClick;
            _viewModel.ChildPageMap.Add(ctr, null);
            _viewModel.ListItems.Add(ctr);

            panel.Children.Add(ctr);
        }

        private void PlatformRemoved(PlatformEnum platformEnum)
        {
            DynamicButtonControlHV tmp_ctr = null;
            int index = 0;
            int ix = 0;

            foreach (var item in _viewModel.ListItems)
            {
                if (item is DynamicButtonControlHV hv)
                {
                    var tmp_platform = GamePlatform.Instance.GetPlatformModel((PlatformEnum)hv.Tag);
                    if (GamePlatform.Instance.HomePlatforms.Contains(tmp_platform))
                    {
                        hv.Index = index++;
                    }
                    else
                    {
                        ix = index;
                        tmp_ctr = hv;
                    }
                }
            }

            _viewModel.ChildPageMap.Remove(tmp_ctr);
            _viewModel.ListItems.Remove(tmp_ctr);
            panel.Children.Remove(tmp_ctr);
            tmp_ctr.OnSelectedStateChange -= _viewModel.OnButtonSelectedStateChange;
            tmp_ctr.OnHovedStateChange -= _viewModel.OnButtonHovedStateChange;
            tmp_ctr.OnClick -= _viewModel.PlatformClick;

            if (_viewModel.ListItems.Count > 0)
            {
                ix = ix >= _viewModel.ListItems.Count ? (_viewModel.ListItems.Count - 1) : ix;
                _viewModel.ListItems[ix].IsHoved = true;
                _viewModel.ListItems[ix].IsSelected = true;
            }
            else
            {
                _viewModel.TopButton.IsHoved = true;
                _viewModel.TopButton.IsSelected = true;
                _viewModel.CurrentItem = _viewModel.TopButton;
                _viewModel.HovedItem = _viewModel.TopButton;
            }
        }
    }
}
