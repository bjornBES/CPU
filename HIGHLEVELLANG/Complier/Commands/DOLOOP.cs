using System;
using System.Collections.Generic;
using System.Text;
public enum operators
{
    none = -1,
    QT = 0,
    NQ = 1,
    LT = 2,
    LQ = 3
}
namespace CPUTing.HIGHLEVELLANG.Complier.Commands
{
    public static class DOLOOP
    {
        public static int LoopEND;
        public static string Code;
        public static string ENDCode;
        public static void DOLoop(string[] BZCode, int BZindex, string ARGS, int LableIndex)
        {
            operators Operator = operators.none;
            Code = "";
            CompilerCommands.FindEnd(BZCode, BZindex, "DO", "END", out LoopEND);
            Code = "\tPUHR #04" + "|";
            string CMPValue = "";
            if (ARGS.Contains(' '))
            {
                Operator = CompilerCommands.FindOperator(ARGS);
                CMPValue = ARGS.Split(' ')[2];
                if (CMPValue.StartsWith("0x"))
                    CMPValue = CMPValue.Remove(0, 2);
                else
                    CMPValue = ARGS;
                ushort value = Variable.GetVariableAddr(CompilerCommands.FindVarName(ARGS));
                Code += "\tLOAD &" + value + "|";
            }
            else
            {
                if (ARGS.StartsWith("0x"))
                    CMPValue = ARGS.Remove(0, 2);
                else
                    CMPValue = ARGS;
                Code += "\tLODD #" + (int.Parse(ARGS) + 1) + "|";
                Operator = operators.QT;
            }
            Code += "LOOP" + LableIndex + ":";
            switch (Operator)
            {
                case operators.QT:
                    ENDCode = "\tCMPD #" + CMPValue + "|";
                    break;
                case operators.NQ:
                    ENDCode = "\tCMPD #" + CMPValue + "|";
                    break;
                case operators.LT:
                    break;
                case operators.LQ:
                    break;
                default:
                    break;
            }
            ENDCode += "\tJINT LOOP" + LableIndex + "|";
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
