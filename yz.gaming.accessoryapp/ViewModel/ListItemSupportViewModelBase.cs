using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel
{
    public class ListItemSupportViewModelBase : ViewModelBase, IListItemSupport
    {
        public List<IPageListItem> ListItems { get; set; }

        public IPageListItem CurrentItem { get; set; }
        public IPageListItem HovedItem { get; set; }
        public IPageListItem PreviousItem { get; set; }

        public virtual void OnButtonSelectedStateChange(IPageListItem sender, bool isSelected)
        {
            if (isSelected)
            {
                ListItems.ForEach(p =>
                {
                    if (!p.Equals(sender))
                    {
                        p.IsSelected = false;
                    }
                });

                CurrentItem = sender;
            }
        }

        public virtual void OnButtonHovedStateChange(IPageListItem sender, bool isHoved)
        {
            if (isHoved)
            {
                ListItems.ForEach(p =>
                {
                    if (!p.Equals(sender))
                    {
                        p.IsHoved = false;
                    }
                });

                HovedItem = sender;
                PreviousItem = HovedItem;
            }
        }

        public virtual void MoveToNextItem()
        {
            var item = HovedItem.Index + 1 < ListItems.Count ? ListItems[HovedItem.Index + 1] : ListItems[0];
            item.IsHoved = true;

            if (!CurrentItem.Equals(item))
            {
                CurrentItem.IsSelected = false;
            }

            CurrentItem = item;
        }

        public virtual void MoveToItem(int index)
        {
            if (index > ListItems.Count) index = ListItems.Count - 1;
            if (index < 0) index = 0;

            var item = ListItems[index];
            item.IsHoved = true;

            if (!CurrentItem.Equals(item))
            {
                CurrentItem.IsSelected = false;
            }
        }

        public virtual void MoveToPrevItem()
        {
            var item = HovedItem.Index - 1 >= 0 ? ListItems[HovedItem.Index - 1] : ListItems[ListItems.Count - 1];
            item.IsHoved = true;

            if (!CurrentItem.Equals(item))
            {
                CurrentItem.IsSelected = false;
            }

            CurrentItem = item;
        }

        public virtual void OnButtonClick(IPageListItem sender)
        {
        }

        public virtual void UnSelectAllItem()
        {
            ListItems.ForEach(p => p.IsSelected = false);
            CurrentItem = ListItems[0];
        }

        public override void Initialization()
        {
            CurrentItem = PreviousItem == null ? ListItems[0] : PreviousItem;
            HovedItem = PreviousItem == null ? ListItems[0] : PreviousItem;
        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            switch (key)
            {
                case KeyCodeEnum.DPAD_UP:
                case KeyCodeEnum.DPAD_LEFT:
                    if (CurrentItem.IsSelected &&
                        CurrentItem is QuickMenuComboBox comboBoxUp &&
                        key == KeyCodeEnum.DPAD_UP)
                    {
                        comboBoxUp.MoveSelectItem(-1);
                    }
                    else
                    {
                        MoveToPrevItem();
                    }
                    break;
                case KeyCodeEnum.DPAD_RIGHT:
                case KeyCodeEnum.DPAD_DOWN:
                    if (CurrentItem.IsSelected &&
                       CurrentItem is QuickMenuComboBox comboBoxDown &&
                       key == KeyCodeEnum.DPAD_UP)
                    {
                        comboBoxDown.MoveSelectItem(1);
                    }
                    else
                    {
                        MoveToNextItem();
                    }
                    break;
                case KeyCodeEnum.A:
                    if (CurrentItem.Equals(HovedItem) || HovedItem == null)
                    {
                        if (CurrentItem.IsSelected)
                        {
                            CurrentItem?.ConfirmPressed();
                        }
                        else if (!CurrentItem.IsSelected &&
                            CurrentItem is QuickMenuComboBox quickMenuComboBox1)
                        {
                            quickMenuComboBox1.ConfirmPressed();
                        }
                        else
                        {
                            CurrentItem.IsSelected = true;
                            if (CurrentItem is CenterSliderListItem sliderListItem)
                            {
                                sliderListItem.ConfirmPressed();
                            }
                        }
                    }
                    else if (HovedItem != null)
                    {
                        HovedItem.IsSelected = true;
                    }
                    break;
                case KeyCodeEnum.B:
                    if (CurrentItem != null)
                    {
                        if (CurrentItem is QuickMenuComboBox comboBox) comboBox.IsNeedSave = false;
                        CurrentItem.IsSelected = false;
                    }
                    break;
            }
        }

        public override void HandleThumbStatusEvent(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            switch (direction)
            {
                case ThumbDirectionEnmu.UP:
                case ThumbDirectionEnmu.LEFT:
                    if (CurrentItem.IsSelected &&
                        CurrentItem is QuickMenuComboBox comboBoxUp &&
                        direction == ThumbDirectionEnmu.UP)
                    {
                        comboBoxUp.MoveSelectItem(-1);
                    }
                    else
                    {
                        MoveToPrevItem();
                    }
                    break;
                case ThumbDirectionEnmu.DOWN:
                case ThumbDirectionEnmu.RIGHT:
                    if (CurrentItem.IsSelected &&
                        CurrentItem is QuickMenuComboBox comboBoxDown &&
                        direction == ThumbDirectionEnmu.DOWN)
                    {
                        comboBoxDown.MoveSelectItem(1);
                    }
                    else
                    {
                        MoveToNextItem();
                    }
                    break;
            }
        }
    }
}
