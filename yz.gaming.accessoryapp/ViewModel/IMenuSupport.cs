using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace yz.gaming.accessoryapp.ViewModel
{
    public interface IMenuSupport
    {
        event Action<Grid> OnSelectedMenuChanged;
        List<Grid> MenuList { get; set; }
        Grid CurrentMenu { get; set; }
        int CurrentIndex { get; set; }


        void MenuSelect(int index);
    }
}
