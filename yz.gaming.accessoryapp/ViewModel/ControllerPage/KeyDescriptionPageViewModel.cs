using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Languange;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Utils;
using yz.gaming.accessoryapp.ViewModel.Main;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class KeyDescriptionPageViewModel : ViewModelBase, ITipButtomMapSupport, IMenuSupport
    {
        public event Action<Grid> OnSelectedMenuChanged;

        public List<bool> TipButtomMap { get; } = new List<bool> {true, false, false, true, true, false, false, false, true };

        public List<Grid> MenuList { get; set; }

        public Grid CurrentMenu { get; set; }

        public int CurrentIndex { get; set; } = 0;

        const ushort V0 = 0;
        const ushort V10 = 10;
        const ushort V20 = 20;
        const ushort V50 = 50;
        const ushort V100 = 100;
        const ushort V500 = 500;

        public List<ItemModel> DelayMs = new List<ItemModel>
        {
            new ItemModel() { Index = 0, Text = V0.ToString(), Value = V0 },
            new ItemModel() { Index = 0, Text = V100.ToString(), Value = V100 }
        };

        public List<ISelectableItem> MacroItems { get; set; }

        public ISelectableItem SelectedKeyItem { get; set; }
        public int HoverItemIndex { get; set; } = 0;

        Dictionary<KeyCodeEnum, MacroMappingEnum> KeyMacioMappingDic = new Dictionary<KeyCodeEnum, MacroMappingEnum>()
        {
            { KeyCodeEnum.Back, MacroMappingEnum.K_BACK_Pos },
            { KeyCodeEnum.L2, MacroMappingEnum.K_L2_Pos },
            { KeyCodeEnum.L1, MacroMappingEnum.K_L1_Pos },
            { KeyCodeEnum.L3D_UP, MacroMappingEnum.K_UP_Pos },
            { KeyCodeEnum.L3D_DOWN, MacroMappingEnum.K_DOWN_Pos },
            { KeyCodeEnum.L3D_LEFT, MacroMappingEnum.K_LEFT_Pos },
            { KeyCodeEnum.L3D_RIGHT, MacroMappingEnum.K_RIGHT_Pos },
            { KeyCodeEnum.L3, MacroMappingEnum.K_L3_Pos },
            { KeyCodeEnum.DPAD_UP, MacroMappingEnum.K_UP_Pos },
            { KeyCodeEnum.DPAD_DOWN, MacroMappingEnum.K_DOWN_Pos },
            { KeyCodeEnum.DPAD_LEFT, MacroMappingEnum.K_LEFT_Pos },
            { KeyCodeEnum.DPAD_RIGHT, MacroMappingEnum.K_RIGHT_Pos },
            { KeyCodeEnum.Start, MacroMappingEnum.K_START_Pos },
            { KeyCodeEnum.R2, MacroMappingEnum.K_R2_Pos },
            { KeyCodeEnum.R1, MacroMappingEnum.K_R1_Pos },
            { KeyCodeEnum.A, MacroMappingEnum.K_A_Pos },
            { KeyCodeEnum.B, MacroMappingEnum.K_B_Pos },
            { KeyCodeEnum.X, MacroMappingEnum.K_X_Pos },
            { KeyCodeEnum.Y, MacroMappingEnum.K_Y_Pos },
            { KeyCodeEnum.R3, MacroMappingEnum.K_R3_Pos }
        };

        public KeyDescriptionPageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            Title = GetString("KeyDescription");
        }

        public void MenuSelect(int index)
        {
            if (index >= 0 && index < MenuList.Count)
            {
                OnSelectedMenuChanged?.Invoke(MenuList[index]);
            }
        }

        public void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type, out bool isCancel)
        {
            isCancel = false;

            switch (key)
            {
                case KeyCodeEnum.B:
                    if (CurrentIndex == 3 && (SelectedKeyItem != null || HoverItemIndex > 0))
                    {
                        HandleKeyEvent(key, type);
                        isCancel = true;
                    }
                    break;
            }
        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            if (CurrentIndex == 3 && SelectedKeyItem != null 
                && SelectedKeyItem is SelectableTextBlock tmp 
                && type != KeyPressTypeEnmu.AppClick)
            {
                if (type == KeyPressTypeEnmu.ShortPress)
                {
                    if (KeyMacioMappingDic.ContainsKey(key))
                    {
                        SelectedKeyItem = null;
                        tmp.Text = EnumUtility.GetName(KeyMacioMappingDic[key]);
                        tmp.Key = (byte)KeyMacioMappingDic[key];
                        tmp.IsSelected = false;
                    }
                    return;
                }
                else if (type == KeyPressTypeEnmu.LongPress)
                {
                    if (key == KeyCodeEnum.X)
                    {
                        SelectedKeyItem = null;
                        tmp.Text = "NC";
                        tmp.Key = 0xFF;
                        tmp.IsSelected = false;
                        tmp.IsHover = false;
                        return;
                    }
                }
            }

            switch (key)
            {
                case KeyCodeEnum.A:
                    if (HoverItemIndex > 0 && type != KeyPressTypeEnmu.AppClick)
                    {
                        if (MacroItems[HoverItemIndex - 1] is SelectableComboBox cbx)
                        {
                            if (cbx.IsOpen)
                            {
                                cbx.SetDropDownOpen(false);
                            }
                            else
                            {
                                cbx.SetDropDownOpen(true);
                            }
                        }
                        else if (SelectedKeyItem == null)
                        {
                            MacroItems[HoverItemIndex - 1].IsSelected = true;
                        }
                    }
                    else
                    {
                        if (CurrentIndex == 3) SetMacro();
                    }
                    break;
                case KeyCodeEnum.B:
                    if (MacroItems[HoverItemIndex - 1] is SelectableComboBox cbx2 && cbx2.IsOpen)
                    {
                        cbx2.SetDropDownOpen(false);
                    }
                    else
                    {
                        MacroItems[HoverItemIndex - 1].IsHover = false;
                        HoverItemIndex = 0;
                    }
                    break;
                case KeyCodeEnum.L1:
                    if (CurrentIndex > 0)
                    {
                        OnSelectedMenuChanged?.Invoke(MenuList[CurrentIndex - 1]);
                    }
                    break;
                case KeyCodeEnum.R1:
                    if (CurrentIndex < MenuList.Count -1)
                    {
                        OnSelectedMenuChanged?.Invoke(MenuList[CurrentIndex + 1]);
                    }
                    break;
                case KeyCodeEnum.DPAD_UP:
                case KeyCodeEnum.DPAD_DOWN:
                case KeyCodeEnum.DPAD_LEFT:
                case KeyCodeEnum.DPAD_RIGHT:
                    if (CurrentIndex == 3 && 
                        HoverItemIndex > 0 && 
                        MacroItems[HoverItemIndex - 1] is SelectableComboBox cbx3 && 
                        cbx3.IsOpen)
                    {
                        cbx3.MoveSelectItem(-1);
                    }
                    else if (CurrentIndex == 3)
                    {
                        if (HoverItemIndex != 0)
                        {
                            MacroItems[HoverItemIndex - 1].IsHover = false;
                        }

                        ISelectableItem item;

                        switch (key)
                        {
                            case KeyCodeEnum.DPAD_UP:
                                HoverItemIndex = HoverItemIndex - 7 < 1 ? HoverItemIndex : HoverItemIndex - 7;
                                break;
                            case KeyCodeEnum.DPAD_LEFT:
                                HoverItemIndex = HoverItemIndex - 1 < 1 ? MacroItems.Count : HoverItemIndex - 1;
                                break;
                            case KeyCodeEnum.DPAD_RIGHT:
                                HoverItemIndex = HoverItemIndex + 1 > MacroItems.Count ? 1 : HoverItemIndex + 1;
                                break;
                            case KeyCodeEnum.DPAD_DOWN:
                                HoverItemIndex = HoverItemIndex + 7 > MacroItems.Count ? HoverItemIndex : HoverItemIndex + 7;
                                break;
                        }

                        item = MacroItems[HoverItemIndex - 1];
                        item.IsHover = true;
                    }
                    break;
                default:
                    base.HandleKeyEvent(key, type);
                    break;
            }
        }

        public override void HandleThumbStatusEvent(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            if (CurrentIndex == 3 && SelectedKeyItem != null)
            {
                if (SelectedKeyItem is SelectableTextBlock tmp)
                {
                    SelectedKeyItem = null;

                    switch (direction)
                    {
                        case ThumbDirectionEnmu.UP:
                            tmp.Text = EnumUtility.GetName(MacroMappingEnum.K_UP_Pos);
                            tmp.Key = (byte)MacroMappingEnum.K_UP_Pos;
                            break;
                        case ThumbDirectionEnmu.DOWN:
                            tmp.Text = EnumUtility.GetName(MacroMappingEnum.K_DOWN_Pos);
                            tmp.Key = (byte)MacroMappingEnum.K_DOWN_Pos;
                            break;
                        case ThumbDirectionEnmu.LEFT:
                            tmp.Text = EnumUtility.GetName(MacroMappingEnum.K_LEFT_Pos);
                            tmp.Key = (byte)MacroMappingEnum.K_LEFT_Pos;
                            break;
                        case ThumbDirectionEnmu.RIGHT:
                            tmp.Text = EnumUtility.GetName(MacroMappingEnum.K_RIGHT_Pos);
                            tmp.Key = (byte)MacroMappingEnum.K_RIGHT_Pos;
                            break;
                    }

                    tmp.IsSelected = false;
                    return;
                }
            }
            else
            {
                if (HoverItemIndex > 0 && MacroItems[HoverItemIndex - 1] is SelectableComboBox cbx && cbx.IsOpen)
                {
                    switch (direction)
                    {
                        case ThumbDirectionEnmu.UP:
                        case ThumbDirectionEnmu.LEFT:
                            cbx.MoveSelectItem(-1);
                            break;
                        case ThumbDirectionEnmu.RIGHT:
                        case ThumbDirectionEnmu.DOWN:
                            cbx.MoveSelectItem(1);
                            break;
                    }
                }
                else
                {
                    if (SelectedKeyItem != null)
                    {
                        SelectedKeyItem.IsSelected = false;
                        SelectedKeyItem = null;
                    }

                    if (HoverItemIndex != 0)
                    {
                        MacroItems[HoverItemIndex - 1].IsHover = false;
                    }

                    ISelectableItem item;

                    switch (direction)
                    {
                        case ThumbDirectionEnmu.UP:
                            HoverItemIndex = HoverItemIndex - 7 < 1 ? HoverItemIndex : HoverItemIndex - 7;
                            break;
                        case ThumbDirectionEnmu.LEFT:
                            HoverItemIndex = HoverItemIndex - 1 < 1 ? MacroItems.Count : HoverItemIndex - 1;
                            break;
                        case ThumbDirectionEnmu.RIGHT:
                            HoverItemIndex = HoverItemIndex + 1 > MacroItems.Count ? 1 : HoverItemIndex + 1;
                            break;
                        case ThumbDirectionEnmu.DOWN:
                            HoverItemIndex = HoverItemIndex + 7 > MacroItems.Count ? HoverItemIndex : HoverItemIndex + 7;
                            break;
                    }

                    item = MacroItems[HoverItemIndex - 1];
                    item.IsHover = true;
                }
            }
        }

        public void SetMacro()
        {
            int index = 0;
            // M1
            Model.BackKey.M1.CycleFlag = 0x00;
            Model.BackKey.M1.CycleInterval = 0x00;
            Model.BackKey.M1.Step.Key = (MacroItems[index++] as SelectableTextBlock).Key;
            Model.BackKey.M1.Step.Type = 1;
            Model.BackKey.M1.Step.Time = 10;
            Model.BackKey.M1.Step.Interval = (ushort)((MacroItems[index++] as SelectableComboBox).SelectedItem as ItemModel).Value;
            ClearStep(Model.BackKey.M1.Step);
            MacroStep M1Next = Model.BackKey.M1.Step;
            while (index <= 6 && (MacroItems[index] as SelectableTextBlock).Key != 0xFF)
            {
                var tmp = new MacroStep()
                {
                    Key = (MacroItems[index++] as SelectableTextBlock).Key,
                    Type = 1,
                    Time = 10,
                    Interval = index < 7 ? (ushort)((MacroItems[index++] as SelectableComboBox).SelectedItem as ItemModel).Value : (ushort)0 
                };

                M1Next.Next = tmp;
                M1Next = tmp;
            }

            index = 7;
            // M2
            Model.BackKey.M2.CycleFlag = 0x00;
            Model.BackKey.M2.CycleInterval = 0x00;
            Model.BackKey.M2.Step.Key = (MacroItems[index++] as SelectableTextBlock).Key;
            Model.BackKey.M2.Step.Type = 1;
            Model.BackKey.M2.Step.Time = 10;
            Model.BackKey.M2.Step.Interval = (ushort)((MacroItems[index++] as SelectableComboBox).SelectedItem as ItemModel).Value;
            ClearStep(Model.BackKey.M2.Step);
            MacroStep M2Next = Model.BackKey.M2.Step;
            while (index <= 13 && (MacroItems[index] as SelectableTextBlock).Key != 0xFF)
            {
                var tmp = new MacroStep()
                {
                    Key = (MacroItems[index++] as SelectableTextBlock).Key,
                    Type = 1,
                    Time = 10,
                    Interval = index < 13 ? (ushort)((MacroItems[index++] as SelectableComboBox).SelectedItem as ItemModel).Value : (ushort)0
                };

                M2Next.Next = tmp;
                M2Next = tmp;
            }

            SaveProfile();
            ShowAlertUtils.ShowAlert(LanguangeManager.Instance.GetString("SaveSuccess"));
        }

        private MacroStep ClearStep(MacroStep step)
        {
            if (step.Next != null)
            {
                var tmp = ClearStep(step.Next);
                tmp.Next = null;
            }

            step.Next = null;
            return step;
        }
    }
}
