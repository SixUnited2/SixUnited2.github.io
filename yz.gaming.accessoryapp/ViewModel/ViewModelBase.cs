using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using yz.gaming.accessoryapp.Languange;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Service;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.ViewModel
{
    public abstract class ViewModelBase : ObservableObject, IViewModel
    {
        static YzProfileModel _model;
        protected Logger _logger = LogManager.GetCurrentClassLogger();

        string _title;
        public string Title
        { 
            get => _title;
            set
            {
                SetProperty(ref _title, value);
            }
        }

        public YzProfileModel Model => _model;

        public bool IsShown { get; set; }

        public ViewModelBase()
        {
            LanguangeManager.Instance.LanguageChangedNotiry += OnLanguageChangedNotifyHandler;
            
            _model = Ioc.Default.GetRequiredService<YzProfileModel>();
        }

        public virtual void SaveProfile()
        {
            if (!YzGamingService.Instance.WriteProfile(Model.ToStruct()))
            {
                Model.Restore();
            }

            Model.FreeTmpPtr();
        }

        public abstract void Initialization();
        public virtual void HandleKeyEvent(KeyCodeEnum key, KeyPressTypeEnmu type)
        {
        }

        public virtual void HandleThumbStatusEvent(ThumbKeyEnmu key, ThumbDirectionEnmu direction)
        {
        }

        protected virtual void OnLanguageChangedNotifyHandler()
        {
            Initialization();
        }

        public string GetString(string key) => LanguangeManager.Instance.GetString(key);

    }
}
