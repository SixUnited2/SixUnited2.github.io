using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;

namespace yz.gaming.accessoryapp.ViewModel
{
    public interface IChildPageSupport
    {
        Dictionary<IPageListItem, IPageViewInterface> ChildPageMap { get; set; }
    }
}
