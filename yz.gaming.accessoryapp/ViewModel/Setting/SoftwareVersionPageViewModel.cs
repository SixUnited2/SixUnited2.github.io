using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Utils;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class SoftwareVersionPageViewModel : ViewModelBase, ITipButtomMapSupport
    {
        public List<bool> TipButtomMap => new List<bool> { false, false, false, true, true, false, false, false, false };

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

        public SoftwareVersionPageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            SetProperty(ref _newVersion, GetString("Non"), nameof(NewVersion));

            var version = Application.ResourceAssembly.GetName().Version;

            Title = GetString("SoftwareVersion");
            CurrentVersion = $"{GetString("Version")}：{version}";
            CurrentLogs = $"{GetString("SoftwareVersionLogs").Replace("\\r\\n", Environment.NewLine)}";

            if (UpdateUtils.Instance.Version != null &&
                UpdateUtils.Instance.Version > version) 
            {
                IsNewVersion = true;
                NewVersion = $"{GetString("NewVersion")}：{UpdateUtils.Instance.Version}";
                NewLogs = UpdateUtils.Instance.UpdateLogs;
            }
            else
            {
                IsNewVersion = false;
                NewVersion = $"{GetString("NewVersion")}：{NewVersion}";
            }
        }
    }
}
