using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using yz.gaming.accessoryapp.Model;
using System.Windows.Controls;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel.ControllerPage
{
    public class LanguagePageViewModel : ViewModelBase, ITipButtomMapSupport
    {
        public List<bool> TipButtomMap => new List<bool> { false, false, false, true, true, false, false, false, false };

        public List<ItemModel> LanguageData;

        public ComboBox LanguageComboBox { get; set; }
        public bool IsNeedSave { get; set; }

        public object SelectedItem { get; set; }

        public LanguagePageViewModel()
            : base()
        {
            LanguageData = new List<ItemModel>
            {
                new ItemModel() { Index = 0, Text = GetString("Chinese"), Value = "zh-Hans" },
                new ItemModel() { Index = 1, Text = GetString("English"), Value = "en-US" }
            };
        }

        public ItemModel GetModelByValue(string lang)
        {
            ItemModel l = LanguageData.Where(p => p.Value.ToString() == lang).FirstOrDefault();

            if (l == null) l = LanguageData[0];

            return l;
        }

        public override void Initialization()
        {
            Title = GetString("Language");
        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            switch (key)
            {
                case KeyCodeEnum.A:
                    if (type == KeyPressTypeEnmu.ShortPress || type == KeyPressTypeEnmu.AppClick)
                    {
                        LanguageComboBox.IsDropDownOpen = !LanguageComboBox.IsDropDownOpen;
                        IsNeedSave = true;
                    }
                    break;
            }
        }

        public void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type, out bool isCancel)
        {
            isCancel = false;

            switch (key)
            {
                case KeyCodeEnum.B:
                    if ((type == KeyPressTypeEnmu.ShortPress || type == KeyPressTypeEnmu.AppClick)
                        && LanguageComboBox.IsDropDownOpen)
                    {
                        LanguageComboBox.IsDropDownOpen = false;
                        isCancel = true;
                        IsNeedSave = false;
                    }
                    break;
            }
        }

        public override void HandleThumbStatusEvent(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            if (LanguageComboBox.IsDropDownOpen == false) return;

            switch (direction)
            {
                case ThumbDirectionEnmu.UP:
                    if (LanguageComboBox.SelectedIndex > 0)
                    {
                        LanguageComboBox.SelectedIndex -= 1;
                    }
                    break;
                case ThumbDirectionEnmu.DOWN:
                    if (LanguageComboBox.SelectedIndex < LanguageComboBox.Items.Count - 1)
                    {
                        LanguageComboBox.SelectedIndex += 1;
                    }
                    break;
                case ThumbDirectionEnmu.LEFT:
                case ThumbDirectionEnmu.RIGHT:
                    break;
            }
        }
    }
}
