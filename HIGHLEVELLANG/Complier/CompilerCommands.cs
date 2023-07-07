using System;
using CrypticWizard.HexConverter;
using CPUTing.HIGHLEVELLANG.Complier;

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
    }
    public enum PortCommands
    {
        none,
        Write,
        GetKey,
        GetLine,
        Move
    }
    public class CompilerCommands
    {
        public const string COMM = ":";
        public const string HexStart = "0x";
        public const string BinStart = "0b";
        public const string PortExt = "::";
        public const char String = '"'; //"
        public const char Char = '\''; //'
        public const string NewLineChar = "\\n"; //\n
    }
}
