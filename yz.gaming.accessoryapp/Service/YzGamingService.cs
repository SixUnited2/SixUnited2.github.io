using CommunityToolkit.Mvvm.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using yz.gaming.accessoryapp.Api;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Utils;
using yz.gaming.accessoryapp.Utils.Com;
using static yz.gaming.accessoryapp.Api.YzCommonApi;
using static yz.gaming.accessoryapp.Service.ThumbStatus;

namespace yz.gaming.accessoryapp.Service
{
    public class YzGamingService
    {
        public delegate void ProfileDataChangedHandler(YZDpadStruct dpadStruct);
        public delegate void DpadKeyPressHandler(KeyCodeEnum key, KeyPressTypeEnmu type);
        public delegate void TdpChangedHandler(byte tdp);

        static YzGamingService _yzGamingApi;
        static object locker = new object();

        public static YzGamingService Instance
        {
            get
            {
                lock (locker)
                {
                    if (_yzGamingApi == null)
                    {
                        _yzGamingApi = new YzGamingService();
                    }
                }

                return _yzGamingApi;
            }
        }

        IntPtr _foreWinHandle = IntPtr.Zero;
        IntPtr _hProfileDevice = IntPtr.Zero;
        IntPtr _contextPtr;
        YzApiContext _context;
        Pfn_ProfileDeviceEventHandler pfn_handler = null;
        ThumbStatus _thumbStatus;
        Dictionary<KeyCodeEnum, TimeSpan> _pressTimeDic;
        YzProfileModel _model;
        Logger _logger = LogManager.GetCurrentClassLogger();
        bool _isCheckLongPressRunning = false;

        public event ProfileDataChangedHandler OnProfileDataChanged;
        public event DpadKeyPressHandler OnDpadKeyPress;
        public event ThumbStatusReportHandler OnThumbStatusReport;
        public event TdpChangedHandler OnTdpChanged;

        public Action<Queue<Type>> NavigationToPage { get; set; }

        public QuickMenu QuickMenu { get; set; }
        public MainWindow MainWindow { get; set; }
        public bool IsDeviceExists { get; set; }
        public bool IsInited { get; set; }

        public bool IsQuickMenuShown { get; set; } = false;
        public bool IsMainShown { get; set; } = false;
        public bool IsProcessRunning { get; set; } = false;

        public void Initialize()
        {
            _thumbStatus = new ThumbStatus();
            _thumbStatus.OnThumbStatusReport += ThumbStatusReportHandler;
            _pressTimeDic = new Dictionary<KeyCodeEnum, TimeSpan>();
            _model = Ioc.Default.GetRequiredService<YzProfileModel>();
            _context = new YzApiContext();
            _contextPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(YZDpadStruct)));

            pfn_handler = new Pfn_ProfileDeviceEventHandler(Pfn_ProfileDeviceEvent);
            IsInited = YzCommonApi.YZGamingCom_Initialize(pfn_handler, _contextPtr) == 0;

            _logger.Debug($"YZGamingCom initialize {(IsInited ? "success" : "fail")}.");

