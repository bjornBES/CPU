using CrypticWizard.HexConverter;
using System;

namespace CPUTing
{
    public class Lables
    {
        public string name;
        public string addr;
    }
    public class AssSets
    {
        public const string ASSEMBLERTOADDR = ".ORG &";
        public const string SETADDRINCODE = ".word &";
        public const char LABLES = ':';
        public const char CHAR = '\'';
        public const char STRING = '\"';
        public const char COMM = ';';
        public const char IMMHEX = '#';
        public const char AMMHEX = '&';
        public const char EMETY = ' ';
        public const string INSTR = "*";
        public const string ESCTAG = "ESC";
        public const string NLTAG = "NL";
    }
    public class Assembler : AssSets
    {
        public string[] MCCODE;
        private int MCINDEX = 0;
        private readonly Lables[] Lables = new Lables[0xFF];
        private int LABLEINDEX = 0;
        private ushort PCADDR = 0x0000;
        public string[] ASMCODE;
        public int CODELEN;
        public Assembler()
        {
            Reset();
        }
        public void Reset()
        {
            MCCODE = new string[0xFFFF + 1];
            ASMCODE = new string[0xFFFF + 1];
            for (int i = 0; i < Lables.Length; i++)
            {
                Lables[i] = new Lables
                {
                    name = "",
                    addr = "0"
                };
            }
            MCCODE.Initialize();
            ASMCODE.Initialize();
        }
        public void Start()
        {
            if (ASMCODE.Length == 0xFFFF + 1)
            {
                SPLITCODE();
            }
        }
        public void SPLITCODE()
        {
            //this is for the "i" in here 
            int ASMCODEINDEX = 0;
            // start here

            do
            {
                // setting the keywords to hex
                if (ASMCODE[ASMCODEINDEX].Contains(NLTAG))
                    MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX].Replace(NLTAG, "1B");

                if (ASMCODE[ASMCODEINDEX].Contains(ESCTAG))
                    MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX].Replace(ESCTAG, "0A");
                string fullADDR;
                string ADDRs1;
                string ADDRs2;

                if (ASMCODE[ASMCODEINDEX].Contains(ASSEMBLERTOADDR))
                {
                    //at 6 the addr is 
                    ADDRs1 = ASMCODE[ASMCODEINDEX][6].ToString() + ASMCODE[ASMCODEINDEX][7].ToString(); //high
                    ADDRs2 = ASMCODE[ASMCODEINDEX][8].ToString() + ASMCODE[ASMCODEINDEX][9].ToString(); //low
                    fullADDR = ADDRs1 + ADDRs2;
                    byte[] HEX = HexConverter.GetBytes(fullADDR);
                    PCADDR = ushort.Parse((HEX[0].ToString() + HEX[1].ToString()));
                }

                if (ASMCODE[ASMCODEINDEX].Contains(SETADDRINCODE))
                {
                    MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX][6].ToString() + ASMCODE[ASMCODEINDEX][7].ToString(); //high
                }

