using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Model
{
    public class PlatformModel
    {
        public PlatformEnum Platform { get; set; }

        public bool IsInstall { get; set; }

        public string Name { get; set; }

        public string FindName { get; set; }

        public string ProcessesByName { get; set; }

        public string Path { get; set; }

        public string Exe { get; set; }

        public string Icon { get; set; }

        public string Parameter { get; set; }

        public string Url { get; set; }
        public string PackageName { get; set; }
    }

    public enum PlatformEnum
    {
        STEAM,
        EPIC,
        EA,
        WEGAME,
        XBOX,
        ROCKSTAR,
        GOG,
        UBI,
        BLIZZARD
    }
}
