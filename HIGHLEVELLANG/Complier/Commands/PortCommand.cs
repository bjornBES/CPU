using System;
using System.Collections.Generic;
using System.Text;

namespace CPUTing.HIGHLEVELLANG.Complier.commands
{
    public static class PortCommand
    {
        static string[] AsmCode;
        static int ASMIndex;
        public static void CompilCode(string Ops, string Name, string CodeLine, int Line, string FullLine)
        {
            bool Parsed;
            if (Ops.Contains(CompilerCommands.PortExt) == false)
            {
                CompilerCommands.SetNewError("Port func not found line", Line, FullLine, "A723", 3);
                return;
            }
            string PortCommand = Ops.Split(CompilerCommands.PortExt)[1];
            object TempportCommand;
            PortCommands portCommands = PortCommands.none;
            Parsed = Enum.TryParse(typeof(PortCommands), PortCommand, out TempportCommand);
            if (Parsed == true)
            {
                portCommands = (PortCommands)TempportCommand;
            }
            switch (portCommands)
            {
                case PortCommands.Write:
                    if (CompilerCommands.IsChar)
                    {
                        AsmCode[ASMIndex] = "INBY #08";
                        ASMIndex++;
                        AsmCode[ASMIndex] = "LODC \'" + Name.Split(CompilerCommands.Char)[1] + "\'";
                        ASMIndex++;
                        AsmCode[ASMIndex] = "INBY #E0";
                        ASMIndex++;
                        AsmCode[ASMIndex] = "INBY #00";
                        ASMIndex++;
                        AsmCode[ASMIndex] = "INBY #06";
                        ASMIndex++;
                    }
                    if (CompilerCommands.IsString)
                    {
                        AsmCode[ASMIndex] = "PUHS " + CompilerCommands.String + Name.Split(CompilerCommands.String)[1] + CompilerCommands.String;
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
                    CompilerCommands.SetNewError("command not found line", Line, FullLine, "B832", 2);
                    break;
            }
        }
        public static string[] GetCode()
        {
            return AsmCode;
        }
        public static string GetCode(int index)
        {
            return AsmCode[index];
        }
    }
}
