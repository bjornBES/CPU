using System;
using System.Collections.Generic;
using System.Text;

namespace CPUTing.HIGHLEVELLANG.Complier.Commands
{
    public static class GetKEY
    {
        static string code;
        public static void GetKey(string args, int lableIndex)
        {
            string Arg1 = args.Split(' ')[0];
            string Arg2 = args.Split(' ')[1];
            code = "GETKEY" + lableIndex + ":" + "|";
            code += "\tINBY #20" + "|"; 
            code += "\tOUTB #00" + "|";
            ushort addr = Variable.GetVariableAddr(Arg2);
            if (Arg1.Contains("0x"))
                code += "\tCMPC #" + CompilerCommands.HEXTODEC(Arg1).ToString() + "|";
            else
                code += "\tCMPC #" + Arg1 + "|";
            code += "\tJINT " + "GETKEY" + lableIndex + "|";
            code += "\tSTOC &" + addr  + "|";
        }
        public static int GetLen()
        {
            return code.Split('|').Length;
        }
        public static string GetCode(int index)
        {
            return code.Split('|')[index];
        }
    }
}
