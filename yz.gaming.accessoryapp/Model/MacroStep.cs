using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Model
{
    public class MacroStep
    {
        public byte Key { get; set; }
        public byte Type { get; set; }
        public ushort Time { get; set; }
        public ushort Interval { get; set; }

        public MacroStep Next { get; set; }
    }
}
