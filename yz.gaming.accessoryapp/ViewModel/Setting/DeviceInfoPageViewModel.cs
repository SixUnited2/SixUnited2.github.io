using System;
using System.Collections.Generic;
using System.Text;
using yz.gaming.accessoryapp.Utils;

namespace yz.gaming.accessoryapp.ViewModel.Setting
{
    public class DeviceInfoPageViewModel : ViewModelBase, ITipButtomMapSupport
    {
        public List<bool> TipButtomMap { get; } = new List<bool> {false, false, false, false, true, false, false, false, false };

        private string _deviceModel;
        public string DeviceModel
        {
            get => _deviceModel;
            set => SetProperty(ref _deviceModel, value);
        }

        private string _processor;
        public string Processor
        {
            get => _processor;
            set => SetProperty(ref _processor, value);
        }

        private string _graphicsCard;
        public string GraphicsCard
        {
            get => _graphicsCard;
            set => SetProperty(ref _graphicsCard, value);
        }

        private string _memory;
        public string Memory
        {
            get => _memory;
            set => SetProperty(ref _memory, value);
        }

        private string _disk;
        public string Disk
        {
            get => _disk;
            set => SetProperty(ref _disk, value);
        }

        private string _displayhScreen;
        public string DisplayhScreen
        {
            get => _displayhScreen;
            set => SetProperty(ref _displayhScreen, value);
        }

        private string _betteryCapacity;
        public string BetteryCapacity
        {
            get => _betteryCapacity;
            set => SetProperty(ref _betteryCapacity, value);
        }

        private string _wlan;
        public string WLAN
        {
            get => _wlan;
            set => SetProperty(ref _wlan, value);
        }

        public DeviceInfoPageViewModel() 
            : base()
        {
        }

        public override void Initialization()
        {
            Title = GetString("DeviceInfo");

            SetProperty(ref _deviceModel, SystemUtils.Instance.GetComputerSystemModel(), nameof(DeviceModel));
            SetProperty(ref _processor, SystemUtils.Instance.GetCPUName(), nameof(Processor));
            SetProperty(ref _graphicsCard, SystemUtils.Instance.GetDisplayAdapterName(), nameof(GraphicsCard));
            SetProperty(ref _memory, (SystemUtils.Instance.GetMemoryCapacity() / 1024 / 1024 / 1024).ToString("#0GB"), nameof(Memory));
            SetProperty(ref _disk, (SystemUtils.Instance.GetDiskSize() / 1024 / 1024 / 1024).ToString("#0GB"), nameof(Disk));
            SetProperty(ref _displayhScreen, SystemUtils.Instance.GetCurrentDisplayName(), nameof(DisplayhScreen));
            SetProperty(ref _betteryCapacity, $"{Math.Round(SystemUtils.Instance.GetBatteryFullChargeCapacity() / 1000d)}Wh", nameof(BetteryCapacity));
            SetProperty(ref _wlan, SystemUtils.Instance.GetWirelessNetworkAdapter(), nameof(WLAN));
        }
    }
}
