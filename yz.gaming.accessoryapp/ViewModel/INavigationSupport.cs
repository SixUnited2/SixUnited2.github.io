using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.View;

namespace yz.gaming.accessoryapp.ViewModel
{
    public interface INavigationSupport
    {
        List<IPageViewInterface> PageList { get; set; }
        List<INavigationItem> MenuList { get; set; }
        Dictionary<INavigationItem, IPageViewInterface> NavigationPageMap { get; set; }
        int CurrentPageIndex { get; set; }
        IPageViewInterface CurrentPageView { get; set; }
        IViewModel CurrentPageViewModel { get; set; }
        Frame PageContainer { get; set; }

        void NavigationTo(int index);
        void InitNavigationData();
        void ChildPageNavgation(IChildPageSupport sender, IPageViewInterface page);
        void UpdateTitle(string title);
        void OnTipButtonMapChanged();
    }
}
