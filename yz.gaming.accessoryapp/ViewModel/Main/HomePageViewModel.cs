using System;
using System.Collections.Generic;
using System.Text;
using yz.gaming.accessoryapp.Controls;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Service;

namespace yz.gaming.accessoryapp.ViewModel.Main
{
    public class HomePageViewModel : ChildPageSupportViewModelBase
    {
        public event Action OnPlatformChanged;

        public PlatformModel Platform { get; set; }

        public HomePageViewModel()
            : base()
        {
        }

        public override void Initialization()
        {
            base.Initialization();
            Title = GetString("GamePlatform");
        }

        public string GetPlatName()
        {
            return GetString(Platform.Name);
        }
        public void OpenButtonlick(IPageListItem sender)
        {
            if (sender is DynamicButtonControlHV item)
            {
                if (int.TryParse(item.Tag.ToString(), out int tag))
                {
                    var platform = GamePlatform.Instance.GetPlatformModel((PlatformEnum)tag);
                    GamePlatform.Instance.Start(platform);
                }
            }
        }
    }
}
