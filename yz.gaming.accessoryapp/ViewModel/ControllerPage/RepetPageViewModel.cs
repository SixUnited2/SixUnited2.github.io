using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class RepetPageViewModel : JoystickSelectionViewModelBase, ITipButtomMapSupport
    {
        public List<bool> TipButtomMap => new List<bool> { false, false, true, true, true, false, false, false, false };

        bool _turboOpen;
        public bool TurboOpen
        {
            get => _turboOpen;
            set
            {
                SetProperty(ref _turboOpen, value);
                Model.TurboOpen = (byte)(value ? 1 : 0);
                SaveProfile();
            }
        }

        bool _turboA;
        public bool TurboA
        {
            get => _turboA;
            set
            {
                SetProperty(ref _turboA, value);
                Model.TurboA = (byte)(value ? 1 : 0);
                SaveProfile();
            }
        }

        bool _turboB;
        public bool TurboB
        {
            get => _turboB;
            set
            {
                SetProperty(ref _turboB, value);
                Model.TurboB = (byte)(value ? 1 : 0);
                SaveProfile();
            }
        }

        bool _turboX;
        public bool TurboX
        {
            get => _turboX;
            set
            {
                SetProperty(ref _turboX, value);
                Model.TurboX = (byte)(value ? 1 : 0);
                SaveProfile();
            }
        }

        bool _turboY;
        public bool TurboY
        {
            get => _turboY;
            set
            {
                SetProperty(ref _turboY, value);
                Model.TurboY = (byte)(value ? 1 : 0);
                SaveProfile();
            }
        }

        bool _turboL1;
        public bool TurboL1
        {
            get => _turboL1;
            set
            {
                SetProperty(ref _turboL1, value);
                Model.TurboL1 = (byte)(value ? 1 : 0);
                SaveProfile();
            }
        }

        bool _turboL2;
        public bool TurboL2
        {
            get => _turboL2;
            set
            {
                SetProperty(ref _turboL2, value);
                Model.TurboR1 = (byte)(value ? 1 : 0);
                SaveProfile();
            }
        }

        public RepetPageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            base.Initialization();
            Title = GetString("RepeatMode");
            SetProperty(ref _turboOpen, Model.TurboOpen == 1, nameof(TurboOpen));
            SetProperty(ref _turboA, Model.TurboA == 1, nameof(TurboA));
            SetProperty(ref _turboB, Model.TurboB == 1, nameof(TurboB));
            SetProperty(ref _turboX, Model.TurboX == 1, nameof(TurboX));
            SetProperty(ref _turboY, Model.TurboY == 1, nameof(TurboY));
            SetProperty(ref _turboL1, Model.TurboL1 == 1, nameof(TurboL1));
            SetProperty(ref _turboL2, Model.TurboR1 == 1, nameof(TurboL2));
        }
    }
}
