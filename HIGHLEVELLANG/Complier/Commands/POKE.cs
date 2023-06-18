using System;
using System.Collections.Generic;
using System.Text;

namespace CPUTing.HIGHLEVELLANG.Complier.Commands
{
    public static class POKE
    {
        static string Code;
        public static void poke(string ADDR, string ARGS)
        {
            //pokeing into mem
            Code = "STOI #" + ARGS + " &" + ADDR;
        }
    }
}
