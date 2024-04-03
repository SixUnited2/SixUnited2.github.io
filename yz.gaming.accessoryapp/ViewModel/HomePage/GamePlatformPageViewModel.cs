using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Service;
using yz.gaming.accessoryapp.Utils;
using static yz.gaming.accessoryapp.Api.YzCommonApi;
using System.Diagnostics;
using Windows.System;

namespace yz.gaming.accessoryapp.ViewModel.HomePage
{
    public class GamePlatformPageViewModel : ListItemSupportViewModelBase, ITipButtomMapSupport
    {
        const int DEFAULT_CORNER_RADIUS = 12;

        double _buttonWidth = 172;
        double _buttonHeight = 216;
        int _cornerRadius = DEFAULT_CORNER_RADIUS;

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

        public int CornerRadius
        {
            get => _cornerRadius;
            set => SetProperty(ref _cornerRadius, value);
        }

        public List<bool> TipButtomMap { get; } = new List<bool> { false, false, true, true, true, true, false, false, false };
        public Action OnTipButtomMapChanged { get; set; }

        public GamePlatformPageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            Title = GetString("GamePlatform");
        }

        public void SetButtonSize(double pageWidth)
        {
            ButtonWidth = pageWidth / 10;
            ButtonHeight = _buttonWidth * 1.2681;
            CornerRadius = Convert.ToInt32(Math.Round(DEFAULT_CORNER_RADIUS / SystemUtils.Instance.GetScreenScalingFactor()));
        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            switch (key)
            {
                case KeyCodeEnum.X:
                    if (TipButtomMap[5] && CurrentItem is DynamicButtonControl ctr)
                    {
                        if (int.TryParse(ctr.Tag.ToString(), out int tag))
                        {
                            var platform = (PlatformEnum)tag;
                            GamePlatform.Instance.SendToHomePage(platform, out string msg);
                            ShowAlertUtils.ShowAlert(msg);
                            TipButtomMap[5] = !GamePlatform.Instance.HomePlatforms.Exists(p => p.Platform == platform);
                            OnTipButtomMapChanged?.Invoke();
                        }
                    }
                    break;
                default:
                    base.HandleKeyEvent(key, type);
                    break;
            }
        }

        public void PlatformClick(IPageListItem sender)
        {
            if (sender is DynamicButtonControl item)
            {
                if (int.TryParse(item.Tag.ToString(), out int tag))
                {
                    var platform = GamePlatform.Instance.GetPlatformModel((PlatformEnum)tag);
                    if (platform.IsInstall)
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
                                    await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={platform.PackageName}"));
                                }
                                else
                                {
                                    _logger.Trace($"Open url => {platform.Url}");
                                    await Launcher.LaunchUriAsync(new Uri(platform.Url));
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

        public override void OnButtonSelectedStateChange(IPageListItem sender, bool isSelected)
        {
            base.OnButtonSelectedStateChange(sender, isSelected);

            if (isSelected)
            {
                var platform = (PlatformEnum)int.Parse((sender as DynamicButtonControl).Tag.ToString());
                TipButtomMap[5] = !GamePlatform.Instance.HomePlatforms.Exists(p => p.Platform == platform);
                OnTipButtomMapChanged?.Invoke();
            }
        }
    }
}
