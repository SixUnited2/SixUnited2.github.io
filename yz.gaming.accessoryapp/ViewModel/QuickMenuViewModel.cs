using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Service;
using yz.gaming.accessoryapp.Utils;
using static yz.gaming.accessoryapp.Api.YzCommonApi;
using static yz.gaming.accessoryapp.Utils.SystemUtils;

namespace yz.gaming.accessoryapp.ViewModel
{
    public class QuickMenuViewModel : ListItemSupportViewModelBase
    {
        static Thickness DEFAULT_TITLE_MARGIN = new Thickness(12, 8, 0, 8);

        Delegate _handleKeyEvent;
        Delegate _handleThumbStatusEvent;

        public bool IsHandleDpadKey { get; set; }

        public List<ItemModel> Resolution { get; set; }
        public List<ItemModel> RefresRate { get; set; }
        public List<ItemModel> ScaleRate { get; set; }

        public Action VolumeChanged { get; set; }

        Byte _controllerMode;
        public Byte ControllerMode
        {
            get => _controllerMode;
            set
            {
                SetProperty(ref _controllerMode, value);

                if (Model.ControllerMode != value)
                {
                    Model.ControllerMode = value;
                    SaveProfile();
                }
            }
        }

        byte _motorIntensity;
        public byte MotorIntensity
        {
            get => _motorIntensity;
            set
            {
                SetProperty(ref _motorIntensity, value);

                if (Model.MotorIntensity != value)
                {
                    Model.MotorIntensity = value;
                    SaveProfile();
                }
            }
        }

        bool _lightIntensity;
        public bool LightIntensity
        {
            get => _lightIntensity;
            set
            {
                SetProperty(ref _lightIntensity, value);

                if (Model.LightIntensity != (byte)(value ? 5 : 1))
                {
                    Model.LightIntensity = (byte)(value ? 5 : 1);
                    SaveProfile();
                }
            }
        }

        bool _gyroEmulate;
        public bool GyroEmulate
        {
            get => _gyroEmulate;
            set
            {
                SetProperty(ref _gyroEmulate, value);
            }
        }

        bool _turboOpen;
        public bool TurboOpen
        {
            get => _turboOpen;
            set
            {
                SetProperty(ref _turboOpen, value);

                if (Model.TurboOpen != (byte)(value ? 1 : 0))
                {
                    Model.TurboOpen = (byte)(value ? 1 : 0);
                    SaveProfile();
                }
            }
        }

        private TdpUtils.TdpMode _performanceMode;
        public TdpUtils.TdpMode PerformanceMode
        {
            get => _performanceMode;
            set
            {
                SetProperty(ref _performanceMode, value);
                Performance = value;
            }
        }

        private TdpUtils.TdpMode _performance;
        public TdpUtils.TdpMode Performance
        {
            get => _performance;
            set
            {
                SetProperty(ref _performance, value);
            }
        }

        bool _statusMonitoring;
        public bool StatusMonitoring
        {
            get => _statusMonitoring;
            set
            {
                SetProperty(ref _statusMonitoring, value);
            }
        }

        private int _brightness;
        public int Brightness
        {
            get => _brightness;
            set
            {
                SetProperty(ref _brightness, value);
            }
        }

        private int _resolutionIndex;
        public int ResolutionIndex
        {
            get => _resolutionIndex;
            set
            {
                SetProperty(ref _resolutionIndex, value);
            }
        }

        private int _refresRateIndex;
        public int RefresRateIndex
        {
            get => _refresRateIndex;
            set
            {
                SetProperty(ref _refresRateIndex, value);
            }
        }

        private double _speakerVolume;
        public double SpeakerVolume
        {
            get => _speakerVolume;
            set
            {
                if (SetProperty(ref _speakerVolume, value))
                {
                    AudioUtils.Instance.SpeakerVolume = value;
                }
            }
        }

        private double _microphoneVolume;
        public double MicrophoneVolume
        {
            get => _microphoneVolume;
            set
            {
                if (SetProperty(ref _microphoneVolume, value))
                {
                    AudioUtils.Instance.MicrophoneVolume = value;
                }
            }
        }

        Thickness _titleMargin;
        public Thickness TitleMargin
        {
            get => _titleMargin;
            set
            {
                SetProperty(ref _titleMargin, value);
            }
        }

