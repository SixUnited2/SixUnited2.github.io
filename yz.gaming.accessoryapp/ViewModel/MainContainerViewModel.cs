using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.ViewModel.Main;
using yz.gaming.accessoryapp.Utils.Command;
using yz.gaming.accessoryapp.View;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using static yz.gaming.accessoryapp.Api.YzCommonApi;
using yz.gaming.accessoryapp.Service;
using System.Windows;
using NLog;
using yz.gaming.accessoryapp.ViewModel.ControllerPage;

namespace yz.gaming.accessoryapp.ViewModel
{
    public class MainContainerViewModel : NavigationSupportViewModel
    {
        static readonly List<bool> TIP_BUTTON_DEFAULT = new List<bool> {true, true, true, true, true, false, false, false, false };

        List<bool> _tipButtonVisible;
        int _navigatedLayer = 0;
        Stack<IPageViewInterface> _navigationPages;
        Delegate _handleKeyEvent;
        Delegate _handleThumbStatusEvent;

        public int NavigatedLayer
        {
            get { return _navigatedLayer; }
            set
            {
                if (SetProperty(ref _navigatedLayer, value))
                {
                    OnPropertyChanged(nameof(IsFirstLayer));
                    OnPropertyChanged(nameof(IsNavigationSupport));
                }
            }
        }

        public List<bool> TipButtonVisible
        {
            get { return _tipButtonVisible; }
            set { SetProperty(ref _tipButtonVisible, value); }
        
        }

        public bool IsFirstLayer { get => NavigatedLayer == 0; }
        public bool IsNavigationSupport { get => NavigatedLayer == 0; }

        public Dictionary<INavigationItem, IChildPageSupport> ChildPageMap { get; set; }

        public MainContainerViewModel()
        {
            Title = "";
            TipButtonVisible = TIP_BUTTON_DEFAULT;

            _handleKeyEvent = new Action<KeyCodeEnum, KeyPressTypeEnmu>(HandleKeyEvent);
            _handleThumbStatusEvent = new Action<ThumbKeyEnmu, ThumbDirectionEnmu>(HandleThumbStatusEvent);
        }

        public override void InitNavigationData()
        {
            base.InitNavigationData();

            ChildPageMap = new Dictionary<INavigationItem, IChildPageSupport>();
            MenuList.ForEach(p => ChildPageMap.Add(p, (IChildPageSupport)PageList[p.Index].ViewModel));
            _navigationPages = new Stack<IPageViewInterface>();

            YzGamingService.Instance.OnDpadKeyPress += OnDpadKeyPress;
            YzGamingService.Instance.OnThumbStatusReport += OnThumbStatusReport;
        }

        public override void NavigationTo(int index)
        {
            if (index >= 0 && index < MenuList.Count)
            {
                base.NavigationTo(index);
                if (CurrentPageView.ViewModel is ITipButtomMapSupport tipMap)
                {
                    TipButtonVisible = tipMap.TipButtomMap;
                }
                else
                {
                    TipButtonVisible = TIP_BUTTON_DEFAULT;
                }
            }
        }

        public override void ChildPageNavgation(IChildPageSupport sender, IPageViewInterface page)
        {
            if (page != null)
            {
                _navigationPages.Push(CurrentPageView);
                PageContainer.Content = page;
                Title = page.ViewModel?.Title;
                NavigatedLayer += 1;
                TipButtonVisible = (page.ViewModel as ITipButtomMapSupport).TipButtomMap;
                CurrentPageView.ViewModel.IsShown = false;
                page.ViewModel.IsShown = true;
                CurrentPageView = page;
                CurrentPageViewModel = CurrentPageView.ViewModel;
            }
        }

