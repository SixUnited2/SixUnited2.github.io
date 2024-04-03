using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.View;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel
{
    public abstract class NavigationSupportViewModel : ViewModelBase, INavigationSupport
    {
        public List<IPageViewInterface> PageList { get; set; }
        public List<INavigationItem> MenuList { get; set; }
        public Dictionary<INavigationItem, IPageViewInterface> NavigationPageMap { get; set; }
        public int CurrentPageIndex { get; set; } = -1;
        public INavigationItem CurrentMenu { get; set; }
        public IPageViewInterface CurrentPageView { get; set; }
        public IViewModel CurrentPageViewModel { get; set; }
        public Frame PageContainer { get; set; }

        public virtual void InitNavigationData()
        {
            NavigationPageMap = new Dictionary<INavigationItem, IPageViewInterface>();
            MenuList.ForEach(p => NavigationPageMap.Add(p, PageList[p.Index]));
        }

        public virtual void NavigationTo(int index)
        {
            if (index != CurrentPageIndex)
            {
                if (MenuList.Count > index)
                {
                    MenuList.ForEach(p =>
                    {
                        if (p.Index != index)
                        {
                            p.IsSelected = false;
                        }
                        else
                        {
                            if (!p.IsSelected) p.IsSelected = true; 
                            CurrentMenu = p;
                        }
                    });

                    if (CurrentPageView != null)
                    {
                        CurrentPageView.ViewModel.IsShown = false;
                    }

                    CurrentPageIndex = index;
                    PageContainer.Content = PageList[index];
                    CurrentPageView = PageList[index];
                }
            }
            else if (!PageContainer.Content.Equals(PageList[index]))
            {
                PageContainer.Content = PageList[index];
                CurrentPageView = PageList[index];
            }

            CurrentPageViewModel = CurrentPageView.ViewModel;
        }

        public void OnSelectedStateChange(MenuTextControl sender, bool isSelected)
        {
            if (isSelected)
            {
                NavigationTo(sender.Index);
            }
        }

        public abstract void ChildPageNavgation(IChildPageSupport sender, IPageViewInterface page);
        public abstract void OnTipButtonMapChanged();

        public virtual void UpdateTitle(string title)
        {
            Title = title;
        }

        public override void Initialization()
        {

        }

        public override void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
            switch (key)
            {
                case KeyCodeEnum.L1:
                    if (CurrentMenu.Index > 0)
                    {
                        NavigationTo(CurrentMenu.Index - 1);
                        CurrentMenu.IsSelected = true;
                    }
                    break;
                case KeyCodeEnum.R1:
                    if (CurrentMenu.Index < MenuList.Count - 1)
                    {
                        NavigationTo(CurrentMenu.Index + 1);
                        CurrentMenu.IsSelected = true;
                    }
                    break;
                default:
                    if (CurrentPageView != null)
                    {
                        CurrentPageView.ViewModel.HandleKeyEvent(key, type);
                    }
                    break;
            }
        }
    }
}
