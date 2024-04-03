using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace yz.gaming.accessoryapp.Languange
{
    public class LanguangeManager
    {
        static LanguangeManager _languangeHelper;
        ResourceDictionary _languangeDictionary;
        public string Languange { get; set; } = "zh-Hans";

        public static LanguangeManager Instance
        {
            get
            {
                if (_languangeHelper == null)
                {
                    _languangeHelper = new LanguangeManager();
                }

                return _languangeHelper;
            } 
        }

        public delegate void LanguangeChangedHandler(string language);
        public event LanguangeChangedHandler OnLanguageChanged;
        public event Action LanguageChangedNotiry;

        public ResourceDictionary LanguangeDictionary
        {
            get => _languangeDictionary;
            set
            {
                _languangeDictionary = value;
                LanguageChangedNotiry?.Invoke();
            } 
        }

        public void SetLanguage(string language)
        {
            OnLanguageChanged?.Invoke(language);
            Languange = language;
        }
        public string GetString(string key)
        {
            return LanguangeDictionary.Contains(key) ? LanguangeDictionary[key].ToString() : string.Empty;
        }

    }
}
