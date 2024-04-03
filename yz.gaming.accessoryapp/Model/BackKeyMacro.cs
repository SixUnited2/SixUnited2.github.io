using System;
using System.Collections.Generic;
using System.Text;

namespace yz.gaming.accessoryapp.Model
{
    public class BackKeyMacro
    {
        public BackKeyMacro()
        {
            M1 = new Macro()
            {
                Step = new MacroStep()
            };
            M2 = new Macro()
            {
                Step = new MacroStep()
            };
        }

        public Macro M1 { get; set; }
        public Macro M2 { get; set; }

        public class Macro
        {
            public byte CycleFlag { get; set; }
            public ushort CycleInterval { get; set; }
            public MacroStep Step { get; set; }
        }
    }
}
