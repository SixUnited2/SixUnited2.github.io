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
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.ControllerPage;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.View.ControllerPage
{
    /// <summary>
    /// LightEffectPageView.xaml 的交互逻辑
    /// </summary>
    public partial class LightEffectPageView : Page, IPageViewInterface
    {
        LightEffectPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;
        public LightEffectPageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<LightEffectPageViewModel>();
            DataContext = _viewModel;

            _viewModel.ListItems = new List<IPageListItem>
            {
                Brightness,
                LightEffect,
                SingleColor
            };

            Brightness.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            LightEffect.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            SingleColor.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            Brightness.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            LightEffect.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            SingleColor.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;

            Brightness.OnClick += _viewModel.OnButtonClick;
            LightEffect.OnClick += _viewModel.OnButtonClick;
            SingleColor.OnClick += _viewModel.OnButtonClick;

            Brightness.OnSliderListItemValueChanged += OnCenterSliderListItemValueChanged;
            LightEffect.OnValueChangedHandler += OnValueChangedHandler;
            SingleColor.OnColorChanged += OnColorChanged;

            this.Loaded += LightEffectPageView_Loaded;
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();
            return this;
        }

        private void LightEffectPageView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetButtonSize(LightEffectContainer.ActualWidth);
            LightEffect.ButtonHeight = _viewModel.ButtonHeight;
            LightEffect.ButtonWidth = _viewModel.ButtonWidth;
            switch (_viewModel.LightType)
            {
                case LightTypeEnum.MonoBright:
                    LightEffect.SelectIndex = 3;
                    break;
                case LightTypeEnum.MonoBreath:
                    LightEffect.SelectIndex = 4;
                    break;
                case LightTypeEnum.RGBBreath:
                    LightEffect.SelectIndex = 2;
                    break;
                case LightTypeEnum.RGBCycle:
                    LightEffect.SelectIndex = 1;
                    break;
                case LightTypeEnum.RGBWave:
                    LightEffect.SelectIndex = 0;
                    break;
            }
            SingleColor.SetColor(_viewModel.LightColor);

            _viewModel.ListItems.ForEach(p => p.SetButtonEffect(false, false));
            Brightness.IsSelected = true;
            Brightness.IsHoved = true;
            Brightness.Value = (int)_viewModel.LightIntensity;
        }

        private void OnCenterSliderListItemValueChanged(IPageListItem sender, int value)
        {
            _viewModel.LightIntensity = (byte)value;
        }

        private void OnValueChangedHandler(IPageListItem sender, int index)
        {
            switch (index)
            {
                case 0:
                    _viewModel.LightType = LightTypeEnum.RGBWave;
                    break;
                case 1:
                    _viewModel.LightType = LightTypeEnum.RGBCycle;
                    break;
                case 2:
                    _viewModel.LightType = LightTypeEnum.RGBBreath;
                    break;
                case 3:
                    _viewModel.LightType = LightTypeEnum.MonoBright;
                    break;
                case 4:
                    _viewModel.LightType = LightTypeEnum.MonoBreath;
                    break;
            }
        }

        private void OnColorChanged(IPageListItem sender, Color color)
        {
            _viewModel.LightColor = color;
        }
    }
}
