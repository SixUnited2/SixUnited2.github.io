using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Service;
using yz.gaming.accessoryapp.Utils;
using yz.gaming.accessoryapp.View.ControllerPage;
using yz.gaming.accessoryapp.View.Main;
using yz.gaming.accessoryapp.ViewModel;
using static yz.gaming.accessoryapp.Utils.SystemUtils;

namespace yz.gaming.accessoryapp
{
    /// <summary>
    /// QuickMenu.xaml 的交互逻辑
    /// </summary>
    public partial class QuickMenu : Window
    {
        QuickMenuViewModel _viewModel = null;
        public IViewModel ViewModel => _viewModel;
        Dictionary<Pixels, List<Frequency>> _freqs;
        TimeSpan _lastShownTime;

        int _tmpVolume = 0;

        public QuickMenu()
        {
            InitializeComponent();
            _viewModel = Ioc.Default.GetRequiredService<QuickMenuViewModel>();
            DataContext = _viewModel;

            _viewModel.ListItems = new List<IPageListItem>
            { 
                TaskManager,
                CutScreen,
                //ESC,
                Mute,
                //QuitGame,
                KeyDescription,
                ScreenKeyboard,
                PerformanceMode,
                //Custom,
                //StatusMonitoring,
                ControllerMode,
                Shake,
                //Light,
                //GyroEmulate,
                RepeatMode,
                Brightness,
                Resolution,
                //ScaleRate,
                //RefresRate,
                Speaker
                //Microphone
            };

            //TaskManager.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            //CutScreen.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            //ESC.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            //Mute.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            //QuitGame.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            //KeyDescription.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            PerformanceMode.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Custom.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            StatusMonitoring.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            ControllerMode.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Shake.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Light.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            GyroEmulate.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RepeatMode.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Brightness.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Resolution.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            RefresRate.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            ScaleRate.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Speaker.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;
            Microphone.OnSelectedStateChange += _viewModel.OnButtonSelectedStateChange;

            TaskManager.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            CutScreen.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            ESC.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Mute.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            QuitGame.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            KeyDescription.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            ScreenKeyboard.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            PerformanceMode.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Custom.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            StatusMonitoring.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            ControllerMode.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Shake.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Light.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            GyroEmulate.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RepeatMode.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Brightness.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Resolution.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            RefresRate.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            ScaleRate.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Speaker.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;
            Microphone.OnHovedStateChange += _viewModel.OnButtonHovedStateChange;

            var tuple = SystemUtils.Instance.GetScreenPixels();
            List<ItemModel> resolution = new List<ItemModel>();
            List<ItemModel> res_freqs = new List<ItemModel>();
            var pixels = tuple.Item1;
            _freqs = tuple.Item2;

            for (int i = 0; i < pixels.Count; i++)
            {
                resolution.Add(new ItemModel() { Index = i, Text = pixels[i].ToString(), Value = pixels[i] });
            }

            _viewModel.Resolution = resolution;
            Resolution.Data = _viewModel.Resolution;
            Resolution.SelectedItem = resolution[0];
            Resolution.OnSelectItemChanged += ResolutionChanged;

            var freqs = _freqs[resolution[Resolution.SelectIndex].Value as Pixels];

            for (int i = 0; i < freqs.Count; i++)
            {
                res_freqs.Add(new ItemModel() { Index = i, Text = freqs[i].ToString(), Value = freqs[i] });
            }

            _viewModel.RefresRate = res_freqs;
            RefresRate.Data = _viewModel.RefresRate;

            ScaleRate.Data = _viewModel.ScaleRate;

            this.Loaded += QuickMenu_Loaded;
            this.Deactivated += QuickMenu_Deactivated;

            PerformanceMode.OnSelectedElementChanged += (s, o) =>
            {
                if (o is TdpUtils.TdpMode tdp && _viewModel.PerformanceMode != tdp)
                {
                    var value = o;
                    _viewModel.PerformanceMode = tdp;
                    //TdpUtils.Instance.SetTdp(Convert.ToByte(value));
                    TdpUtils.Instance.SetTdpByQueue(tdp, TimeSpan.FromTicks(DateTime.Now.Ticks));
                }
            };

            Custom.OnSliderListItemValueChanged += (s, v) => _viewModel.Performance = (TdpUtils.TdpMode)v;
            
            ControllerMode.OnSelectedElementChanged += (s, v) => _viewModel.ControllerMode = (byte)v;

            _viewModel.VolumeChanged = () =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Speaker.Value = Convert.ToInt32(_viewModel.SpeakerVolume);
                    //Microphone.Value = Convert.ToInt32(_viewModel.MicrophoneVolume);
                }));
            };
            Brightness.OnSliderListItemValueChanged += (s, v) => SystemUtils.Instance.SetBrightness(v);
            Speaker.OnSliderListItemValueChanged += (s, v) => _viewModel.SpeakerVolume = v;
            Microphone.OnSliderListItemValueChanged += (s, v) => _viewModel.MicrophoneVolume = v;
            Shake.OnSelectedElementChanged += (s, v) => _viewModel.MotorIntensity = (byte)v;
            RepeatMode.OnCheckedStateChanged += (s, v) => _viewModel.TurboOpen = v;

            TaskManager.OnClick += s => YzGamingService.Instance.StartTaskManage();
            CutScreen.OnClick += s => YzGamingService.Instance.HideQuickMenu(() => CaptureUtils.Instance.Capture());
            ESC.OnClick += s => YzGamingService.Instance.SendEscMessage();
            Mute.OnClick += s =>
            {
                AudioUtils.Instance.Mute = !AudioUtils.Instance.Mute;
                //if (Speaker.Value == 0)
                //{
                //    Speaker.Value = _tmpVolume;
                //    _viewModel.SpeakerVolume = _tmpVolume;
                //}
                //else
                //{
                //    _tmpVolume = Speaker.Value;
                //    Speaker.Value = 0;
                //    _viewModel.SpeakerVolume = 0;
                //}
            };
            KeyDescription.OnClick += s =>
            {
                Queue<Type> queue = new Queue<Type>();
                queue.Enqueue(typeof(ControllerPageView));
                queue.Enqueue(typeof(KeyPageView));
                queue.Enqueue(typeof(KeyDescriptionPageView));

                YzGamingService.Instance.NavigationToPage(queue);
            };
            ScreenKeyboard.OnClick += s => YzGamingService.Instance.OpenTouchKeyboard();

            PerformanceMode.LeftElement = TdpUtils.TdpMode.BALANCE;
            PerformanceMode.RightElement = TdpUtils.TdpMode.PERFERMANCE;
            if (PerformanceMode.SelectElement == null) {
                PerformanceMode.SelectElement = TdpUtils.TdpMode.BALANCE;
            }
            ControllerMode.LeftElement = (byte)0;
            ControllerMode.RightElement = (byte)1;
            Shake.LeftElement = (byte)2;
            Shake.CenterElement = (byte)1;
            Shake.RightElement = (byte)0;

            _viewModel.Model.OnProfileReresh += model =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _viewModel.ControllerMode = model.ControllerMode;
                    if (this.IsActive)
                    {
                        ControllerMode.SelectElement = model.ControllerMode;
                    }
                }));
            };

            _lastShownTime = new TimeSpan(DateTime.Now.Ticks);
            GridSetting.Visibility = Visibility.Collapsed;
            GridButton.Visibility = Visibility.Collapsed;
        }

        private void QuickMenu_Deactivated(object sender, EventArgs e)
        {
            if (!YzGamingService.Instance.IsQuickMenuShown) return;

            YzGamingService.Instance.HideQuickMenu();
        }

        private void QuickMenu_Loaded(object sender, RoutedEventArgs e)
        {
            //Initialization();
        }

        public new void Show()
        {
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            if (now.Subtract(_lastShownTime).TotalMilliseconds < 300) return;
            _lastShownTime = now;

            Initialization();
            base.Show();

            this.WindowState = WindowState.Maximized;
            this.Activate();

            PerformanceMode.Reload();
            ControllerMode.Reload();
            Shake.Reload();

            TdpUtils.Instance.StartMonitor(tdp =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _viewModel.Performance = tdp;
                    PerformanceMode.SelectElement = tdp;
                }));
            });
        }

        public new void Hide()
        {
            YzGamingService.Instance.IsQuickMenuShown = false;
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            if (now.Subtract(_lastShownTime).TotalMilliseconds < 300) return;
            _lastShownTime = now;

            GridSetting.Visibility = Visibility.Collapsed;
            GridButton.Visibility = Visibility.Collapsed;

            try
            {
                this.WindowState = WindowState.Minimized;
                _viewModel.IsHandleDpadKey = false;
                TdpUtils.Instance.StopMonitor();

                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() => base.Hide()));
                });
            }
            catch
            {

            }
        }

        public void Hide(Action callBack)
        {
            YzGamingService.Instance.IsQuickMenuShown = false;
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            if (now.Subtract(_lastShownTime).TotalMilliseconds < 300) return;
            _lastShownTime = now;

            try
            {
                this.WindowState = WindowState.Minimized;
                _viewModel.IsHandleDpadKey = false;
                TdpUtils.Instance.StopMonitor();

                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() => base.Hide()));
                });

                if (callBack != null)
                {
                    Task.Run(callBack);
                }
            }
            catch
            {
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            this.Left = this.Width / 2;
            this.Top = 0.0;
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;

            double last = 40;
            double left = this.Left;

            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.Opacity = 1;
                    GridSetting.Visibility = Visibility.Visible;
                    GridButton.Visibility = Visibility.Visible;
                }));

                while (left > 0)
                {
                    if (left < 300 && last > 5)
                    {
                        last -= 5;
                    }

                    left = left - last < 0 ? 0 : left - last;

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Left = left;
                    }));
                    
                    Thread.Sleep(1);
                }

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Left = 0;
                    this.Top = 0.0;

                    this.WindowState = WindowState.Maximized;
                    YzGamingService.Instance.IsQuickMenuShown = true;
                }));
            });
        }

        private void Initialization()
        {
            _viewModel.Initialization();

            Resolution.IsNeedSave = true;

            // performance
            PerformanceMode.SelectElement = _viewModel.PerformanceMode;
            Custom.Value = Convert.ToInt32(_viewModel.Performance);
            StatusMonitoring.IsChecked = _viewModel.StatusMonitoring;

            // controller
            ControllerMode.SelectElement = _viewModel.ControllerMode;
            Shake.SelectElement = _viewModel.MotorIntensity;
            Light.IsChecked = _viewModel.LightIntensity;
            GyroEmulate.IsChecked = _viewModel.GyroEmulate;
            RepeatMode.IsChecked = _viewModel.TurboOpen;

            // display
            Brightness.Value = _viewModel.Brightness;
            Resolution.SelectIndex = _viewModel.ResolutionIndex;
            RefresRate.SelectIndex = _viewModel.RefresRateIndex;
            ScaleRate.SelectIndex = _viewModel.GetScaleRate(SystemUtils.Instance.GetScreenScalingFactor()).Index;

            // audio
            Speaker.Value = Convert.ToInt32(_viewModel.SpeakerVolume);
            Microphone.Value = Convert.ToInt32(_viewModel.MicrophoneVolume);

            TaskManager.IsSelected = true;
            TaskManager.IsHoved = true;
            _viewModel.IsHandleDpadKey = true;
        }

        private void ResolutionChanged(IQuickMenuControl sender, object item)
        {
            System.Diagnostics.Debug.WriteLine("ResolutionChanged 1");
            if (item is ItemModel model && model.Value is Pixels pixels)
            {
                System.Diagnostics.Debug.WriteLine("ResolutionChanged 2");
                SystemUtils.Instance.ChangeResolution(pixels.DevModel);

                _viewModel.RefresRate.Clear();
                var freqs = _freqs[_viewModel.Resolution[Resolution.SelectIndex].Value as Pixels];

                for (int i = 0; i < freqs.Count; i++)
                {
                    _viewModel.RefresRate.Add(new ItemModel() { Index = i, Text = freqs[i].ToString(), Value = freqs[i] });
                }
            }
        }
    }
}
