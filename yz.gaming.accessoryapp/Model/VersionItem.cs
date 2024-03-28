using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Model
{
    public class VersionItem
    {
        public string Version { set; get; }
        public string Logs { get; set; }
        public List<languageItem> languages { get; set; }
        public string Sha256 { get; set; }
        public string Url { get; set; }

        public class languageItem
        {
            public string Language { get; set; }
            public string Log { get; set; }
        }
    }
}
