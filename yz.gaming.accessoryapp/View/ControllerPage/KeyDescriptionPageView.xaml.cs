using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
using yz.gaming.accessoryapp.Utils;
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.ControllerPage;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.View.ControllerPage
{
    /// <summary>
    /// KeyDescription.xaml 的交互逻辑
    /// </summary>
    public partial class KeyDescriptionPageView : Page, IPageViewInterface
    {
        KeyDescriptionPageViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;

        public KeyDescriptionPageView()
        {
            InitializeComponent();

            _viewModel = Ioc.Default.GetRequiredService<KeyDescriptionPageViewModel>();
            DataContext = _viewModel;

            ThirdRadio.LeftElement = JoystickMode;
            ThirdRadio.CenterElement = MouKbdMode;
            ThirdRadio.RightElement = CombinationKey;
            ThirdRadio.M1M2Element = M1AndM2;
            ThirdRadio.OnSelectedElementChanged += ThirdRadio_OnSelectedElementChanged;

            _viewModel.MenuList = new List<Grid>()
            {
                JoystickMode,
                MouKbdMode,
                CombinationKey,
                M1AndM2
            };

            M1Delay1.SetSource(_viewModel.DelayMs);
            M1Delay2.SetSource(_viewModel.DelayMs);
            M1Delay3.SetSource(_viewModel.DelayMs);
            M2Delay1.SetSource(_viewModel.DelayMs);
            M2Delay2.SetSource(_viewModel.DelayMs);
            M2Delay3.SetSource(_viewModel.DelayMs);

            _viewModel.MacroItems = new List<ISelectableItem>()
            {
                M1Key1,
                M1Delay1,
                M1Key2,
                M1Delay2,
                M1Key3,
                M1Delay3,
                M1Key4,
                M2Key1,
                M2Delay1,
                M2Key2,
                M2Delay2,
                M2Key3,
                M2Delay3,
                M2Key4
            };

            _viewModel.OnSelectedMenuChanged += OnSelectedMenuChanged;

            ThirdRadio.SelectElement = ThirdTabControl.SelectElementEnum.LeftElement;

            this.Loaded += KeyDescriptionPageView_Loaded;
        }

        private void KeyDescriptionPageView_Loaded(object sender, RoutedEventArgs e)
        {
            ThirdRadio.SelectElement = ThirdTabControl.SelectElementEnum.LeftElement;
            _viewModel.CurrentIndex = 0;

            int index = 0;

            M1Key1.Key = _viewModel.Model.BackKey.M1.Step.Key;
            M1Key1.Text = EnumUtility.GetName((MacroMappingEnum)_viewModel.Model.BackKey.M1.Step.Key);
            M1Delay1.SelectedItem = _viewModel.DelayMs.FirstOrDefault(p => (ushort)p.Value == _viewModel.Model.BackKey.M1.Step.Interval);
            MacroStep M1Next = _viewModel.Model.BackKey.M1.Step.Next;
            index = 2;
            while (index <= 6 && M1Next != null)
            {
                (_viewModel.MacroItems[index] as SelectableTextBlock).Key = M1Next.Key;
                (_viewModel.MacroItems[index++] as SelectableTextBlock).Text = EnumUtility.GetName((MacroMappingEnum)M1Next.Key);
                if (index < 6)
                {
                    (_viewModel.MacroItems[index++] as SelectableComboBox).SelectedItem = _viewModel.DelayMs.FirstOrDefault(p => (ushort)p.Value == M1Next.Interval);
                }

                M1Next = M1Next.Next;
            }

            M2Key1.Key = _viewModel.Model.BackKey.M2.Step.Key;
            M2Key1.Text = EnumUtility.GetName((MacroMappingEnum)_viewModel.Model.BackKey.M2.Step.Key);
            M2Delay1.SelectedItem = _viewModel.DelayMs.FirstOrDefault(p => (ushort)p.Value == _viewModel.Model.BackKey.M2.Step.Interval);
            MacroStep M2Next = _viewModel.Model.BackKey.M2.Step.Next;
            index = 9;
            while (index <= 13 && M2Next != null)
            {
                (_viewModel.MacroItems[index] as SelectableTextBlock).Key = M2Next.Key;
                (_viewModel.MacroItems[index++] as SelectableTextBlock).Text = EnumUtility.GetName((MacroMappingEnum)M2Next.Key);
                if (index < 13)
                {
                    (_viewModel.MacroItems[index++] as SelectableComboBox).SelectedItem = _viewModel.DelayMs.FirstOrDefault(p => (ushort)p.Value == M2Next.Interval);
                }

                M2Next = M2Next.Next;
            }

            _viewModel.MacroItems.ForEach(p =>
            {
                p.IsSelected = false;
                p.IsHover = false;
            });
        }

        private void ThirdRadio_OnSelectedElementChanged(ThirdTabControl sender, object element)
        {
            JoystickMode.Visibility = Visibility.Hidden;
            MouKbdMode.Visibility = Visibility.Hidden;
            CombinationKey.Visibility = Visibility.Hidden;
            M1AndM2.Visibility = Visibility.Hidden;

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
                ThirdRadio.SelectElement = ThirdTabControl.SelectElementEnum.LeftElement;
            }
            else if (grid.Equals(MouKbdMode))
            {
                ThirdRadio.SelectElement = ThirdTabControl.SelectElementEnum.CenterElement;
            }
            else if (grid.Equals(CombinationKey))
            {
                ThirdRadio.SelectElement = ThirdTabControl.SelectElementEnum.RightElement;
            }
            else if (grid.Equals(M1AndM2))
            {
                ThirdRadio.SelectElement = ThirdTabControl.SelectElementEnum.M1M2Element;
            }
        }

        private void OnKeyItemSelected(ISelectableItem item)
        {
            if (item.IsSelected)
            {
                _viewModel.MacroItems.ForEach(p => 
                {
                    if (!p.Equals(item))
                    {
                        p.IsSelected = false;
                        p.IsHover = false;
                    }
                });
                item.IsHover = true;
                _viewModel.SelectedKeyItem = item;
                _viewModel.HoverItemIndex = item.Index;
            }
            else
            {
                _viewModel.SelectedKeyItem = null;
            }
        }
    }
}
