using System;
using System.Collections.Generic;
using System.Management;

namespace YZ.Gaming.Utils
{
    public class SystemUtils
    {
        private static SystemUtils instance = null;
        private readonly string root = @"\\.\root\cimv2";

        public static SystemUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SystemUtils();
                }

                return instance;
            }
        }

        public string GetComputerSystemModel()
        {
            var result = ExecuteWMIQuery("SELECT * FROM Win32_ComputerSystem", "Model");
            return (string)result[0];
        }

        public string GetCPUName()
        {
            var result = ExecuteWMIQuery("SELECT * FROM Win32_Processor", "Name");
            return (string)result[0];
        }

        public string GetDisplayAdapterName()
        {
            var result = ExecuteWMIQuery("SELECT * FROM Win32_VideoController", "Name");
            return (string)result[0];
        }

        public UInt64 GetMemoryCapacity()
        {
            UInt64 capacity = 0;

            var result = ExecuteWMIQuery("SELECT * FROM Win32_PhysicalMemory", "Capacity");
            foreach (var v in result)
            {
                capacity += (UInt64)v;
            }

            return capacity;
        }

        public UInt64 GetDiskSize()
        {
            UInt64 size = 0;

            var result = ExecuteWMIQuery("SELECT * FROM Win32_DiskDrive", "Size");
            foreach (var v in result)
            {
                size += (UInt64)v;
            }

            return size;
        }

        public UInt32 GetBatteryFullChargeCapacity()
        {
            UInt32 capacity = 0;

            var result = ExecuteWMIQuery("SELECT * FROM Win32_Battery", "FullChargeCapacity");
            if (result[0] != null)
            {
                capacity = (UInt32)result[0];
            }

            return capacity;
        }

        public string GetWirelessNetworkAdapter()
        {
            var result = ExecuteWMIQuery("SELECT * FROM Win32_NetworkAdapter", "Name");
            return (string)result.Find(x => { string s = (string)x; return (s.ToLower().Contains("wireless") || s.Contains("802.11") || s.ToLower().Contains("wlan")) && !s.ToLower().Contains("virtual"); });
        }

        public string GetCurrentDisplayName()
        {
            var result = ExecuteWMIQuery("SELECT * FROM Win32_DesktopMonitor ", "Name");
            return (string)result[0];
        }

        private List<object> ExecuteWMIQuery(string wmiquery, string field)
        {
            List<object> result = new List<object>();

            ManagementScope scope = new ManagementScope(root);
            if (scope != null)
            {
                scope.Connect();
                if (scope.IsConnected)
                {
                    ObjectQuery query = new ObjectQuery(wmiquery);
                    if (query != null)
                    {
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                        if (searcher != null)
                        {
                            foreach (var m in searcher.Get())
                            {
                                result.Add(m[field]);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
