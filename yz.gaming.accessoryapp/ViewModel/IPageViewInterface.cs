using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.ViewModel
{
    public interface IPageViewInterface
    {
        IViewModel ViewModel { get; }
        IPageViewInterface Init(INavigationSupport navigationParent);
    }
}
