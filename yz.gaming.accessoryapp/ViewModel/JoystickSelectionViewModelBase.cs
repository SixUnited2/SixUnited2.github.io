using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel
{
    public class JoystickSelectionViewModelBase : ListItemSupportViewModelBase, IListItemSupport
    {
        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            switch (key)
            {
                case KeyCodeEnum.DPAD_LEFT:
                    if (CurrentItem is ISliderPageListItem sliderItem)
                    {
                        if (CurrentItem.Equals(HovedItem))
                        {
                            sliderItem.Value = sliderItem.Value - 1;
                        }
                    }
                    else if (CurrentItem is IArrayPageListItem arrayItem)
                    {
                        arrayItem.SelectPrev();
                    }
                    else
                    {
                        CurrentItem.ConfirmPressed();
                    }
                    break;
                case KeyCodeEnum.DPAD_RIGHT:
                    if (CurrentItem is ISliderPageListItem sliderItem1)
                    {
                        if (CurrentItem.Equals(HovedItem))
                        {
                            sliderItem1.Value = sliderItem1.Value + 1;
                        }
                    }
                    else if (CurrentItem is IArrayPageListItem arrayItem)
                    {
                        arrayItem.SelectNext();
                    }
                    else
                    {
                        CurrentItem.ConfirmPressed();
                    }
                    break;
                case KeyCodeEnum.X:
                    if (CurrentItem is ISliderPageListItem sliderItem2)
                    {
                        if (CurrentItem.Equals(HovedItem))
                        {
                            sliderItem2.RollbackValue();
                        }
                    }
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
                    if (CurrentItem is ISliderPageListItem sliderItem)
                    {
                        if (CurrentItem.Equals(HovedItem))
                        {
                            sliderItem.Value = sliderItem.Value - 1;
                        }
                    }
                    else if (CurrentItem is IArrayPageListItem arrayItem)
                    {
                        arrayItem.SelectPrev();
                    }
                    else
                    {
                        CurrentItem.ConfirmPressed();
                    }
                    break;
                case ThumbDirectionEnmu.RIGHT:
                    if (CurrentItem is ISliderPageListItem sliderItem1)
                    {
                        if (CurrentItem.Equals(HovedItem))
                        {
                            sliderItem1.Value = sliderItem1.Value + 1;
                        }
                    }
                    else if (CurrentItem is IArrayPageListItem arrayItem)
                    {
                        arrayItem.SelectNext();
                    }
                    else
                    {
                        CurrentItem.ConfirmPressed();
                    }
                    break;
                default:
                    base.HandleThumbStatusEvent(key, direction);
                    break;
            }
        }
    }
}