        public void Back(object para)
        {
            if (NavigatedLayer > 0)
            {
                NavigatedLayer -= 1;
                if (NavigatedLayer == 0)
                {
                    NavigationTo(CurrentPageIndex);
                }
                else
                {
                    var page = _navigationPages.Pop();
                    PageContainer.Content = page;
                    Title = page.ViewModel?.Title;
                    if (page.ViewModel is ITipButtomMapSupport childPage)
                    {
                        TipButtonVisible = childPage.TipButtomMap;
                    }
                    CurrentPageView.ViewModel.IsShown = false;
                    page.ViewModel.IsShown = true;
                    CurrentPageView = page;
                    CurrentPageViewModel = CurrentPageView.ViewModel;
                }
            }
            // Never back home when in the first layer,
            // and recover the layer to first when the current layer error.
            else if (NavigatedLayer < 0)
            {
                NavigatedLayer = 0;
                NavigationTo(0);
            }
        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            try
            {
                switch (key)
                {
                    case KeyCodeEnum.B:
                        //if (CurrentPageViewModel is ListItemSupportViewModelBase listItemSupport &&
                        //    listItemSupport.CurrentItem.IsSelected)
                        //{
                        //    listItemSupport.CurrentItem.IsSelected = false;
                        //}
                        //else
                        {
                            //Model.Restore();
                            if (CurrentPageViewModel is LanguagePageViewModel languagePageVM)
                            {
                                languagePageVM.HandleKeyEvent(key, type, out bool isCancel);
                                if (!isCancel) Back(null);
                            }
                            else if (CurrentPageViewModel is KeyDescriptionPageViewModel keyDescriptionPageVM)
                            {
                                keyDescriptionPageVM.HandleKeyEvent(key, type, out bool isCancel);
                                if (!isCancel) Back(null);
                            }
                            else
                            {
                                Back(null);
                            }
                        }
                        break;
                    //case KeyCodeEnum.A:
                    //    if (NavigatedLayer == 0)
                    //    {
                    //        CurrentPageView.ViewModel.HandleKeyEvent(key, type);
                    //    }
                    //    else if (CurrentPageView != null)
                    //    {
                    //        if (type == KeyEventTypeEnmu.DOWN)
                    //        {
                    //            SaveProfile();
                    //            Back(null);
                    //        }
                    //    }
                    //    break;
                    case KeyCodeEnum.Quick:
                        if (type == KeyPressTypeEnmu.ShortPress || type == KeyPressTypeEnmu.AppClick)
                        {
                            if (YzGamingService.Instance.QuickMenu != null &&
                                YzGamingService.Instance.QuickMenu.WindowState == WindowState.Maximized)
                            {
                                YzGamingService.Instance.HideQuickMenu();
                            }
                            else
                            {
                                YzGamingService.Instance.ShowQuickMenu();
                            }
                        }
                        else
                        {
                            if (YzGamingService.Instance.MainWindow.WindowState == WindowState.Maximized)
                            {
                                //bool isNeedReShow = !YzGamingService.Instance.IsMainShown;
                                YzGamingService.Instance.HideMainWindows();
                                //if (isNeedReShow) YzGamingService.Instance.ShowMainWindows();
                            }
                            else
                            {
                                YzGamingService.Instance.ShowMainWindows();
                            }
                        }
                        break;
                    default:
                        if (NavigatedLayer == 0)
                        {
                            base.HandleKeyEvent(key, type);
                        }
                        else if (CurrentPageView != null)
                        {
                            CurrentPageView.ViewModel.HandleKeyEvent(key, type);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        public override void HandleThumbStatusEvent(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            try
            {
                if (CurrentPageView != null)
                {
                    CurrentPageView.ViewModel.HandleThumbStatusEvent(key, direction);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

        }

        public void OnMiniWindows()
        {
            GamePlatform.Instance.MiniWindows();
        }

        public override void OnTipButtonMapChanged()
        {
            OnPropertyChanged(nameof(TipButtonVisible));
        }

        private void OnDpadKeyPress(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            //if ((!GamePlatform.Instance.IsPlatformRunning && !YzGamingService.Instance.IsQuickMenuShown && YzGamingService.Instance.IsMainShown) 
            //    || key == KeyCodeEnum.Quick)
            if ((!YzGamingService.Instance.IsQuickMenuShown && YzGamingService.Instance.IsMainShown) 
                || key == KeyCodeEnum.Quick)
            {
                Application.Current.Dispatcher.BeginInvoke(_handleKeyEvent, new object[] { key, type });
            }
        }

        private void OnThumbStatusReport(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
            //if (!GamePlatform.Instance.IsPlatformRunning &&
            //    !YzGamingService.Instance.IsQuickMenuShown && YzGamingService.Instance.IsMainShown)
            if (!YzGamingService.Instance.IsQuickMenuShown && YzGamingService.Instance.IsMainShown)
            {
                Application.Current.Dispatcher.BeginInvoke(_handleThumbStatusEvent, new object[] { key, direction });
            }
        }
    }
}
