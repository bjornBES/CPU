using System;
using CrypticWizard.HexConverter;
using CPUTing.HIGHLEVELLANG.Complier.commands;

namespace CPUTing.HIGHLEVELLANG.Complier
{
    public enum Commands
    {
        none,
        var,
        While,
        If,
        func,
        inport,
        port,
        release,
        exit,
    }
    public enum Keywords
    {
        local,
        global,
    }
    public enum PortCommands
    {
        none,
        Write,
        GetKey,
        GetLine,
        Move
    }
    public static class CompilerCommands
    {
        public static string COMM = ":";
        public static string HexStart = "0x";
        public static string BinStart = "0b";
        public static string PortExt = "::";
        public static char String = '"'; //"
        public static char Char = '\''; //'
        public static string NewLineChar = "\\n"; //\n
        public static bool IsDec;
        public static bool IsHex;
        public static bool IsBin;
        public static bool IsString;
        public static bool IsChar;
        public static string Error { get; private set; }
        public static void SetNewError(string ErrorText, int LineNumber, string CodeLine, string ErrorCode, int ErrorWarningLevel)
        {
            Error += ErrorText + " line number " + LineNumber + " code " + CodeLine + " BZ-" + ErrorCode + "EW" + ErrorWarningLevel + "\n";
        }
    }
}
