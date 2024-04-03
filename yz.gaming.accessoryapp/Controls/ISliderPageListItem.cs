using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Controls
{
    public interface ISliderPageListItem : IPageListItem
    {
        int Value { get; set; }

        void RollbackValue();
    }
}
