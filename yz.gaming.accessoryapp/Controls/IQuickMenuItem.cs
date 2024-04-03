using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Controls
{
    public interface IQuickMenuItem : IPageListItem
    {
        void SetButtonEffect(bool isSelected, bool isHovered);
    }
}
