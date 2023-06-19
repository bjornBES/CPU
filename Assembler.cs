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
        public string[] ADDRCODES = { "CALL", "STOI", "LOAA", "STOA", "LOAB", "STOB", "LOAC", "STOC", "LOAD", "STOD", "INTB", "JUMP", "JINZ", "JINT", "JIFT" };
        public string[] MCCODE;
        public byte[] MEMRAM;
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
            MEMRAM = new byte[0xFFFF + 1];
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
            MEMRAM.Initialize();
            MCCODE.Initialize();
            ASMCODE.Initialize();
        }
        public void Start()
        {
            if (ASMCODE.Length == 0xFFFF + 1)
            {
                SPLITCODE();
                PreAssembler();
            }
        }
        public void SPLITCODE()
        {
            //this is for the "i" in here 
            int ASMCODEINDEX = 0;
            // start here

            do
            {
                string fullADDR;
                string ADDRs1;
                string ADDRs2;

                if (ASMCODE[ASMCODEINDEX].Contains(ASSEMBLERTOADDR))
                {
                    //at 6 the addr is 
                    ADDRs1 = ASMCODE[ASMCODEINDEX][6].ToString() + ASMCODE[ASMCODEINDEX][7].ToString(); //high
                    ADDRs2 = ASMCODE[ASMCODEINDEX][8].ToString() + ASMCODE[ASMCODEINDEX][9].ToString(); //low
                    fullADDR = ADDRs1 + ADDRs2;
                    ushort HEX = Convert.ToUInt16(fullADDR, 16);
                    PCADDR = HEX;
                }

                if (ASMCODE[ASMCODEINDEX].Contains(SETADDRINCODE))
                {
                    ADDRs1 = ASMCODE[ASMCODEINDEX][7].ToString() + ASMCODE[ASMCODEINDEX][8].ToString(); // high
                    ADDRs2 = ASMCODE[ASMCODEINDEX][9].ToString() + ASMCODE[ASMCODEINDEX][10].ToString(); // low
                    fullADDR = ADDRs1 + ADDRs2;
                    ushort HEX = Convert.ToUInt16(fullADDR, 16);
                    if (HEX.ToString().Length == 4)
                    {
                        MEMRAM[PCADDR] = byte.Parse(HEX.ToString().Remove(0, 2)); //LOW
                        PCADDR++;
                        MEMRAM[PCADDR] = byte.Parse(HEX.ToString().Remove(2, 2)); //high
                    }
                    if (HEX.ToString().Length == 3)
                    {
                        MEMRAM[PCADDR] = byte.Parse(HEX.ToString().PadLeft(2, '0').Remove(0, 2)); //LOW
                        PCADDR++;
                        MEMRAM[PCADDR] = byte.Parse(HEX.ToString().Remove(2, 2)); //high
                    }
                    if (HEX.ToString().Length == 1 || HEX.ToString().Length == 2)
                    {
                        MEMRAM[PCADDR] = 00; //LOW
                        PCADDR++;
                        MEMRAM[PCADDR] = byte.Parse(HEX.ToString().PadLeft(2, '0')); //high
                    }

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

                if (ASMCODE[ASMCODEINDEX].Contains("\t") && ASMCODE[ASMCODEINDEX].Length > 2)
                {
                    int ATINDEX = 0;
                    if (ASMCODE[ASMCODEINDEX].Contains(IMMHEX) || ASMCODE[ASMCODEINDEX].Contains(AMMHEX))
                    {
                        if (ASMCODE[ASMCODEINDEX].Contains(IMMHEX))
                        {
                            ATINDEX = ASMCODE[ASMCODEINDEX].IndexOf(IMMHEX);
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Remove(ATINDEX, 1);
                        }
                        if (ASMCODE[ASMCODEINDEX].Contains(AMMHEX))
                        {
                            ATINDEX = ASMCODE[ASMCODEINDEX].IndexOf(AMMHEX);
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Remove(ATINDEX, 1);
                        }
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
                                    ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Split(' ', 2)[0] + " " + HEXADDR.PadLeft(4, '0');
                                }
                            }
                        }
                    }

                    // setting the keywords to hex
                    if (ASMCODE[ASMCODEINDEX].Contains(NLTAG))
                        ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Replace(NLTAG, "1B");

                    if (ASMCODE[ASMCODEINDEX].Contains(ESCTAG))
                        ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Replace(ESCTAG, "0A");

                    MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX];
                    for (int c = 0; c < ADDRCODES.Length; c++)
                    {
                        if(ASMCODE[ASMCODEINDEX].Split('\t', ' ')[1] == ADDRCODES[c])
                        {
                            PCADDR++;
                        }
                    }
                    PCADDR++;
                }

                if (ASMCODE[ASMCODEINDEX] == "")
                {
                }

                ASMCODEINDEX++;
            } while (ASMCODEINDEX < CODELEN);
            return;
            // end here
        }
        public void PreAssembler()
        {
            SPLITCHAR();
            SPLITSTRING();
            for (int i = 0; i < MCCODE.Length; i++)
            {
                if (MCCODE[i] != null && MCCODE[i] != "")
                {
                    if (TriggerTO == false)
                    {
                        ASS(i);
                    }
                    else
                        TriggerTO = false;
                }
                else
                {
                    MCINDEX++;
                }
            }
        }
        public void SPLITCHAR()
        {
            for (int i = 0; i < MCCODE.Length; i++)
            {
                if (MCCODE[i] != null && MCCODE[i].Contains(CHAR))
                {
                    int index = MCCODE[i].IndexOf(CHAR);
                    char letter = MCCODE[i][index + 1];
                    MCCODE[i] = MCCODE[i].Trim(CHAR);
                    MCCODE[i] = MCCODE[i].Replace("'" + letter, letter.ToString());
                    string[] text = MCCODE[i].Split(' ', 1);
                    text[1] = ((byte)letter).ToString();
                    byte[] HEX = { Convert.ToByte(text[1]) };
                    MCCODE[i] = text[0] + " " + HexConverter.GetHexString(HEX);
                }
            }
        }
        public void SPLITSTRING()
        {
            for (int i = 0; i < MCCODE.Length; i++)
            {
                if (MCCODE[i] != null && MCCODE[i].Contains(STRING))
                {
                    int index = MCCODE[i].IndexOf(STRING);
                    int indexEnd = MCCODE[i].IndexOf(STRING, index + 1);
                    int Len = indexEnd - index - 1;
                    string letterHEX = "";
                    string letter = "";
                    for (int S = 0; S < Len; S++)
                    {
                        byte[] HEX = { ((byte)MCCODE[i][index + S + 1]) };
                        letterHEX += HexConverter.GetHexString(HEX) + " ";
                        letter += MCCODE[i][index + S + 1];
                    }
                    letter = letter.TrimEnd(' ');
                    letterHEX = letterHEX.TrimEnd(' ');
                    MCCODE[i] = MCCODE[i].Trim(STRING);
                    MCCODE[i] = MCCODE[i].Replace("\"" + letter, letterHEX.ToString());
                    string[] text = MCCODE[i].Split(' ', 2);
                    text[1] = ((byte)letter[0]).ToString();
                    MCCODE[i] = text[0] + " (" + letterHEX + ")";
                }
            }
        }
        bool TriggerTO = false;
        public void ASS(int i)
        {
            string ARGS1 = "";
            string ARGS2 = "";
            string OPCODE;
            if (MCCODE[i].Split('\t')[1].Contains(' '))
            {
                OPCODE = MCCODE[i].Split('\t')[1].Split(' ')[0];
                if (MCCODE[i].Split('\t')[1].Contains('('))
                {
                    ARGS1 = MCCODE[i].Split('\t')[1].Split('(', ')')[1];
                }
                else
                {
                    ARGS1 = MCCODE[i].Split('\t')[1].Split(' ')[1].PadLeft(2, '0');
                    if (MCCODE[i].Split('\t')[1].Split(' ').Length == 3)
                    {
                        ARGS2 = MCCODE[i].Split('\t')[1].Split(' ')[2].PadLeft(4, '0');
                    }
                }
            }
            else
            {
                OPCODE = MCCODE[i].Split('\t')[1].PadLeft(4, '0');
            }
                TOMCCODE(OPCODE, ARGS1.ToUpper(), ARGS2.ToUpper());
        }
        public void TOMCCODE(string OPCODE, string ARGS1, string ARGS2 = "")
        {
            INSTR[] iNSTR = (INSTR[])Enum.GetValues(typeof(INSTR));
            string[] Values = Enum.GetNames(typeof(INSTR));
            for (int i = 0; i < iNSTR.Length; i++)
            {
                if (OPCODE == Values[i])
                {
                    string code = HexConverter.GetHexString(new byte[] { (byte)iNSTR[i] });

                    if (ARGS2.Length == 4 && ARGS1.Length == 2) // ADDR AND IMM & ARGS1 | ARGS2
                    {
                        MCCODE[MCINDEX] = code + ARGS1;
                        MCINDEX++;
                        MCCODE[MCINDEX] = ARGS2;
                        TriggerTO = true;
                    }
                    if (ARGS1 == "") // NULL
                    {
                        MCCODE[MCINDEX] = code + "00";
                    }
                    else if (ARGS1.Length == 4) // ADDR & ARGS2
                    {
                        MCCODE[MCINDEX] = code + "00";
                        MCINDEX++;
                        MCCODE[MCINDEX] = ARGS1;
                        TriggerTO = true;
                    }
                    else if (ARGS1.Length > 4) // STRING & ARGS1
                    {
                        int len = ARGS1.Split(' ').Length;
                        MCCODE[MCINDEX] = code + len.ToString().PadLeft(2, '0') + " (" + ARGS1 + ")";
                    }
                    else // IMM & ARGS1
                    {
                        MCCODE[MCINDEX] = code + ARGS1;
                    }

                    MCINDEX++;
                }
            }
        }
    }
}
