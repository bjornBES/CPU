using CrypticWizard.HexConverter;
using System;
using CPUTing.HIGHLEVELLANG.Complier;

namespace CPUTing.HIGHLEVELLANG.Complier
{
    public class Complier : CompilerCommands
    {
        string[] Code;
        public string[] RawCode;
        public string[] AsmCode = new string[0xFFFF + 1];
        public int ASMIndex = 0; 
        public string Error;
        public void Start(string[] BZCode)
        {
            Code = BZCode;
            RawCode = new string[Code.Length];
            for (int i = 0; i < Code.Length; i++)
            {
                RemoveComms(i);
            }
            for (int i = 0; i < RawCode.Length; i++)
            {
                if (RawCode[i] != "\t" && RawCode[i] != "" && RawCode[i] != " ")
                    GetCode(i);
            }
        }
        public void RemoveComms(int i)
        {
            if(Code[i].Contains(COMM))
            {
                int Index = Code[i].IndexOf(COMM);
                RawCode[i] = Code[i].Remove(Index, Code[i].Length);
            }
            else
            {
                if(Code[i] != "\t" && Code[i] != "" && Code[i] != " ")
                    RawCode[i] = Code[i];
                return;
            }
        }
        string Ops;
        string Name = "";
        int DecNummber;
        bool IsDec;
        bool IsHex;
        bool IsBin;
        bool IsString;
        bool IsChar;
        string Operator = "";
        string VarType = "";
        object Tempcommands;
        bool Parsed;
        Commands commands;
        public void GetCode(int i)
        {
            string[] Line = RawCode[i].Split(' ');
            Ops = Line[0];

            FindCommand(Line, i);

            switch (commands)
            {
                case Commands.var:
                    break;
                case Commands.While:
                    break;
                case Commands.If:
                    break;
                case Commands.func:
                    break;
                case Commands.inport:
                    break;
                case Commands.port:
                    if(Ops.Contains(PortExt) == false)
                    {
                        Error += "Port func not found line " + i + " " + Line[i] + " \n";
                        return;
                    }
                    string PortCommand = Ops.Split(PortExt)[1];
                    object TempportCommand;
                    PortCommands portCommands = PortCommands.none;
                    Parsed = Enum.TryParse(typeof(PortCommands), PortCommand, out TempportCommand);
                    if(Parsed == true)
                    {
                        portCommands = (PortCommands)TempportCommand;
                    }
                    switch (portCommands)
                    {
                        case PortCommands.Write:
                            if(IsChar)
                            {
                                AsmCode[ASMIndex] = "INBY #08";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "LODC \'" + Name.Split(Char)[1] + "\'";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "INBY #E0";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "INBY #00";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "INBY #06";
                                ASMIndex++;
                            }
                            if(IsString)
                            {
                                AsmCode[ASMIndex] = "PUHS " + String + Name.Split(String)[1] + String;
                                ASMIndex++;
                                AsmCode[ASMIndex] = "*PRINT_STRING_FUN:";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "POPR #02";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "INBY #E0";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "DECR #00";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "INBY #05";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "JINZ PRINT_STRING_FUN";
                                ASMIndex++;
                                AsmCode[ASMIndex] = "INBY #05";
                                ASMIndex++;
                            }
                            break;
                        case PortCommands.GetKey:
                            break;
                        case PortCommands.GetLine:
                            break;
                        case PortCommands.Move:
                            break;
                        default:
                            Error += "command not found line " + i + " " + Line[i] + "\n";
                            break;
                    }
                    break;
                default:
                    Error += "command not found line " + i + " " + Line[i] + "\n";
                    break;
            }
        }
        public void FindCommand(string[] Line, int i)
        {
            if (Line.Length == 2)
            {
                Name = Line[1];
            }
            if (Line.Length == 4)
            {
                if (Line[3] == "")
                {
                    IsDec = int.TryParse(Line[3], out DecNummber);
                    IsHex = Line[3].Contains(HexStart);
                    IsBin = Line[3].Contains(BinStart);
                    IsString = Line[3].Contains(String);
                    IsChar = Line[3].Contains(Char);
                    bool Result = IsDec | IsHex | IsBin | IsString | IsChar;
                    if (Result == false)
                    {
                        Error += "type not found line " + i + " " + Line[i] + "\n";
                    }
                }
                Operator = Line[2];
            }
            if (Line.Length > 4)
            {
                VarType = Line[5];
            }
            Parsed = Enum.TryParse(typeof(Commands), Ops, out Tempcommands);

            if (Parsed == false)
                Parsed = Enum.TryParse(typeof(Commands), Ops.ToLower(), out Tempcommands);

            if (Parsed == false)
                Parsed = Enum.TryParse(typeof(Commands), Ops.ToUpper(), out Tempcommands);

            if (Parsed == true)
            {
                commands = (Commands)Tempcommands;
            }
            else
            {
                Error += "Command can not be found line " + i + " " + RawCode[i] + "\n";
                return;
            }

        }
    }
}
