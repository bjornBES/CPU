using System;
using System.Collections.Generic;
using System.Text;

namespace CPUTing.HIGHLEVELLANG.Complier.Commands
{
    public static class IF
    {
        public static int LoopEND;
        public static string Code;
        public static string ENDCode;
        public static void IfSta(string[] BZCode, int BZindex, string ARGS, int LableIndex)
        {
            int LenIndex = 0;
            for (int i = 0; i < BZCode[BZindex].Length; i++)
            {
                if(BZCode[BZindex][i] == ' ')
                {
                    LenIndex++;
                }
            }
            Code = "";
            operators Operator = operators.none;
            CompilerCommands.FindEnd(BZCode, BZindex, "IF", "ENDIF", out int o);
            Code = "\tPUHR #04" + "|";
            string CMPValue = "";
            if (LenIndex == 3)
            {
                CMPValue = ARGS.Split(' ')[2];
                if (CMPValue.StartsWith("0x"))
                    CMPValue = CMPValue.Remove(0, 2);
                Operator = CompilerCommands.FindOperator(ARGS);
                ushort value = Variable.GetVariableAddr(CompilerCommands.FindVarName(ARGS));
                Code += "\tLOAD &" + value + "|";
            }
            else
            {
                if (ARGS.StartsWith("0x"))
                    CMPValue = ARGS.Remove(0, 2);
                Code += "\tLODD #01|";
                Operator = operators.QT;
            }
            Code += "\tCMPD #" + CMPValue + "|";
            switch (Operator)
            {
                case operators.QT:
                    Code += "\tJNFT ENDIF" + LableIndex;
                    break;
                case operators.NQ:
                    Code += "\tJIFT ENDIF" + LableIndex;
                    break;
                case operators.LT:
                    break;
                case operators.LQ:
                    break;
                default:
                    break;
            }
            ENDCode = "ENDIF " + LableIndex + ":|";
            ENDCode += "\tPOPR #04";
        }
        public static string CODEOUT(int index)
        {
            return Code.Split('|')[index];
        }
        public static int GETLEN()
        {
            return Code.Split('|').Length;
        }
        public static int GetLen(int NowIndex)
        {
            return NowIndex + ENDCode.Split('|').Length;
        }
    }
}
