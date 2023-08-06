using CrypticWizard.HexConverter;
using System;
using System.IO;
using System.Collections.Generic;
using CPUTing.HIGHLEVELLANG.Complier.commands;

namespace CPUTing.HIGHLEVELLANG.Complier
{
    public class Complier
    {
        int FileIndex = 0;
        List<string> Files; //Name1.255-FileIndex2
        string[] FuncAddrs = new string[255]; //AHex4-FileIndex2
        string[] Code;
        public string[] RawCode;
        public string[] AsmCode = new string[0xFFFF + 1];
        public int ASMIndex = 0; 
        public void Start(string[] BZCode)
        {
            FuncAddrs.Initialize();
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
            if(Code[i].Contains(CompilerCommands.COMM))
            {
                int Index = Code[i].IndexOf(CompilerCommands.COMM);
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
        string Operator = "";
        string VarType = "";
        object Tempcommands;
        Commands commands;
        public void GetCode(int i)
        {
            string[] Line = RawCode[i].Split(' ');

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
                    if (FileIndex == 255)
                    {
                        CompilerCommands.SetNewError("FileIndex has overflowed", i, RawCode[i], "ABE2", 4);
                        break;
                    }
                    Files.Add(Name + "-" + FileIndex);
                    FileIndex++;
                    break;
                case Commands.port:
                    PortCommand.CompilCode(Ops, Name, Line[i], i, RawCode[i]);
                    break;
                case Commands.release:
                    break;
                case Commands.exit:

                    break;
                default:
                    CompilerCommands.SetNewError("command not found line", i, RawCode[i], "815B", 3);
                    break;
            }
        }
        public void FindCommand(string[] LineCode, int LineNumber)
        {
            object Result;
            bool HasKayword = Enum.TryParse(typeof(Keywords), LineCode[0], out Result);
            string Code = "";

            for (int i = 0; i < LineCode.Length; i++)
            {
                Code += LineCode[i];
            }
            if(HasKayword == true)
            {
                Ops = LineCode[1];
                Name = LineCode[2];
                //port, if, while, inport
            }
            else
            {
                Ops = LineCode[0];
            }
            if(Name.Length > 255)
            {
                CompilerCommands.SetNewError("The Name is to long at line", LineNumber, Code, "173A", 1);
                return;
            }


            /*
            bool Parsed;
            //checking for variables
            CompilerCommands.IsDec = int.TryParse(Line[3], out DecNummber);
            CompilerCommands.IsHex = Line[3].Contains(CompilerCommands.HexStart);
            CompilerCommands.IsBin = Line[3].Contains(CompilerCommands.BinStart);
            CompilerCommands.IsString = Line[3].Contains(CompilerCommands.String);
            CompilerCommands.IsChar = Line[3].Contains(CompilerCommands.Char);
            bool Result =
                CompilerCommands.IsDec |
                CompilerCommands.IsHex |
                CompilerCommands.IsBin |
                CompilerCommands.IsString |
                CompilerCommands.IsChar;
            if (Result == false)
            {
                Error += "type not found line " + i + " " + Line[i] + "\n";
            }
            if (Line.Length == 2)
            {
                Name = Line[1];
            }
            if (Line.Length == 4)
            {
                if (Line[3] == "")
                {
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
            */
        }
    }
}