        public QuickMenuViewModel()
        {
            _handleKeyEvent = new Action<KeyCodeEnum, KeyPressTypeEnmu>(HandleKeyEvent);
            _handleThumbStatusEvent = new Action<ThumbKeyEnmu, ThumbDirectionEnmu>(HandleThumbStatusEvent);

            YzGamingService.Instance.OnDpadKeyPress -= OnDpadKeyPress;
            YzGamingService.Instance.OnDpadKeyPress += OnDpadKeyPress;
            YzGamingService.Instance.OnThumbStatusReport -= OnThumbStatusReport;
            YzGamingService.Instance.OnThumbStatusReport += OnThumbStatusReport;

            AudioUtils.Instance.OnMicrophoneVolumeChanged += p =>
            {
                SetProperty(ref _speakerVolume, p, nameof(SpeakerVolume));
                VolumeChanged();
            };
            AudioUtils.Instance.OnSpeakerVolumeChanged += p =>
            {
                SetProperty(ref _microphoneVolume, p, nameof(MicrophoneVolume));
                VolumeChanged();
            }; 

            ScaleRate = GetScaleRateList();
        }

        public override void Initialization()
        {
            base.Initialization();

            SetProperty(ref _controllerMode, Model.ControllerMode, nameof(ControllerMode));
            SetProperty(ref _motorIntensity, Model.MotorIntensity, nameof(MotorIntensity));
            SetProperty(ref _lightIntensity, Model.LightIntensity > 0, nameof(LightIntensity));
            SetProperty(ref _gyroEmulate, false, nameof(GyroEmulate));
            SetProperty(ref _turboOpen, Model.TurboOpen > 0, nameof(TurboOpen));

            var tdp = TdpUtils.Instance.GetTdp();
            SetProperty(ref _performanceMode, tdp, nameof(PerformanceMode));
            SetProperty(ref _performance, _performanceMode, nameof(Performance));
            SetProperty(ref _statusMonitoring, false, nameof(StatusMonitoring));

            SetProperty(ref _brightness, SystemUtils.Instance.GetBrightness());
            //SetProperty(ref _brightness, 50, nameof(Brightness));

            var current = new Pixels()
            {
                Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height,
                Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width
            };
            var select = Resolution.Where(p => (Pixels)p.Value == current).FirstOrDefault();
            var index = select == null ? 0 : select.Index;

            SetProperty(ref _resolutionIndex, index, nameof(ResolutionIndex));
            SetProperty(ref _refresRateIndex, 0, nameof(RefresRateIndex));

            SetProperty(ref _speakerVolume, AudioUtils.Instance.SpeakerVolume, nameof(SpeakerVolume));
            SetProperty(ref _microphoneVolume, AudioUtils.Instance.MicrophoneVolume, nameof(MicrophoneVolume));

            double scaling = SystemUtils.Instance.GetScreenScalingFactor();

            if (scaling != 1)
            {
                TitleMargin = new Thickness()
                {
                    Left = Convert.ToInt32(Math.Ceiling(DEFAULT_TITLE_MARGIN.Left / scaling)),
                    Top = Convert.ToInt32(Math.Ceiling(DEFAULT_TITLE_MARGIN.Top / scaling)),
                    Right = Convert.ToInt32(Math.Ceiling(DEFAULT_TITLE_MARGIN.Right / scaling)),
                    Bottom = Convert.ToInt32(Math.Ceiling(DEFAULT_TITLE_MARGIN.Bottom / scaling))
                };
            }
            else
            {
                TitleMargin = DEFAULT_TITLE_MARGIN;
            }
        }

        public List<ItemModel> GetScaleRateList()
        {
            return new List<ItemModel>()
            {
                new ItemModel() { Index = 0, Value = 1.00, Text = "100%" },
                new ItemModel() { Index = 1, Value = 1.25, Text = "125%" },
                new ItemModel() { Index = 2, Value = 1.50, Text = "150%" },
                new ItemModel() { Index = 3, Value = 1.75, Text = "175%" },
                new ItemModel() { Index = 4, Value = 2.00, Text = "200%" }
            };
        }

