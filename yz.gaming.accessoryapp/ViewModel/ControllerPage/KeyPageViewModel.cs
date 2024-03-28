using System;
using System.Collections.Generic;
using System.Text;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.ViewModel.Main;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class KeyPageViewModel : ChildPageSupportViewModelBase, ITipButtomMapSupport
    {
        public List<bool> TipButtomMap { get; } = new List<bool> { false, false, true, true, true, false, false, false, false };

        byte _controllerMode;
        public byte ControllerMode
        {
            get => _controllerMode;
            set
            {
                SetProperty(ref _controllerMode, value);

                if (Model.ControllerMode != value)
                {
                    Model.ControllerMode = value;
                    SaveProfile();
                }
            }
        }

        bool _useNintendoLayout;
        public bool UseNintendoLayout
        {
            get => _useNintendoLayout;
            set
            {
                SetProperty(ref _useNintendoLayout, value);

                if (Model.KeyLayout != (byte)(value ? 1 : 0))
                {
                    Model.KeyLayout = (byte)(value ? 1 : 0);
                    SaveProfile();
                }
            }
        }

        public KeyPageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            base.Initialization();
            Title = GetString("Key");
            SetProperty(ref _controllerMode, Model.ControllerMode, nameof(ControllerMode));
            SetProperty(ref _useNintendoLayout, Model.KeyLayout == 1, nameof(UseNintendoLayout));
        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            switch (key)
            {
                case KeyCodeEnum.DPAD_LEFT:
                    //CurrentItem?.ConfirmPressed();
                    break;
                case KeyCodeEnum.DPAD_RIGHT:
                    //CurrentItem?.ConfirmPressed();
                    break;
                default:
                    base.HandleKeyEvent(key, type);
                    break;
            }
        }

        public override void HandleThumbStatusEvent(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            switch (direction)
            {
                case ThumbDirectionEnmu.LEFT:
                    //CurrentItem?.ConfirmPressed();
                    break;
                case ThumbDirectionEnmu.RIGHT:
                    //CurrentItem?.ConfirmPressed();
                    break;
                default:
                    base.HandleThumbStatusEvent(key, direction);
                    break;
            }
        }
    }
}
