using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Controls
{
    public interface ISelectableItem
    {
        string Text { get; set; }
        bool IsSelected { get; set; }
        bool IsHover { get; set; }
        int Index { get; set; }
    }
}