            SystemUtils.Instance.WmiWatcherStart(s => { OnWmiCallback(s); });
        }

        public void Uninitialize()
        {
            if (IsInited)
            {
                YzCommonApi.YZGamingCom_Uninitialize();
                SystemUtils.Instance.WmiWacherStop();
            }
        }

        public bool ReadProfile(ref YZDpadStruct dpadStruct)
        {
            if (IsDeviceExists)
            {
                IntPtr data = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(YZDpadStruct)));

                try
                {
                    if (YzCommonApi.YZGamingCom_ReadProfileData(_hProfileDevice, data) == YzCommonApi.SUCCESS)
                    {
                        dpadStruct = (YZDpadStruct)Marshal.PtrToStructure(data, typeof(YZDpadStruct));
                        return true;
                    }
                }
                finally
                {
                    YzCommonApi.YZGamingCom_ReleaseProfileDataForRead(data);
                    Marshal.FreeHGlobal(data);
                }
            }


            return false;
        }

        public bool WriteProfile(YZDpadStruct dpadStruct)
        {
            if (IsDeviceExists)
            {
                IntPtr data = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(YZDpadStruct)));

                try
                {
                    Marshal.StructureToPtr(dpadStruct, data, true);
                    var result = YzCommonApi.YZGamingCom_WriteProfileData(_hProfileDevice, data);
                    return result == YzCommonApi.SUCCESS;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    Marshal.FreeHGlobal(data);
                }
            }

            return false;
        }

        private void Pfn_ProfileDeviceEvent(ProfileDeviceEventEnmu deviceEvent, IntPtr hProfileDevice, UIntPtr wParam, IntPtr lParam, IntPtr lpContext)
        {
            try
            {
                switch (deviceEvent)
                {
                    case ProfileDeviceEventEnmu.DeviceAdded:
                        DeviceAdded(hProfileDevice);
                        break;
                    case ProfileDeviceEventEnmu.DeviceRemoved:
                        DeviceRemoved();
                        break;
                    case ProfileDeviceEventEnmu.KeyPressed:
                        if (_model is null) return;

                        var keyCode = (KeyCodeEnum)wParam.ToUInt32();

                        if (_model.ControllerMode == 1 &&
                            (deviceEvent == ProfileDeviceEventEnmu.KeyPressed || deviceEvent == ProfileDeviceEventEnmu.ThumbPressed) &&
                            keyCode != KeyCodeEnum.Quick)
                        {
                            _logger.Trace($"Ignore Key event in KeyMouse model!");
                            return;
                        }

                        _logger.Trace($"KeyPressed: [{wParam} | {lParam}]");

                        var eventType = (KeyEventTypeEnmu)lParam.ToInt32();
                        var pressType = CheckPressType(keyCode, eventType);

                        if (eventType == KeyEventTypeEnmu.UP && pressType != KeyPressTypeEnmu.Ignore)
                        {
                            //if ((!GamePlatform.Instance.IsPlatformRunning && !GamePlatform.Instance.IsPlatformStarting) ||
                            //                        (KeyCodeEnum)wParam.ToUInt32() == KeyCodeEnum.Quick)
                            {
                                OnDpadKeyPress?.Invoke(keyCode, pressType);
                            }
                        }
                        break;
                    case ProfileDeviceEventEnmu.TriggerPressed:
                        _logger.Trace($"TriggerPressed: [{wParam} | {lParam}]");
                        break;
                    case ProfileDeviceEventEnmu.ThumbPressed:
                        if (_model is null) return;
                        if (_model.ControllerMode == 1 &&
                            (deviceEvent == ProfileDeviceEventEnmu.KeyPressed || deviceEvent == ProfileDeviceEventEnmu.ThumbPressed))
                        {
                            _logger.Trace($"Ignore Thumb event in KeyMouse model!");
                            return;
                        }

                        //_logger.Trace($"ThmbPressed: [{wParam} | {lParam}]");

                        //if ((!GamePlatform.Instance.IsPlatformRunning && !GamePlatform.Instance.IsPlatformStarting) ||
                        //    IsQuickMenuShown)
                        {
                            _thumbStatus.InValueGot((ThumbEnmu)wParam.ToUInt32(), lParam.ToInt32());
                        }
                        break;
                    case ProfileDeviceEventEnmu.ModeChanged:
                        _logger.Trace($"Controller mode changed from [{wParam}] to [{lParam}]");
                        if (_model != null)
                        {
                            _model.SetControllerMode(lParam.ToInt32() > 0 ? (byte)0x01 : (byte)0x00);
                        }
                        break;
                    case ProfileDeviceEventEnmu.VersionArrived:
                        var minor = BitConverter.GetBytes(lParam.ToInt32())[0].ToString("X2");
                        _logger.Trace($"Firmware version is {wParam}.{minor}");
                        if (_model != null)
                        {
                            int iMinor = Convert.ToInt32(minor);
                            _model.Version = new Version((int)wParam, iMinor);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void DeviceAdded(IntPtr hProfileDevice)
        {
            _logger.Info("Device online");
            _hProfileDevice = hProfileDevice;
            IntPtr data = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(YZDpadStruct)));

            try
            {
                _logger.Info("Reading profile...");
                if (YzCommonApi.YZGamingCom_ReadProfileData(_hProfileDevice, data) == YzCommonApi.SUCCESS)
                {
                    _logger.Info("Read profile success.");
                    YZDpadStruct dpadStruct = (YZDpadStruct)Marshal.PtrToStructure(data, typeof(YZDpadStruct));
                    OnProfileDataChanged?.Invoke(dpadStruct);
                }
                else
                {
                    _logger.Info("Read profile fail!");
                }
            }
            finally
            {
                _logger.Info("Free profile ptr data...");
                YzCommonApi.YZGamingCom_ReleaseProfileDataForRead(data);
                Marshal.FreeHGlobal(data);
            }

            IsDeviceExists = true;
        }

        private void DeviceRemoved()
        {
            _logger.Info("Device offline");
            _hProfileDevice = IntPtr.Zero;
            IsDeviceExists = false;
        }

        private KeyPressTypeEnmu CheckPressType(KeyCodeEnum keyCode, KeyEventTypeEnmu eventType)
        {
            var pressType = KeyPressTypeEnmu.ShortPress;

            if (eventType == KeyEventTypeEnmu.DOWN)
            {
                if (_pressTimeDic.ContainsKey(keyCode))
                {
                    _pressTimeDic[keyCode] = new TimeSpan(DateTime.Now.Ticks);
                }
                else
                {
                    _pressTimeDic.Add(keyCode, new TimeSpan(DateTime.Now.Ticks));
                }
            }
            else if (eventType == KeyEventTypeEnmu.UP)
            {
                if (_pressTimeDic.ContainsKey(keyCode))
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks).Subtract(_pressTimeDic[keyCode]);
                    if (ts.TotalMilliseconds > 800) pressType = KeyPressTypeEnmu.LongPress;
                    _pressTimeDic.Remove(keyCode);
                }
                else
                {
                    pressType = KeyPressTypeEnmu.Ignore;
                }
            }

            if (!_isCheckLongPressRunning && _pressTimeDic.Count > 0)
            {
                Task.Run(async () =>
                {
                    _isCheckLongPressRunning = _pressTimeDic.Count > 0;

                    while (_isCheckLongPressRunning)
                    {
                        List<KeyCodeEnum> needRemove = new List<KeyCodeEnum>();

                        foreach (var item in _pressTimeDic)
                        {
                            var ts = new TimeSpan(DateTime.Now.Ticks).Subtract(item.Value);
                            if (ts.TotalMilliseconds > 800)
                            {
                                OnDpadKeyPress?.Invoke(item.Key, KeyPressTypeEnmu.LongPress);
                                if (!needRemove.Contains(item.Key)) needRemove.Add(item.Key);
                            }
                        }

                        needRemove.ForEach(p => _pressTimeDic.Remove(p));
                        _isCheckLongPressRunning = _pressTimeDic.Count > 0;
                        await Task.Delay(10);
                    }
                });
            }

            return pressType;
        }

        public void ThumbStatusReportHandler(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            _logger.Trace($"ThumbStatusReport: [{key} | {direction}]");
            OnThumbStatusReport?.Invoke(key, direction);
        }

        public void ShowQuickMenu()
        {
            //GamePlatform.Instance.MiniWindows();
            //_foreWinHandle = WinApi.GetForegroundWindow();
            //_logger.Trace($"Show quick menu ForegroundWindows {_foreWinHandle.ToInt32()}");
            QuickMenu.Show();
        }

        public void HideQuickMenu()
        {
            if (QuickMenu != null)
            {
                _logger.Trace($"Hide quick menu");
                QuickMenu.Hide();
                MainWindow.CheckStatus();
            }
        }

        public void HideQuickMenu(Action callBack)
        {
            if (QuickMenu != null)
            {
                _logger.Trace($"Hide quick menu");
                QuickMenu.Hide(callBack);
                MainWindow.CheckStatus();
            }
        }

        public void ShowMainWindows()
        {
            MainWindow.Show();
        }

        public void HideMainWindows()
        {
            if (MainWindow != null)
            {
                MainWindow.Hide();
            }
        }

        Process proc = null;
        public void StartTaskManage()
        {
            try
            {
                proc = new Process();
                //string command = $"%WinDir%\\System32\\taskmgr.exe";
                string command = $"C:\\Windows\\System32\\taskmgr.exe";
                proc.StartInfo.FileName = command;
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(command);
                proc.EnableRaisingEvents = true;
                proc.Exited += (s, e) =>
                {
                    proc.Dispose();
                    IsProcessRunning = false;
                };

                IsProcessRunning = true;
                proc.Start();
                QuickMenu.Hide();
            }
            catch (Exception ex)
            {
                _logger.Error("Start TaskManage process exception");
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        public void OnWmiCallback(byte @event)
        {
            if (@event == 0x38) 
            {
                OpenTouchKeyboard();
                return;
            }

            if (@event == 0x11 || @event == 0x12 || @event == 0x13)
            {
                OnTdpChanged?.Invoke(@event);
            }
        }

        Process keyboard = null;
        const string WINDOW_CLASS = "IPTip_Main_Window";
        public void OpenTouchKeyboard()
        {
            try
            {
                IntPtr taptip = WinApi.FindWindowEx(IntPtr.Zero, IntPtr.Zero, WINDOW_CLASS, string.Empty);
                if (taptip != IntPtr.Zero)
                {
                    ITipInvocation instance = null;
                    instance = (ITipInvocation)Activator.CreateInstance(ComTypes.TipInvocationType);
                    instance.Toggle(WinApi.GetDesktopWindow());
                }
                else
                {
                    keyboard = new Process();
                    string command = @"C:\Program Files\Common Files\microsoft shared\ink\TabTip.exe";
                    keyboard.StartInfo.FileName = command;
                    keyboard.StartInfo.Verb = "runas";
                    keyboard.StartInfo.WorkingDirectory = Path.GetDirectoryName(command);
                    keyboard.EnableRaisingEvents = true;
                    keyboard.StartInfo.UseShellExecute = true;
                    keyboard.Exited += (s, e) =>
                    {
                        keyboard.Dispose();
                    };

                    keyboard.Start();
                }
                QuickMenu.Hide();
            }
            catch (Exception ex)
            {
                _logger.Error("Open TouchKeyboard process exception");
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        public void SendEscMessage()
        {
            //WinApi.PostMessage(_foreWinHandle, WinApi.WM_KEYDOWN, 27, 0);
            //System.Threading.Thread.Sleep(10);
            //WinApi.PostMessage(_foreWinHandle, WinApi.WM_KEYUP, 27, 0);
        }
    }
}
