using CrypticWizard.HexConverter;
using System;
using CPUTing.HIGHLEVELLANG.Complier.Commands;

namespace CPUTing.HIGHLEVELLANG.Complier
{
    public class Complier : CompilerCommands
    {
        private const string TRUE = "1";
        private const string FALSE = "0";
        private int ASMCODELen;
        private string[] BZCodeS;
        private string erorr = "";
        int PrintLableIndex = 0;
        int LOOPLableIndex = 0;
        int IFLableIndex = 0;
        int KEYLableIndex = 0;
        public void Start(string[] BZCode, bool GetLeng)
        {
            PrintLableIndex = 0;
            LOOPLableIndex = 0;
            IFLableIndex = 0;
            KEYLableIndex = 0;
            erorr = "";
            BZCodeS = BZCode;
            if (GetLeng == false)
            {
                ASMCODEPOINTER = 0;
                ASMCODE = new string[ASMCODELen + 1];
                ASMCODE[ASMCODEPOINTER] = ".ORG &0000";
                ASMCODEPOINTER++;
            }
            else
            {
                ASMCODELen = 0;
            }

            for (int i = 0; i < Variable.variables.Length; i++)
            {
                Variable.variables[i] = new VariableTemp
                {
                    VariableName = "",
                    VariableValue = ""
                };
            }
            string OPCode = "";
            string ARGS = "";
            string ADDR = "";
            for (int i = 0; i < BZCodeS.Length; i++)
            {
                BZCodeS[i] = BZCodeS[i].Replace("\t", "");
                BZCodeS[i] = BZCodeS[i].Replace("true", TRUE);
                BZCodeS[i] = BZCodeS[i].Replace("TRUE", TRUE);
                BZCodeS[i] = BZCodeS[i].Replace("false", FALSE);
                BZCodeS[i] = BZCodeS[i].Replace("FALSE", FALSE);
                bool NotCommand = false;
                if (BZCodeS[i].Contains(Commands))
                {
                    int index = BZCodeS[i].IndexOf(Commands);
                    BZCodeS[i] = BZCodeS[i].Remove(index, BZCodeS[i].Length);
                }
                if (BZCodeS[i].Contains(Var) == true)
                {
                    NotCommand = true;
                    if (GetLeng == false)
                    {
                        // setting a new var
                        BZCodeS[i] = BZCodeS[i].Remove(0, 1);
                        OPCode = BZCodeS[i].Split(' ', 2)[0]; //name
                        ARGS = BZCodeS[i].Split(' ', 2)[1]; //value

                        ASMCODE[ASMCODEPOINTER] = Variable.SetNewVariable(ARGS, OPCode);
                        ASMCODEPOINTER++;
                    }
                    else
                    {
                        ASMCODELen++;
                    }
                }
                else if (BZCodeS[i].Contains(Using) == true)
                {
                    NotCommand = true;
                    // loading a lib in
                    string[] LibNames = Enum.GetNames(typeof(Libs));
                    Libs libs = Libs.none;
                    for (int l = 0; l < LibNames.Length; l++)
                    {
                        if (BZCodeS[i].Split(' ')[1] == LibNames[l])
                            libs = (Libs)Enum.Parse(typeof(Libs), LibNames[l]);
                    }
                    LOADLIB(libs);
                }
                else if (BZCodeS[i].Contains("--") || BZCodeS[i].Contains("++"))
                {
                    NotCommand = true;
                    // for inc a var or dec a var
                    if (GetLeng == true)
                    {
                        ASMCODELen++;
                        ASMCODELen++;
                        ASMCODELen++;
                    }
                    else
                    {
                        if (BZCodeS[i].Contains("--"))
                        {
                            Variable.DecVariable(BZCodeS[i].Remove(BZCodeS[i].Length - 2, 2));
                        }
                        if (BZCodeS[i].Contains("++"))
                        {
                            Variable.IncVariable(BZCodeS[i].Remove(BZCodeS[i].Length - 2, 2));
                        }
                        for (int l = 0; l < Variable.GetLen(); l++)
                        {
                            ASMCODE[ASMCODEPOINTER] = Variable.GetCode(l);
                            ASMCODEPOINTER++;
                        }
                    }
                }
                if (BZCodeS[i] != "" && NotCommand == false)
                {
                    if (BZCodeS[i].Contains(' '))
                    {
                        OPCode = BZCodeS[i].Split(' ', 2)[0];
                        ARGS = BZCodeS[i].Split(' ', 2)[1];
                        if (ARGS.Contains(' '))
                        {
                            ADDR = ARGS.Split(' ')[1];
                        }
                    }
                    else
                    {
                        if (BZCodeS[i] != "")
                        {
                            OPCode = BZCodeS[i];
                        }
                    }
                    Enum.TryParse(typeof(CODEs), OPCode, out object cODEs);
                    switch ((CODEs)cODEs)
                    {
                        case CODEs.none:
                            break;
                        case CODEs.print:
                            if (usinglib[0] == true)
                            {
                                if (GetLeng == true)
                                {
                                    ASMCODELen += Print.GetLen();
                                }
                                else
                                {
                                    Print.print(ARGS, PrintLableIndex);
                                    for (int l = 0; l < Print.printCode.Length; l++)
                                    {
                                        ASMCODE[ASMCODEPOINTER] = Print.ASMOUT(l);
                                        ASMCODEPOINTER++;
                                    }
                                    PrintLableIndex++;
                                }
                            }
                            else
                            {
                                erorr = "Missing lib at line " + i + ",";
                                return;
                            }
                            break;
                        case CODEs.POKE:
                            if (GetLeng == true)
                                ASMCODELen++;
                            else
                            {
                                ASMCODE[ASMCODEPOINTER] = "\tSTOI #" + ARGS.Split(' ')[0].Remove(0, 2) + " &" + ADDR.Remove(0, 2);
                                ASMCODEPOINTER++;
                            }
                            break;
                        case CODEs.DO:
                            DOLOOP.DOLoop(BZCodeS, i, ARGS, LOOPLableIndex);
                            if (GetLeng == true)
                            {
                                ASMCODELen = DOLOOP.GetLen(ASMCODELen);
                            }
                            else
                            {
                                for (int l = 0; l < DOLOOP.GETLEN(); l++)
                                {
                                    ASMCODE[ASMCODEPOINTER] = DOLOOP.CODEOUT(l);
                                    ASMCODEPOINTER++;
                                }
                                LOOPLableIndex++;
                            }
                            break;
                        case CODEs.END:
                            if (GetLeng == true)
                            {
                                ASMCODELen += DOLOOP.GETLEN();
                            }
                            else
                            {
                                LOOPLableIndex--;
                                ASMCODE[ASMCODEPOINTER] = DOLOOP.ENDCode.Split('|')[0];
                                ASMCODEPOINTER++;
                                ASMCODE[ASMCODEPOINTER] = DOLOOP.ENDCode.Split('|')[1];
                                ASMCODEPOINTER++;
                                ASMCODE[ASMCODEPOINTER] = DOLOOP.ENDCode.Split('|')[2];
                                ASMCODEPOINTER++;
                            }
                            break;
                        case CODEs.Cursor:
                            if (GetLeng == true)
                            {
                                ASMCODELen++;
                            }
                            else
                            {
                                ASMCODE[ASMCODEPOINTER] = "\tUCHR";
                                ASMCODEPOINTER++;
                            }
                            break;
                        case CODEs.IF:
                            IF.IfSta(BZCodeS, i, ARGS, IFLableIndex);
                            if (GetLeng == true)
                            {
                                ASMCODELen = IF.GetLen(ASMCODELen);
                            }
                            else
                            {
                                for (int l = 0; l < IF.GETLEN(); l++)
                                {
                                    ASMCODE[ASMCODEPOINTER] = IF.CODEOUT(l);
                                    ASMCODEPOINTER++;
                                }
                                LOOPLableIndex++;
                            }
                            break;
                        case CODEs.ENDIF:
                            if (GetLeng == true)
                            {
                                ASMCODELen += IF.GETLEN();
                            }
                            else
                            {
                                ASMCODE[ASMCODEPOINTER] = IF.ENDCode.Split('|')[0];
                                ASMCODEPOINTER++;
                                ASMCODE[ASMCODEPOINTER] = IF.ENDCode.Split('|')[1];
                                ASMCODEPOINTER++;
                            }
                            break;
                        case CODEs.getkey:
                            GetKEY.GetKey(ARGS, KEYLableIndex);
                            if (GetLeng == true)
                            {
                                ASMCODELen += GetKEY.GetLen();
                            }
                            else
                            {
                                for (int k = 0; k < GetKEY.GetLen(); k++)
                                {
                                    ASMCODE[ASMCODEPOINTER] = GetKEY.GetCode(k);
                                    ASMCODEPOINTER++;
                                }
                            }
                            KEYLableIndex++;
                            break;
                        default:
                            break;
                    }
                }
            }
            if (GetLeng == false)
            {
                for (int i = 0; i < ASMCODE.Length; i++)
                {
                    if (ASMCODE[i] == null)
                        ASMCODE[i] = "\tHLTC";
                }
            }
        }
        public void GetCode(out string[] Code)
        {
            Code = ASMCODE;
        }
        public int GetCodeCharLen()
        {
            int OUt = 0;
            for (int i = 0; i < ASMCODELen; i++)
            {
                for (int c = 0; c < ASMCODE[i].Length; c++)
                {
                    OUt++;
                }
            }
            return OUt;
        }
        public void GetErorrs(out string Code)
        {
            Code = erorr;
        }
    }
}
