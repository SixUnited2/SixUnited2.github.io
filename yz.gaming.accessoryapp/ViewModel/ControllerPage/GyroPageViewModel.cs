using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.ViewModel.Main;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class GyroPageViewModel : JoystickSelectionViewModelBase, ITipButtomMapSupport
    {
        public List<bool> TipButtomMap { get; } = new List<bool> {false, false, false, true, true, false, false, false, false };

        bool _emulateMouse;
        public bool EmulateMouse
        {
            get => _emulateMouse;
            set
            {
                SetProperty(ref _emulateMouse, value);
            }
        }

        byte _sensitivities;
        public byte Sensitivities
        {
            get => _sensitivities;
            set
            {
                SetProperty(ref _sensitivities, value);
            }
        }

        public GyroPageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            Title = GetString("Gyro");
            SetProperty(ref _emulateMouse, false, nameof(EmulateMouse));
            SetProperty(ref _sensitivities, (byte)0, nameof(Sensitivities));

            Model.OnProfileReresh += OnProfileReresh;
        }

        private void OnProfileReresh(YzProfileModel model)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SetProperty(ref _emulateMouse, false, nameof(EmulateMouse));
                SetProperty(ref _sensitivities, (byte)0, nameof(Sensitivities));
            }));
        }
    }
}
