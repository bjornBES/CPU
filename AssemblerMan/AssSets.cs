namespace CPUTing.AssemblerMan
{
    public class AssSets
    {
        public const string ASSEMBLERTOADDR = ".ORG &";
        public const string SETADDRINCODE = ".word";
        public const string SETADDRINROM = ".rword";
        public const char ADDR = '&';
        public const char VAR = '!';
        public const char ZPC = '%';
        public const char RA = 'A';
        public const char RB = 'B';
        public const char RC = 'C';
        public const char RD = 'D';
        public const char LABLES = ':';
        public const char CHAR = '\'';
        public const char STRING = '\"';
        public const char COMM = ';';
        public const char IMMHEX = '#';
        public const char EMETY = ' ';
        public const string INSTR = "*";
        public const string BINTAG = "0b";
        public const string ESCTAG = "ESC";
        public const string NLTAG = "NL";
        public const string AUPTAG = "KUP";
        public const string ADOTAG = "KDOWN";
        public const string ALETAG = "KLEFT";
        public const string ARITAG = "KRIGHT";
    }
}
