using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Controls
{
    public interface IPageListItem
    {
        string Text { get; set; }
        bool IsSelected { get; set; }
        bool IsHoved { get; set; }
        int Index { get; set; }
        void SetButtonEffect(bool isSelected, bool isHoved);
        void ConfirmPressed();
    }
}
