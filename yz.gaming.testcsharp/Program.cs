using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YZ.Gaming.Utils;

namespace YZ.Gaming.TestCSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string s = SystemUtils.Instance.GetDisplayAdapterName();
            s = SystemUtils.Instance.GetWirelessNetworkAdapter();
            s = SystemUtils.Instance.GetCurrentDisplayName();
            UInt64 m = SystemUtils.Instance.GetMemoryCapacity();
            m = m / 1024 / 1024 / 1024;

            UInt64 d = SystemUtils.Instance.GetDiskSize();
            d = d / 1024 / 1024 / 1024;

            UInt32 power = SystemUtils.Instance.GetBatteryFullChargeCapacity();
        }
    }
}
