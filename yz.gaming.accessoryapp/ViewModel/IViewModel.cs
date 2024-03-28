using System;
using System.Collections.Generic;
using System.Text;
using yz.gaming.accessoryapp.Languange;
using yz.gaming.accessoryapp.Model;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel
{
    public interface IViewModel
    {
        string Title { get; set; }

        YzProfileModel Model { get; }

        bool IsShown { get; set; }

        void SaveProfile();

        void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type);

        void HandleThumbStatusEvent(ThumbKeyEnmu key, ThumbDirectionEnmu direction);
    }
}
