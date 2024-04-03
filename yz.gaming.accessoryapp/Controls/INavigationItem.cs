using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Controls
{
    public interface INavigationItem
    {
        string Text { get; set; }
        bool IsSelected { get; set; }
        int Index { get; set; }
    }
}
