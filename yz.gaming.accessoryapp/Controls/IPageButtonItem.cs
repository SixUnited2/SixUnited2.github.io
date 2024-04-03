using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Controls
{
    public interface IPageButtonItem
    {
        bool IsSelected { get; set; }
        bool IsHoved { get; set; }
    }
}
