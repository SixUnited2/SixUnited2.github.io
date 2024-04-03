using System;
using System.Collections.Generic;
using System.Text;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.ViewModel.Main;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class ShakePageViewModel : JoystickSelectionViewModelBase, ITipButtomMapSupport
    {
        public List<bool> TipButtomMap { get; } = new List<bool> {false, false, false, true, true, false, false, false, false };

        byte _motorIntensity;
        public byte MotorIntensity
        {
            get => _motorIntensity;
            set
            {
                SetProperty(ref _motorIntensity, value);
                Model.MotorIntensity = value;
                SaveProfile();
            }
        }

        public ShakePageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            base.Initialization();
            Title = GetString("Shake");
            SetProperty(ref _motorIntensity, Model.MotorIntensity, nameof(MotorIntensity));
        }

        private void OnProfileReresh(YzProfileModel model)
        {
            SetProperty(ref _motorIntensity, model.MotorIntensity, nameof(MotorIntensity));
        }
    }
}