                if (ASMCODE[ASMCODEINDEX].Contains(COMM) == true)
                {
                    int ATindex = ASMCODE[ASMCODEINDEX].IndexOf(COMM);
                    MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX].Remove(ATindex);
                }

                if (ASMCODE[ASMCODEINDEX].Contains(LABLES))
                {
                    Lables[LABLEINDEX].name = ASMCODE[ASMCODEINDEX].Trim(LABLES);
                    Lables[LABLEINDEX].addr = PCADDR.ToString().PadLeft(4, '0');
                    // todo lables not working
                    LABLEINDEX++;
                }

                if (ASMCODE[ASMCODEINDEX].Contains("\t"))
                {
                    int ATINDEX = 0;
                    if (ASMCODE[ASMCODEINDEX].Contains(IMMHEX) || ASMCODE[ASMCODEINDEX].Contains(AMMHEX))
                    {
                        if (ASMCODE[ASMCODEINDEX].Contains(IMMHEX))
                        {
                            ATINDEX = ASMCODE[ASMCODEINDEX].IndexOf(IMMHEX);
                        }
                        if (ASMCODE[ASMCODEINDEX].Contains(AMMHEX))
                        {
                            ATINDEX = ASMCODE[ASMCODEINDEX].IndexOf(AMMHEX);
                        }
                        MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX].Remove(ATINDEX, 1);
                        MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX].TrimEnd(EMETY);
                        MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX].Trim(EMETY);
                    }
                    else
                    {
                        if (ASMCODE[ASMCODEINDEX].Contains(' '))
                        {
                            for (int i = 0; i < LABLEINDEX; i++)
                            {
                                if (Lables[i].name == ASMCODE[ASMCODEINDEX].Split(' ', 2)[1])
                                {
                                    byte[] ADDR = { byte.Parse(Lables[i].addr.Remove(1, 3)), byte.Parse(Lables[i].addr.Remove(0, 2)) };
                                    string HEXADDR = HexConverter.GetHexString(ADDR);
                                    MCCODE[PCADDR] = HEXADDR.PadLeft(4, '0');
                                }
                            }
                        }
                    }
                    PCADDR++;
                }

                if (ASMCODE[ASMCODEINDEX] == "")
                {
                }

                ASMCODEINDEX++;
            } while (ASMCODEINDEX < CODELEN);

            // end here
            /*
            for (int i = 0; i < CODE.Length; i++)
            {
                if (CODE[i] == null)
                    CODE[i] = "*HLTC";
                DoLables(i);
            }
            SPLITCHAR();
            SPLITSTRING();
            MCINDEX = 0;
            for (int i = 0; i < CODEINDEX; i++)
            {
                ASS(i, true);
            }
            */
        }
        /*
        public void SPLITCHAR()
        {
            for (int i = 0; i < CODE.Length; i++)
            {
                if (CODE[i].Contains(CHAR))
                {
                    int index = CODE[i].IndexOf(CHAR);
                    char letter = CODE[i][index + 1];
                    CODE[i] = CODE[i].Trim(CHAR);
                    CODE[i] = CODE[i].Replace("'" + letter, letter.ToString());
                    string[] text = CODE[i].Split(' ', 1);
                    text[1] = ((byte)letter).ToString();
                    byte[] HEX = { Convert.ToByte(text[1]) };
                    CODE[i] = text[0] + " " + HexConverter.GetHexString(HEX);
                }
            }
        }
        public void SPLITSTRING()
        {
            for (int i = 0; i < CODE.Length; i++)
            {
                if (CODE[i].Contains(STRING))
                {
                    int index = CODE[i].IndexOf(STRING);
                    int indexEnd = CODE[i].IndexOf(STRING, index + 1);
                    int Len = indexEnd - index - 1;
                    string letterHEX = "";
                    string letter = "";
                    for (int S = 0; S < Len; S++)
                    {
                        byte[] HEX = { ((byte)CODE[i][index + S + 1]) };
                        letterHEX += HexConverter.GetHexString(HEX) + " ";
                        letter += CODE[i][index + S + 1];
                    }
                    letter = letter.TrimEnd(' ');
                    letterHEX = letterHEX.TrimEnd(' ');
                    CODE[i] = CODE[i].Trim(STRING);
                    CODE[i] = CODE[i].Replace("\"" + letter, letterHEX.ToString());
                    string[] text = CODE[i].Split(' ', 2);
                    text[1] = ((byte)letter[0]).ToString();
                    CODE[i] = text[0] + " (" + letterHEX + ")";
                }
            }
        }
        public void ASS(int i, bool EN)
        {
            string ARGS1 = "";
            string ARGS2 = "";
            string OPCODE;
            if (CODE[i].Contains(' '))
            {
                OPCODE = CODE[i].Split(' ')[0];
                if (CODE[i].Contains('('))
                {
                    ARGS1 = CODE[i].Split('(', ')')[1];
                }
                else
                {
                    ARGS1 = CODE[i].Split(' ')[1].PadLeft(2, '0');
                    if (CODE[i].Split(' ').Length == 3)
                    {
                        ARGS2 = CODE[i].Split(' ')[2].PadLeft(4, '0');
                    }
                }
            }
            else
            {
                OPCODE = CODE[i].PadLeft(4, '0');
            }
            TOMCCODE(OPCODE, ARGS1, ARGS2, EN);
        }
        */
        private void SETANDGETVALUE(string opcode, string ARGS, string Values, INSTR iNSTR, bool EN)
        {
            if (Values == opcode)
            {

                if (EN == true)
                {
                    byte[] code = { (byte)iNSTR };
                    if (ARGS == "")
                    {
                        MCCODE[MCINDEX] = HexConverter.GetHexString(code) + "00";
                    }
                    else if (ARGS.Length == 4)
                    {
                        MCCODE[MCINDEX] = HexConverter.GetHexString(code) + "00";
                        MCINDEX++;
                        MCCODE[MCINDEX] = ARGS;
                    }
                    else if (ARGS.Length > 4)
                    {
                        int len = ARGS.Split(' ').Length;
                        MCCODE[MCINDEX] = HexConverter.GetHexString(code) + len.ToString().PadLeft(2, '0') + " (" + ARGS + ")";
                    }
                    else
                    {
                        MCCODE[MCINDEX] = HexConverter.GetHexString(code) + ARGS;
                    }
                }
                else
                {
                    if (ARGS.Length == 4)
                    {
                        MCINDEX++;
                    }
                }
            }
        }

        private void SETANDGETVALUE(string opcode, string ARGS1, string ARGS2, string Values, INSTR iNSTR, bool EN)
        {
            if (Values == opcode)
            {

                if (EN == true)
                {
                    byte[] code = { (byte)iNSTR };

                    if (ARGS2.Length == 4 && ARGS1.Length == 2)
                    {
                        MCCODE[MCINDEX] = HexConverter.GetHexString(code) + ARGS1;
                        MCINDEX++;
                        MCCODE[MCINDEX] = ARGS2;
                    }
                }
                else
                {
                    MCINDEX++;
                }
            }
        }
        public void TOMCCODE(string OPCODE, string ARGS1, string ARGS2, bool EN)
        {
            INSTR[] iNSTR = (INSTR[])Enum.GetValues(typeof(INSTR));
            string[] Values = Enum.GetNames(typeof(INSTR));
            for (int i = 0; i < iNSTR.Length; i++)
            {
                if (OPCODE == Values[i])
                {
                    if (ARGS2 != "")
                    {
                        SETANDGETVALUE(OPCODE, ARGS1, ARGS2, Values[i], iNSTR[i], EN);
                    }
                    else
                    {
                        SETANDGETVALUE(OPCODE, ARGS1, Values[i], iNSTR[i], EN);
                    }
                    MCINDEX++;
                }
            }
        }
    }
}