        public ItemModel GetScaleRate(double scale)
        {
            var s = ScaleRate.Where(p => (double)p.Value == scale).FirstOrDefault();
            if (s == null) s = ScaleRate[0];
            return s;
        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            try
            {
                if (!YzGamingService.Instance.IsQuickMenuShown)
                {
                    return;
                }

                switch (key)
                {
                    case KeyCodeEnum.Quick:
                    case KeyCodeEnum.B:
                        if (type == KeyPressTypeEnmu.ShortPress || type == KeyPressTypeEnmu.AppClick)
                        {
                            if (CurrentItem is QuickMenuComboBox comboBox && comboBox.IsOpen)
                            {
                                base.HandleKeyEvent(key, type);
                            }
                            else
                            {
                                YzGamingService.Instance.HideQuickMenu();
                            }
                        }
                        break;
                    case KeyCodeEnum.DPAD_LEFT:
                        if (CurrentItem.IsSelected && CurrentItem is QuickMenuComboBox) return;
                        if (CurrentItem is QuickMenuSlider slider &&
                            slider.IsSelected && slider.IsHoved)
                        {
                            slider.Value = slider.Value - 1;
                        }
                        else if (CurrentItem is QuickMenuThirdRadio thirdRadio &&
                            thirdRadio.IsSelected && thirdRadio.IsHoved)
                        {
                            thirdRadio.SelectPrev();
                        }
                        else
                        {
                            if (HovedItem.Index < 4)
                            {
                                MoveToItem(HovedItem.Index + 7);
                            }
                        }
                        break;
                    case KeyCodeEnum.DPAD_RIGHT:
                        if (CurrentItem.IsSelected && CurrentItem is QuickMenuComboBox) return;
                        if (CurrentItem is QuickMenuSlider slider1 &&
                            slider1.IsSelected && slider1.IsHoved)
                        {
                            slider1.Value = slider1.Value + 1;
                        }
                        else if (CurrentItem is QuickMenuThirdRadio thirdRadio &&
                            thirdRadio.IsSelected && thirdRadio.IsHoved)
                        {
                            thirdRadio.SelectNext();
                        }
                        else
                        {
                            if (HovedItem.Index > 6)
                            {
                                MoveToItem(HovedItem.Index - 4);
                            }
                            else if (HovedItem.Index > 3 && HovedItem.Index < 6)
                            {
                                MoveToItem(0);
                            }
                        }
                        break;
                    case KeyCodeEnum.A:
                        if (HovedItem != null && HovedItem is QuickMenuButton button)
                        {
                            button.ConfirmPressed();
                        }
                        else
                        {
                            base.HandleKeyEvent(key, type);
                        }
                        break;
                    case KeyCodeEnum.X:
                        if (CurrentItem is QuickMenuSlider slider2 &&
                            slider2.IsSelected && slider2.IsHoved)
                        {
                            slider2.RollbackValue();
                        }
                        break;
                    default:
                        base.HandleKeyEvent(key, type);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        public override void HandleThumbStatusEvent(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            try
            {
                switch (direction)
                {
                    case ThumbDirectionEnmu.LEFT:
                        if (CurrentItem.IsSelected && CurrentItem is QuickMenuComboBox) return;
                        if (CurrentItem is QuickMenuSlider slider &&
                            slider.IsSelected && slider.IsHoved)
                        {
                            slider.Value = slider.Value - 1;
                        }
                        else if (CurrentItem is QuickMenuThirdRadio thirdRadio &&
                            thirdRadio.IsSelected && thirdRadio.IsHoved)
                        {
                            thirdRadio.SelectPrev();
                        }
                        else 
                        {
                            if (HovedItem.Index < 4)
                            {
                                MoveToItem(HovedItem.Index + 7);
                            }
                        }
                        break;
                    case ThumbDirectionEnmu.RIGHT:
                        if (CurrentItem.IsSelected && CurrentItem is QuickMenuComboBox) return;
                        if (CurrentItem is QuickMenuSlider slider1 &&
                            slider1.IsSelected && slider1.IsHoved)
                        {
                            slider1.Value = slider1.Value + 1;
                        }
                        else if (CurrentItem is QuickMenuThirdRadio thirdRadio &&
                            thirdRadio.IsSelected && thirdRadio.IsHoved)
                        {
                            thirdRadio.SelectNext();
                        }
                        else
                        {
                            if (HovedItem.Index > 6)
                            {
                                MoveToItem(HovedItem.Index - 4);
                            }
                            else if (HovedItem.Index > 3 && HovedItem.Index < 6)
                            {
                                MoveToItem(0);
                            }
                        }
                        break;
                    default:
                        base.HandleThumbStatusEvent(key, direction);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        private void OnDpadKeyPress(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            if (IsHandleDpadKey)
            {
                Application.Current.Dispatcher.BeginInvoke(_handleKeyEvent, new object[] { key, type });
            }
        }

        private void OnThumbStatusReport(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            if (IsHandleDpadKey)
            {
                Application.Current.Dispatcher.BeginInvoke(_handleThumbStatusEvent, new object[] { key, direction });
            }
        }
    }
}
