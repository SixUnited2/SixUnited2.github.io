using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Controls
{
    public interface IArrayPageListItem : IPageListItem
    {
        void SelectNext();
        void SelectPrev();
    }
}
