using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Utils;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class CalibrationAndAdvancedSettingPageViewModel : JoystickSelectionViewModelBase, ITipButtomMapSupport, IMenuSupport
    {
        static Thickness DEFAULT_SENSITIVITY_MARGIN = new Thickness(0, 32, 0, 0);
        static Thickness DEFAULT_HEADZOOM_MARGIN = new Thickness(0, 32, 0, 0);
        static Thickness DEFAULT_CALI_MARGIN = new Thickness(0, 48, 0, 0);

        public event Action<Grid> OnSelectedMenuChanged;

        public List<bool> TipButtomMap => new List<bool> { true, false, true, true, true, false, true, false, false };

        byte _lStickSensitivity;
        public byte LStickSensitivity
        {
            get => _lStickSensitivity;
            set
            {
                SetProperty(ref _lStickSensitivity, value);
                Model.LStickSensitivity = value;
                Model.RStickSensitivity = value;
                SaveProfile();
            }
        }

        byte _rStickSensitivity;
        public byte RStickSensitivity
        {
            get => _rStickSensitivity;
            set
            {
                SetProperty(ref _rStickSensitivity, value);
            }
        }

        byte _lStickHeadZoomValue;
        public byte LStickHeadZoomValue
        {
            get => _lStickHeadZoomValue;
            set
            {
                SetProperty(ref _lStickHeadZoomValue, value);
                Model.LStickHeadZoomValue = value;
                Model.RStickHeadZoomValue = value;
                SaveProfile();
            }
        }

        byte _rStickHeadZoomValue;
        public byte RStickHeadZoomValue
        {
            get => _rStickHeadZoomValue;
            set
            {
                SetProperty(ref _rStickHeadZoomValue, value);
            }
        }

        byte _lTriggerSensitivity;
        public byte LTriggerSensitivity
        {
            get => _lTriggerSensitivity;
            set
            {
                SetProperty(ref _lTriggerSensitivity, value);
                Model.LTriggerSensitivity = value;
                Model.RTriggerSensitivity = value;
                SaveProfile();
            }
        }

        byte _rTriggerSensitivity;
        public byte RTriggerSensitivity
        {
            get => _rTriggerSensitivity;
            set
            {
                SetProperty(ref _rTriggerSensitivity, value);
            }
        }

        Thickness _sensitivityMargin;
        public Thickness SensitivityMargin
        {
            get => _sensitivityMargin;
            set
            {
                SetProperty(ref _sensitivityMargin, value);
            }
        }

        Thickness _headZoomMargin;
        public Thickness HeadZoomMargin
        {
            get => _headZoomMargin;
            set
            {
                SetProperty(ref _headZoomMargin, value);
            }
        }

        Thickness _caliMargin;
        public Thickness CaliMargin
        {
            get => _caliMargin;
            set
            {
                SetProperty(ref _caliMargin, value);
            }
        }

        public List<Grid> MenuList { get; set; }

        public Grid CurrentMenu { get; set; }
        public int CurrentIndex { get; set; } = 0;

        public CenterSliderListItem RightSliderItem { get; set; }
        public CenterSliderListItem CurrentSlider
        {
            get
            {
                return CurrentMenu.Equals(MenuList[1]) ? RightSliderItem : (CenterSliderListItem)CurrentItem;
            }
        }

        public CalibrationAndAdvancedSettingPageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            base.Initialization();
            Title = GetString("CalibrationAndAdvancedSettings");
            SetProperty(ref _lStickSensitivity, Model.LStickSensitivity, nameof(LStickSensitivity));
            SetProperty(ref _rStickSensitivity, Model.RStickSensitivity, nameof(RStickSensitivity));
            SetProperty(ref _lStickHeadZoomValue, Model.LStickHeadZoomValue, nameof(LStickHeadZoomValue));
            SetProperty(ref _rStickHeadZoomValue, Model.RStickHeadZoomValue, nameof(RStickHeadZoomValue));
            SetProperty(ref _lTriggerSensitivity, Model.LTriggerSensitivity, nameof(LTriggerSensitivity));
            SetProperty(ref _rTriggerSensitivity, Model.RTriggerSensitivity, nameof(RTriggerSensitivity));

            double scaling = SystemUtils.Instance.GetScreenScalingFactor();

            if (scaling != 1)
            {
                SensitivityMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_SENSITIVITY_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_SENSITIVITY_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_SENSITIVITY_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_SENSITIVITY_MARGIN.Bottom / scaling))
                };
                HeadZoomMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_HEADZOOM_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_HEADZOOM_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_HEADZOOM_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_HEADZOOM_MARGIN.Bottom / scaling))
                };
                CaliMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_CALI_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_CALI_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_CALI_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_CALI_MARGIN.Bottom / scaling))
                };
            }
            else
            {
                SensitivityMargin = DEFAULT_SENSITIVITY_MARGIN;
                HeadZoomMargin = DEFAULT_HEADZOOM_MARGIN;
                CaliMargin = DEFAULT_CALI_MARGIN;
            }
        }

        public void MenuSelect(int index)
        {
            if (index >= 0 && index < MenuList.Count)
            {
                OnSelectedMenuChanged?.Invoke(MenuList[index]);
            }
        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            switch (key)
            {
                case KeyCodeEnum.L1:
                    OnSelectedMenuChanged?.Invoke(MenuList[0]);
                    break;
                case KeyCodeEnum.R1:
                    OnSelectedMenuChanged?.Invoke(MenuList[1]);
                    break;
                case KeyCodeEnum.DPAD_LEFT:
                    if (CurrentSlider.IsSelected && CurrentSlider.IsHoved)
                    {
                        CurrentSlider.Value = CurrentSlider.Value - 1;
                    }
                    break;
                case KeyCodeEnum.DPAD_RIGHT:
                    if (CurrentSlider.IsSelected && CurrentSlider.IsHoved)
                    {
                        CurrentSlider.Value = CurrentSlider.Value + 1;
                    }
                    break;
                case KeyCodeEnum.DPAD_UP:
                case KeyCodeEnum.DPAD_DOWN:
                    if (CurrentMenu.Equals(MenuList[0]))
                    {
                        base.HandleKeyEvent(key, type);
                    }
                    break;
                default:
                    base.HandleKeyEvent(key, type);
                    break;
            }
        }

        public override void HandleThumbStatusEvent(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            switch (direction)
            {
                case ThumbDirectionEnmu.LEFT:
                    if (CurrentSlider.IsSelected && CurrentSlider.IsHoved)
                    {
                        CurrentSlider.Value = CurrentSlider.Value - 1;
                    }
                    break;
                case ThumbDirectionEnmu.RIGHT:
                    if (CurrentSlider.IsSelected && CurrentSlider.IsHoved)
                    {
                        CurrentSlider.Value = CurrentSlider.Value + 1;
                    }
                    break;
                case ThumbDirectionEnmu.UP:
                case ThumbDirectionEnmu.DOWN:
                    if (CurrentMenu.Equals(MenuList[0]))
                    {
                        base.HandleThumbStatusEvent(key, direction);
                    }
                    break;
                default:
                    base.HandleThumbStatusEvent(key, direction);
                    break;
            }
        }
    }
}
