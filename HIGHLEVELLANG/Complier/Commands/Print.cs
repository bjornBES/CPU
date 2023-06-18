using System;
using System.Collections.Generic;
using System.Text;
using CrypticWizard.HexConverter;

namespace CPUTing.HIGHLEVELLANG.Complier.Commands
{
    public static class Print
    {
        public static string printCode;
        public static void print(string ARG, int LableIndex)
        {
            printCode = "\tPUHR #00|";
            printCode += "\tPUHR #02|";
            if (ARG[0] == '"')
            {
                ReplacePrint(ARG, LableIndex, false);
            }
            else
            {
                for (int v = 0; v < Variable.VarADDRPOINTER; v++)
                {
                    if (Variable.variables[v].VariableName == ARG)
                    {
                        ReplacePrint(Variable.GetVariableAddr(ARG).ToString(), LableIndex, true);
                    }
                }
            }
        }
        static void ReplacePrint(string ARG, int LableIndex, bool VAR)
        {
            if (VAR == false)
                printCode += "\tPUHS " + ARG + "|";
            else
            {
                ushort hex = ushort.Parse(ARG);
                printCode += "\tLOAC &" +  Convert.ToString(hex, 16) + "|";
                printCode += "\tPUHR #03|";
            }
            byte[] HEX = { (byte)ARG.Length };
            Lables(LableIndex);
            if (ARG.Contains(@"\n"))
            {
                HEX = new byte[] { 2 };
            }
            string SHEX = HexConverter.GetHexString(HEX);
            printCode += "\tLODA #" + SHEX + "|";
            printCode += "\tCMPA #00|";
            printCode += "\tPOPR #02|";
            printCode += "\tINBY #00|";
            printCode += "\tDECR #00|";
            printCode += "\tJINZ PRINT" + LableIndex + "|";
            printCode += "\tINBY #10|";
            printCode += "\tPOPR #00|";
            printCode += "\tINBY #40|";
        }
        static void Lables(int LableIndex)
        {
            printCode += "PRINT" + LableIndex.ToString() + ":|";
        }
        public static int GetLen()
        {
            return printCode.Split('|').Length;
        }
        public static string ASMOUT(int index)
        {
            return printCode.Split('|')[index];
        }
    }
}
