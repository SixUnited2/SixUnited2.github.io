using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using yz.gaming.accessoryapp.Api;
using yz.gaming.accessoryapp.Service;

namespace yz.gaming.accessoryapp.Utils
{
    public class TdpUtils
    {
        public enum TdpMode
        {
            BALANCE = 0,
            PERFERMANCE = 1,
            QUIET = 2
        }

        private static TdpUtils _tdpUtils;
        protected Logger _logger = LogManager.GetCurrentClassLogger();
        private Queue<byte> _setQueue { get; set; }
        private TimeSpan _lastSetTime { get; set; }
        private Dictionary<byte, TdpMode> _tdpMapping = new Dictionary<byte, TdpMode>
        {
            {0x12, TdpMode.BALANCE },
            {0x11, TdpMode.QUIET },
            {0x13, TdpMode.PERFERMANCE }
        };
        private Action<TdpMode> _tdpChangedCallback;

        public static TdpUtils Instance
        {
            get
            {
                if (_tdpUtils == null)
                {
                    _tdpUtils = new TdpUtils();
                }

                return _tdpUtils;
            }
        }

        public TdpMode Tdp { get; set; } = TdpMode.BALANCE;

        public int Result { get; set; } = -1;
        public bool IsAutoMonitor { get; set; }
        public Dictionary<byte, TdpMode> TdpMapping => _tdpMapping;

        public TdpUtils()
        {
            _setQueue = new Queue<byte>();
            _lastSetTime = TimeSpan.FromTicks(DateTime.Now.Ticks);

            YzGamingService.Instance.OnTdpChanged += @event =>
            {
                if (_tdpMapping.ContainsKey(@event))
                {
                    _tdpChangedCallback?.Invoke(_tdpMapping[@event]);
                }
            };
        }

        public int Initialize()
        {
            return YzCommonApi.YZGamingWmi_Initialize();
        }

        public bool Uninitialize()
        {
            return YzCommonApi.YzGamingWmi_Uninitialize() == 0;
        }

        public TdpMode GetTdp()
        {
            try
            {
                int tdp = 0;

                Result = Initialize();

                if (Result == 0)
                {
                    Result = YzCommonApi.YzGamingWmi_GetPowerMode(ref tdp);
                    if (Result == 0)
                    {
                        if (tdp >= 0 && tdp <= 2)
                        {
                            Tdp = (TdpMode)tdp;
                        }
                        else
                        {
                            tdp = 0;
                        }
                    }
                }

                return (TdpMode)tdp;
            }
            catch (ManagementException err)
            {
                _logger.Error("An error occurred while trying to execute the WMI method: " + err.Message);
            }
            finally
            {
                Uninitialize();
            }

            return 0;
        }

        public bool SetTdp(byte value)
        {
            try
            {
                if (Initialize() == 0)
                {
                    return YzCommonApi.YzGamingWmi_SetPowerMode(value) == 0;
                }
            }
            catch (ManagementException err)
            {
                _logger.Error("An error occurred while trying to execute the WMI method: " + err.Message);
            }
            finally
            {
                Uninitialize();
            }

            return false;
        }

        public void SetTdpByQueue(TdpMode value, TimeSpan time)
        {
            if (Tdp != value || _setQueue.Count > 0)
            {
                _lastSetTime = time;
                _setQueue.Enqueue((byte)value);
            }
        }

        public void StartMonitor(Action<TdpMode> callback)
        {
            _tdpChangedCallback = callback;

            if (!IsAutoMonitor)
            {
                IsAutoMonitor = true;

                Task.Run(async () =>
                {
                    while (IsAutoMonitor)
                    {
                        int failCount = 10;
                        for (int i = 0; i < 200; i++)
                        {
                            if (_setQueue.Count > 1)
                            {
                                _setQueue.Dequeue();
                            }
                            else if (_setQueue.Count == 1)
                            {
                                TimeSpan now = TimeSpan.FromTicks(DateTime.Now.Ticks);
                                if (now.Subtract(_lastSetTime).TotalMilliseconds > 500)
                                {
                                    byte b = _setQueue.Dequeue();
                                    if (!SetTdp(b))
                                    {
                                        if (failCount > 0) 
                                        {
                                            failCount--;
                                            _setQueue.Enqueue(b);
                                        }
                                    }
                                    else
                                    {
                                        Tdp = (TdpMode)b;
                                    }
                                    await Task.Delay(200);
                                }
                            }

                            await Task.Delay(10);
                            if (!IsAutoMonitor) break;
                        }

                        await Task.Delay(100);
                    }
                });
            }
        }

        public void StopMonitor()
        {
            IsAutoMonitor = false;
        }
    }
}
