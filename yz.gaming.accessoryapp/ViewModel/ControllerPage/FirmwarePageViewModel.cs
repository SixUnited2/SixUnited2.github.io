using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Utils;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class FirmwarePageViewModel : ViewModelBase, ITipButtomMapSupport
    {
        public List<bool> TipButtomMap => new List<bool> { false, false, false, true, true, false, false, false, false };

        string _controllerCode;
        public string ControllerCode
        {
            get => _controllerCode;
            set
            {
                SetProperty(ref _controllerCode, value);
            }
        }

        string _currentVersion;
        public string CurrentVersion
        {
            get => _currentVersion;
            set
            {
                SetProperty(ref _currentVersion, value);
            }
        }

        string _currentLogs;
        public string CurrentLogs
        {
            get => _currentLogs;
            set
            {
                SetProperty(ref _currentLogs, value);
            }
        }

        string _newVersion;
        public string NewVersion
        {
            get => _newVersion;
            set
            {
                SetProperty(ref _newVersion, value);
            }
        }

        string _newLogs;
        public string NewLogs
        {
            get => _newLogs;
            set
            {
                SetProperty(ref _newLogs, value);
            }
        }

        bool _isNewVersion;
        public bool IsNewVersion
        {
            get => _isNewVersion;
            set
            {
                SetProperty(ref _isNewVersion, value);
            }
        }

        public FirmwarePageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            SetProperty(ref _newVersion, GetString("Non"), nameof(NewVersion));

            var model = Ioc.Default.GetRequiredService<YzProfileModel>();

            Title = GetString("ControllerFirmware");
            ControllerCode = $"{GetString("ControllerCode")}：YZ02";
            //CurrentVersion = $"{GetString("FirmwareVersion")}：V{model.Version.Major}.{model.Version.Minor.ToString().PadLeft(2, '0')}";
            if (model != null && model.Version != null)
            {
                // 在这里安全地访问 model.Version 的属性
                CurrentVersion = $"{GetString("FirmwareVersion")}：V{model.Version.Major}.{model.Version.Minor.ToString().PadLeft(2, '0')}";
            }
            else
            {
                CurrentVersion = $"{GetString("FirmwareVersion")}：{model.Version}";
            }
            CurrentLogs = $"{GetString("FirmwareVersionLogs").Replace("\\r\\n", Environment.NewLine)}";
            if (FwUpdateUtils.Instance.Version != null &&
                FwUpdateUtils.Instance.Version > model.Version)
            {
                IsNewVersion = true;
                NewVersion = $"{GetString("NewVersion")}：{FwUpdateUtils.Instance.Version}";
                NewLogs = FwUpdateUtils.Instance.UpdateLogs;
            }
            else
            {
                IsNewVersion = false;
                NewVersion = $"{GetString("NewVersion")}：{NewVersion}";
            }
        }
    }
}
