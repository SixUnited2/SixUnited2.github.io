using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.ViewModel.Main;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class LightEffectPageViewModel : JoystickSelectionViewModelBase, ITipButtomMapSupport
    {
        double _buttonWidth = 172;
        double _buttonHeight = 216;

        public double ButtonWidth
        {
            get => _buttonWidth;
            set => SetProperty(ref _buttonWidth, value);
        }
        public double ButtonHeight
        {
            get => _buttonHeight;
            set => SetProperty(ref _buttonHeight, value);
        }

        public List<bool> TipButtomMap { get; } = new List<bool> {false, false, true, true, true, false, false, false, false };

        byte _lightIntensity;
        public byte LightIntensity
        {
            get => _lightIntensity;
            set
            {
                SetProperty(ref _lightIntensity, value);
                Model.LightIntensity = value;
                SaveProfile();
            }
        }

        LightTypeEnum _lightType;
        public LightTypeEnum LightType
        {
            get => _lightType;
            set
            {
                SetProperty(ref _lightType, value);
                Model.LightType = value;
                SaveProfile();
            }
        }

        Color _lightColor;
        public Color LightColor
        {
            get => _lightColor;
            set
            {
                SetProperty(ref _lightColor, value);
                Model.LightColor = value;
                SaveProfile();
            }
        }

        public LightEffectPageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            base.Initialization();
            Title = GetString("Light");
            SetProperty(ref _lightIntensity, Model.LightIntensity, nameof(LightIntensity));
            SetProperty(ref _lightType, Model.LightType, nameof(LightType));
            SetProperty(ref _lightColor, Model.LightColor, nameof(LightColor));
        }

        public void SetButtonSize(double width)
        {
            ButtonWidth = (width / 2) / 8;
            ButtonHeight = _buttonWidth * 1.5;
        }
    }
}
