using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel
{
    public class ChildPageSupportViewModelBase : ListItemSupportViewModelBase, IChildPageSupport
    {
        public delegate void NavigationToChildPageHandle(IChildPageSupport sender, IPageViewInterface page);

        public Dictionary<IPageListItem, IPageViewInterface> ChildPageMap { get; set; }

        public event NavigationToChildPageHandle OnChildPageNavigation;

        public override void OnButtonClick(IPageListItem sender)
        {
            if (ChildPageMap.ContainsKey(sender) && ChildPageMap[sender] != null)
            {
                OnChildPageNavigation?.Invoke(this, ChildPageMap[sender]);
            }
        }

        public override void Initialization()
        {
            ListItems = new List<IPageListItem>();

            foreach (var item in ChildPageMap)
            {
                ListItems.Add(item.Key);
            }

            base.Initialization();
        }
    }
}
