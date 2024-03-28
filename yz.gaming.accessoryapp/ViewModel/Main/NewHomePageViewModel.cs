using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Service;
using yz.gaming.accessoryapp.Utils;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel.Main
{
    public class NewHomePageViewModel : ChildPageSupportViewModelBase, ITipButtomMapSupport
    {
        const int DEFAULT_CORNER_RADIUS = 16;
        int _cornerRadius = DEFAULT_CORNER_RADIUS;

        public int CornerRadius
        {
            get => _cornerRadius;
            set => SetProperty(ref _cornerRadius, value);
        }

        public IPageListItem TopButton { get; set; }
        public Action OnTipButtomMapChanged { get; set; }
        public List<bool> TipButtomMap { get; } = new List<bool> { true, true, true, true, true, false, false, false, false };

        public NewHomePageViewModel()
            : base()
        {
        }

        public override void OnButtonSelectedStateChange(IPageListItem sender, bool isSelected)
        {
            base.OnButtonSelectedStateChange(sender, isSelected);

            if (isSelected && !sender.Equals(TopButton))
            {
                TopButton.IsSelected = false;
            }

            TipButtomMap[7] = ListItems.Count > 0 && !CurrentItem.Equals(TopButton);
            OnTipButtomMapChanged?.Invoke();
        }

        public override void OnButtonHovedStateChange(IPageListItem sender, bool isHoved)
        {
            base.OnButtonHovedStateChange(sender, isHoved);

            if (isHoved && !sender.Equals(TopButton))
            {
                TopButton.IsHoved = false;
            }
        }

        public void SetButtonSize()
        {
            CornerRadius = Convert.ToInt32(Math.Round(DEFAULT_CORNER_RADIUS / SystemUtils.Instance.GetScreenScalingFactor()));
        }

        public override void Initialization()
        {
            base.Initialization();
            ListItems.Remove(TopButton);
            Title = GetString("Home");

            if (PreviousItem == null)
            {
                if (ListItems.Count > 0)
                {
                    ListItems[0].IsHoved = true;
                    ListItems[0].IsSelected = true;
                }
                else
                {
                    TopButton.IsHoved = true;
                    TopButton.IsSelected = true;
                    CurrentItem = TopButton;
                    HovedItem = TopButton;
                }
            }
        }

        public void PlatformClick(IPageListItem sender)
        {
            if (sender is DynamicButtonControlHV item)
            {
                if (item.Tag is PlatformEnum tag)
                {
                    var platform = GamePlatform.Instance.GetPlatformModel(tag);
                    if (platform.IsInstall && !string.IsNullOrEmpty(platform.Path))
                    {
                        GamePlatform.Instance.Start(platform);
                    }
                    else
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(platform.PackageName))
                                {
                                    _logger.Trace($"Open app store => {platform.PackageName}");
                                    await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={platform.PackageName}"));
                                }
                                else
                                {
                                    _logger.Trace($"Open url => {platform.Url}");
                                    await Windows.System.Launcher.LaunchUriAsync(new Uri(platform.Url));
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex.Message);
                                _logger.Error(ex.StackTrace);
                            }
                        });

                        GamePlatform.Instance.MiniWindows();
                    }
                }
            }
        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            switch (key)
            {
                case KeyCodeEnum.DPAD_LEFT:
                    if (ListItems.Count > 0) MoveToPrevItem();
                    break;
                case KeyCodeEnum.DPAD_RIGHT:
                    if (ListItems.Count > 0) MoveToNextItem();
                    break;
                case KeyCodeEnum.DPAD_UP:
                    if (!TopButton.IsHoved)
                    {
                        TopButton.IsHoved = true;
                        ListItems.ForEach(p => p.IsHoved = false);
                        CurrentItem = TopButton;
                        HovedItem = TopButton;
                    }
                    break;
                case KeyCodeEnum.DPAD_DOWN:
                    if (TopButton.IsHoved && ListItems.Count > 0)
                    {
                        int index = ListItems.Count > 2 ? 2 : ListItems.Count - 1;
                        TopButton.IsHoved = false;
                        ListItems[index].IsHoved = true;
                    }
                    break;
                case KeyCodeEnum.X:
                    if (TipButtomMap[7] && CurrentItem is DynamicButtonControlHV ctr)
                    {
                        GamePlatform.Instance.RemoveFormHomePage((PlatformEnum)ctr.Tag);
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
                    if (ListItems.Count > 0) MoveToPrevItem();
                    break;
                case ThumbDirectionEnmu.RIGHT:
                    if (ListItems.Count > 0) MoveToNextItem();
                    break;
                case ThumbDirectionEnmu.UP:
                    if (!TopButton.IsHoved)
                    {
                        TopButton.IsHoved = true;
                        ListItems.ForEach(p => p.IsHoved = false);
                        CurrentItem = TopButton;
                        HovedItem = TopButton;
                    }
                    break;
                case ThumbDirectionEnmu.DOWN:
                    if (TopButton.IsHoved && ListItems.Count > 0)
                    {
                        int index = ListItems.Count > 2 ? 2 : ListItems.Count - 1;
                        TopButton.IsHoved = false;
                        ListItems[index].IsHoved = true;
                    }
                    break;
                default:
                    base.HandleThumbStatusEvent(key, direction);
                    break;
            }
        }
    }
}
