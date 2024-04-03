using System;
using System.Collections.Generic;
using System.Text;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.Service
{
    public class ThumbStatus
    {
        public delegate void ThumbStatusReportHandler(ThumbKeyEnmu key, ThumbDirectionEnmu direction);

        public event ThumbStatusReportHandler OnThumbStatusReport;

        const int MID_VALUE = 0x80;
        const int THRESHOLD_VALUE = 38;
        const int REPORT_TICK = 300;
        const int REPET_TICK = 100;

        Dictionary<ThumbEnmu, TimeSpan> _thumbReport { get; set; }
        Dictionary<ThumbEnmu, int> _thumbRepet { get; set; }

        TimeSpan _lastThumbEvent;

        public ThumbStatus()
        {

            _thumbReport = new Dictionary<ThumbEnmu, TimeSpan>()
            {
                { ThumbEnmu.LX, TimeSpan.FromTicks(DateTime.Now.Ticks) },
                { ThumbEnmu.LY, TimeSpan.FromTicks(DateTime.Now.Ticks) },
                { ThumbEnmu.RX, TimeSpan.FromTicks(DateTime.Now.Ticks) },
                { ThumbEnmu.RY, TimeSpan.FromTicks(DateTime.Now.Ticks) }
            };

            _thumbRepet = new Dictionary<ThumbEnmu, int>()
            {
                { ThumbEnmu.LX, 0 },
                { ThumbEnmu.LY, 0 },
                { ThumbEnmu.RX, 0 },
                { ThumbEnmu.RY, 0 }
            };

            _lastThumbEvent = TimeSpan.FromTicks(DateTime.Now.Ticks);
        }

        public void InValueGot(ThumbEnmu thumb, int value)
        {
            TimeSpan now = TimeSpan.FromTicks(DateTime.Now.Ticks);
            if (now.Subtract(_lastThumbEvent).TotalMilliseconds < REPORT_TICK && _thumbRepet[thumb] <= 2) return;

            if (value > MID_VALUE + THRESHOLD_VALUE || value < (MID_VALUE - THRESHOLD_VALUE))
            {
                if (now.Subtract(_thumbReport[thumb]).TotalMilliseconds > REPET_TICK)
                {
                    switch (thumb)
                    {
                        case ThumbEnmu.LX:
                            OnThumbStatusReport?.Invoke(ThumbKeyEnmu.Left,
                                value > MID_VALUE ? ThumbDirectionEnmu.RIGHT : ThumbDirectionEnmu.LEFT);
                            break;
                        case ThumbEnmu.LY:
                            OnThumbStatusReport?.Invoke(ThumbKeyEnmu.Left,
                                value > MID_VALUE ? ThumbDirectionEnmu.UP : ThumbDirectionEnmu.DOWN);
                            break;
                        case ThumbEnmu.RX:
                            OnThumbStatusReport?.Invoke(ThumbKeyEnmu.Right,
                                value > MID_VALUE ? ThumbDirectionEnmu.RIGHT : ThumbDirectionEnmu.LEFT);
                            break;
                        case ThumbEnmu.RY:
                            OnThumbStatusReport?.Invoke(ThumbKeyEnmu.Right,
                                value > MID_VALUE ? ThumbDirectionEnmu.UP : ThumbDirectionEnmu.DOWN);
                            break;
                    }

                    if (_thumbRepet[thumb] <= 2 &&
                        now.Subtract(_thumbReport[thumb]).TotalMilliseconds > REPORT_TICK)
                    {
                        _thumbRepet[thumb] = _thumbRepet[thumb] + 1;
                    }

                    _lastThumbEvent = now;
                    _thumbReport[thumb] = now;
                }
            }
            else
            {
                _thumbRepet[thumb] = 0;
            }
        }
    }
}
