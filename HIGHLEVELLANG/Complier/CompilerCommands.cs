using System;
using CrypticWizard.HexConverter;
using CPUTing.HIGHLEVELLANG.Complier.Commands;

namespace CPUTing.HIGHLEVELLANG.Complier
{
    public enum CODEs
    {
        none,
        Cursor,
        print,
        getkey,
        POKE,
        DO,
        END,
        IF,
        ENDIF
    }
    public enum Libs
    {
        none = -1,
        std = 0,
    }
    public class CompilerCommands
    {
        public static string[] Soperators = { "==", "!=", "<", "<=" };
        //using a lib
        public bool[] usinglib = new bool[1];
        // the asm code
        public string[] ASMCODE;
        //the index pointer
        public ushort ASMCODEPOINTER = 0;
        public void LOADLIB(Libs libs)
        {
            switch (libs)
            {
                case Libs.std:
                    if(usinglib[(int)libs] == false)
                    {
                        usinglib[(int)libs] = true;
                    }
                    break;
                default:
                    break;
            }
        }
        public const char Var = '@';
        public const char Using = '#';
        public const string Commands = "! ";
        public static void FindEnd(string[] CODE, int BZindex, string Start, string END, out int LoopEND)
        {
            bool SKIPEND = false;
            for (int i = BZindex + 1; i < CODE.Length; i++)
            {
                if (CODE[i] == Start)
                    SKIPEND = true;
                if (CODE[i] == END && SKIPEND == false)
                {
                    LoopEND = i;
                    return;
                }
                else if (CODE[i] == END && SKIPEND == true)
                {
                    SKIPEND = false;
                }
            }
            LoopEND = 0;
        }
        public static string FindVar(string args)
        {
            string Var = args.Split(' ')[0];
            for (int i = 0; i < Variable.variables.Length; i++)
            {
                if (Variable.variables[i].VariableName == Var)
                    return Variable.variables[i].VariableValue;
            }
            return "";
        }
        public static string FindVarName(string args)
        {
            string Var = args.Split(' ')[0];
            for (int i = 0; i < Variable.variables.Length; i++)
            {
                if (Variable.variables[i].VariableName == Var)
                    return Variable.variables[i].VariableName;
            }
            return "";
        }
        public static VariableTemp FindVarT(string args)
        {
            string Var = args.Split(' ')[0];
            for (int i = 0; i < Variable.variables.Length; i++)
            {
                if (Variable.variables[i].VariableName == Var)
                    return Variable.variables[i];
            }
            return new VariableTemp();
        }
        public static operators FindOperator(string args)
        {
            string OP = args.Split(' ')[1];
            for (int i = 0; i < Soperators.Length; i++)
            {
                if (Soperators[i] == OP)
                    return (operators)Enum.Parse(typeof(operators), Enum.GetName(typeof(operators), i));
            }
            return operators.none;
        }
        public static byte HEXTODEC(string HEX)
        {
            return HexConverter.GetBytes(HEX.Remove(0,2))[0];
        }
    }
}
