using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using yz.gaming.accessoryapp.Controls;

namespace yz.gaming.accessoryapp.ViewModel
{
    public interface IListItemSupport
    {
        List<IPageListItem> ListItems { get; set; }

        IPageListItem CurrentItem { get; set; }

        IPageListItem HovedItem { get; set; }

        void MoveToNextItem();
        void MoveToPrevItem();
    }
}
