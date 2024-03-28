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
    /// CalibrationAndAdvancedSettingPageView.xaml 的交互逻辑
    /// </summary>
    public partial class CalibrationAndAdvancedSettingPageView : Page, IPageViewInterface
    {
        CalibrationAndAdvancedSettingPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public CalibrationAndAdvancedSettingPageView()
        {
            InitializeComponent();
            _viewModel = Ioc.Default.GetRequiredService<CalibrationAndAdvancedSettingPageViewModel>();
            DataContext = _viewModel;

            DoubleTab.LeftElement = Stick;
            DoubleTab.RightElement = Trigger;
            DoubleTab.OnSelectedElementChanged += DoubleTab_OnSelectedElementChanged;
            DoubleTab.SelectElement = DoubleTabControl.SelectElementEnum.LeftElement;

            _viewModel.MenuList = new List<Grid>()
            {
                Stick,
                Trigger
            };

            _viewModel.ListItems = new List<IPageListItem>()
            {
                StickSensitivitiesSlider,
                DeadzoneSlider
            };

            _viewModel.RightSliderItem = TriggerSensitivitiesSlider;
            _viewModel.OnSelectedMenuChanged += OnSelectedMenuChanged;

            StickSensitivitiesSlider.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            DeadzoneSlider.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            StickSensitivitiesSlider.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            DeadzoneSlider.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;

            TriggerSensitivitiesSlider.CenterSliderListItemValueChanged += CenterSliderListItemValueChanged;
            StickSensitivitiesSlider.CenterSliderListItemValueChanged += CenterSliderListItemValueChanged;
            DeadzoneSlider.CenterSliderListItemValueChanged += CenterSliderListItemValueChanged;

            DeadzoneSlider.TextFormat = @"{0}%";

            this.Loaded += CalibrationAndAdvancedSettingPageView_Loaded;
        }

        private void CenterSliderListItemValueChanged(IPageListItem sender, int value)
        {
            switch (sender.Index)
            {
                case 0:
                    _viewModel.LStickSensitivity = Convert.ToByte(value);
                    break;
                case 1:
                    _viewModel.LStickHeadZoomValue = Convert.ToByte(value);
                    break;
                case 2:
                    _viewModel.LTriggerSensitivity = Convert.ToByte(value);
                    break;
            }
        }

        private void CalibrationAndAdvancedSettingPageView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Initialization();
            _viewModel.UnSelectAllItem();
            StickSensitivitiesSlider.IsHoved = true;
            TriggerSensitivitiesSlider.Value = _viewModel.LTriggerSensitivity;
            StickSensitivitiesSlider.Value = _viewModel.LStickSensitivity;
            DeadzoneSlider.Value = _viewModel.LStickHeadZoomValue;
            DoubleTab.SelectElement = DoubleTabControl.SelectElementEnum.LeftElement;

            _viewModel.CurrentMenu = Stick;
            _viewModel.CurrentIndex = Convert.ToInt32(_viewModel.CurrentMenu.Tag);
        }

        public IPageViewInterface Init(INavigationSupport navigationParent)
        {
            _viewModel.Initialization();
            return this;
        }

        private void DoubleTab_OnSelectedElementChanged(DoubleTabControl sender, object element)
        {
            Stick.Visibility = Visibility.Hidden;
            Trigger.Visibility = Visibility.Hidden;

            _viewModel.CurrentMenu = (Grid)element;
            _viewModel.CurrentIndex = Convert.ToInt32(_viewModel.CurrentMenu.Tag);
            _viewModel.CurrentMenu.Visibility = Visibility.Visible;

            if (element.Equals(Stick))
            {
                StickSensitivitiesSlider.IsSelected = true;
                StickSensitivitiesSlider.IsHoved = true;

                _viewModel.HovedItem = StickSensitivitiesSlider;
                _viewModel.CurrentItem = StickSensitivitiesSlider;
            }
            else
            {
                TriggerSensitivitiesSlider.IsSelected = true;
                TriggerSensitivitiesSlider.IsHoved = true;

                _viewModel.HovedItem = TriggerSensitivitiesSlider;
                _viewModel.CurrentItem = TriggerSensitivitiesSlider;
                TriggerSensitivitiesSlider.ConfirmPressed();
            }
        }

        private void OnSelectedMenuChanged(Grid grid)
        {
            if (grid.Equals(Stick))
            {
                DoubleTab.SelectElement = DoubleTabControl.SelectElementEnum.LeftElement;
            }
            else
            {
                DoubleTab.SelectElement = DoubleTabControl.SelectElementEnum.RightElement;
            }
        }
    }
}
