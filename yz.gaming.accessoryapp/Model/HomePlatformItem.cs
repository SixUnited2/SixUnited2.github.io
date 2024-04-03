using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace yz.gaming.accessoryapp.Model
{
    public class HomePlatformItem
    {
        public int Index { get; set; }
        public int Platform { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public Visibility Visibility { get; set; }
    }
}
