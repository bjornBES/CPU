﻿using CPUTing.CPUItems;
using CrypticWizard.HexConverter;
using System;
namespace CPUTing.AssemblerMan
{
    public class Assembler : AssSets
    {
        public string[] ADDRCODES = { "CALL", "STOI", "LOAA", "STOA", "LOAB", "STOB", "LOAC", "STOC", "LOAD", "STOD", "INTB", "JUMP", "JINZ", "JINT", "JIFT", "JIFP", "JINP", "GROM" };
        public string[] MCCODE;
        public byte[] MEMRAM;
        private int MCINDEX = 0;
        private readonly Labels[] Labels = new Labels[0xFF];
        private readonly Vars[] vars = new Vars[0xFF];
        private int LABLEINDEX = 0;
        private int VARINDEX = 0;
        private ushort PCADDR = 0x0000;
        public string[] ASMCODE;
        public int CODELEN;
        public string error;
        public Assembler()
        {
            Reset();
        }
        public void Reset()
        {
            error = "";
            MEMRAM = new byte[0xFFFF + 1];
            MCCODE = new string[0xFFFF + 1];
            ASMCODE = new string[0xFFFF + 1];
            if (AssemblerOps.UseLabels || AssemblerOps.UseVars)
            {
                for (int i = 0; i < Labels.Length; i++)
                {
                    if (AssemblerOps.UseLabels)
                    {
                        Labels[i] = new Labels
                        {
                            name = "",
                            addr = "0"
                        };
                    }
                    if (AssemblerOps.UseVars)
                    {
                        vars[i] = new Vars
                        {
                            name = "",
                            Value = "0"
                        };
                    }
                }
            }
            MCINDEX = 0;
            LABLEINDEX = 0;
            PCADDR = 0;
            CODELEN = 0;

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
                ushort HEX;

                if (ASMCODE[ASMCODEINDEX].Contains(BINTAG))
                {
                    fullADDR = ASMCODE[ASMCODEINDEX].Split(BINTAG)[1];
                    ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Split(BINTAG)[0] + "#" + Convert.ToUInt16(fullADDR, 2).ToString();
                }
                if (AssemblerOps.UseVars && ASMCODE[ASMCODEINDEX].Contains(VAR))
                {
                    string[] SplitStr = ASMCODE[ASMCODEINDEX].Split(' ');
                    SplitStr[0] = SplitStr[0].TrimStart(VAR);
                    string Value;
                    string MEMADRRHEX;
                    string LowAddr = Convert.ToString(VARINDEX, 16).PadLeft(2, '0');
                    if (SplitStr[1].Contains(IMMHEX))
                    {
                        ADDRs1 = SplitStr[1][1].ToString() + SplitStr[1][2].ToString(); //high
                        Value = ADDRs1;
                        byte HexValue = Convert.ToByte(Value, 16);
                        vars[VARINDEX].name = SplitStr[0];
                        vars[VARINDEX].Value = HexValue.ToString();

                        MEMADRRHEX = "10" + LowAddr;
                        MEMRAM[Convert.ToUInt16(MEMADRRHEX, 16)] = HexValue;
                    }
                    if (SplitStr[1].Contains(ADDR))
                    {
                        ADDRs1 = SplitStr[1][1].ToString() + SplitStr[1][2].ToString(); //high
                        ADDRs2 = SplitStr[1][3].ToString() + SplitStr[1][4].ToString(); //low
                        Value = ADDRs1 + ADDRs2;
                        HEX = Convert.ToUInt16(Value, 16);
                        vars[VARINDEX].name = SplitStr[0];
                        vars[VARINDEX].Value = HEX.ToString();

                        MEMADRRHEX = "10" + LowAddr;
                        MEMRAM[Convert.ToUInt16(MEMADRRHEX, 16)] = Convert.ToByte(ADDRs1, 16);
                        VARINDEX++;
                        LowAddr = Convert.ToString(VARINDEX, 16).PadLeft(2, '0');
                        MEMADRRHEX = "10" + LowAddr;
                        MEMRAM[Convert.ToUInt16(MEMADRRHEX, 16)] = Convert.ToByte(ADDRs2, 16);
                    }
                    VARINDEX++;
                }
                if (AssemblerOps.UseDots)
                {
                    if (ASMCODE[ASMCODEINDEX].Contains(ASSEMBLERTOADDR))
                    {
                        //at 6 the addr is 
                        ADDRs1 = ASMCODE[ASMCODEINDEX][6].ToString() + ASMCODE[ASMCODEINDEX][7].ToString(); //high
                        ADDRs2 = ASMCODE[ASMCODEINDEX][8].ToString() + ASMCODE[ASMCODEINDEX][9].ToString(); //low
                        fullADDR = ADDRs1 + ADDRs2;
                        HEX = Convert.ToUInt16(fullADDR, 16);
                        PCADDR = HEX;
                    }
                    if (ASMCODE[ASMCODEINDEX].Contains(SETADDRINCODE))
                    {
                        if (ASMCODE[ASMCODEINDEX].Contains(ADDR))
                        {
                            ADDRs1 = ASMCODE[ASMCODEINDEX][7].ToString() + ASMCODE[ASMCODEINDEX][8].ToString(); // high
                            ADDRs2 = ASMCODE[ASMCODEINDEX][9].ToString() + ASMCODE[ASMCODEINDEX][10].ToString(); // low
                            fullADDR = ADDRs1 + ADDRs2;
                            if (fullADDR.Length == 4)
                            {
                                MEMRAM[PCADDR] = byte.Parse(fullADDR.Remove(0, 2)); //LOW
                                PCADDR++;
                                MEMRAM[PCADDR] = byte.Parse(fullADDR.Remove(2, 2)); //high
                            }
                            if (fullADDR.Length == 3)
                            {
                                MEMRAM[PCADDR] = byte.Parse(fullADDR.PadLeft(2, '0').Remove(0, 2)); //LOW
                                PCADDR++;
                                MEMRAM[PCADDR] = byte.Parse(fullADDR.Remove(2, 2)); //high
                            }
                            if (fullADDR.Length == 1 || fullADDR.Length == 2)
                            {
                                MEMRAM[PCADDR] = 00; //LOW
                                PCADDR++;
                                MEMRAM[PCADDR] = byte.Parse(fullADDR.PadLeft(2, '0')); //high
                            }
                        }
                        else if (AssemblerOps.UseLabels)
                        {
                            for (int l = 0; l < LABLEINDEX; l++)
                            {
                                if (Labels[l].name == ASMCODE[ASMCODEINDEX].Split(' ')[1])
                                {
                                    HEX = Convert.ToUInt16(Labels[l].addr, 10);
                                    MEMRAM[PCADDR] = 00; //LOW
                                    PCADDR++;
                                    MEMRAM[PCADDR] = byte.Parse(HEX.ToString().PadLeft(2, '0')); //high
                                }
                            }
                        }
                        PCADDR++;
                    }
                    if (ASMCODE[ASMCODEINDEX].Contains(SETADDRINROM))
                    {
                        if (ASMCODE[ASMCODEINDEX].Contains(ADDR))
                        {
                            ADDRs1 = ASMCODE[ASMCODEINDEX][8].ToString() + ASMCODE[ASMCODEINDEX][9].ToString(); // high
                            ADDRs2 = ASMCODE[ASMCODEINDEX][10].ToString() + ASMCODE[ASMCODEINDEX][11].ToString(); // low
                            fullADDR = ADDRs1 + ADDRs2;
                            MCCODE[PCADDR] = fullADDR;
                            PCADDR++;
                        }
                        else if (AssemblerOps.UseLabels)
                        {
                            for (int l = 0; l < LABLEINDEX; l++)
                            {
                                if (Labels[l].name == ASMCODE[ASMCODEINDEX].Split(' ')[1])
                                {
                                    HEX = Convert.ToUInt16(Labels[l].addr, 10);
                                    MEMRAM[PCADDR] = 00; //LOW
                                    PCADDR++;
                                    MEMRAM[PCADDR] = byte.Parse(HEX.ToString().PadLeft(2, '0')); //high
                                    PCADDR++;
                                }
                            }
                        }
                    }
                }
                if (ASMCODE[ASMCODEINDEX].Contains(COMM) == true)
                {
                    int ATindex = ASMCODE[ASMCODEINDEX].IndexOf(COMM);
                    MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX].Remove(ATindex);
                }

                if (AssemblerOps.UseLabels && ASMCODE[ASMCODEINDEX].Contains(LABELS))
                {
                    Labels[LABLEINDEX].name = ASMCODE[ASMCODEINDEX].Trim(LABELS);
                    Labels[LABLEINDEX].addr = PCADDR.ToString().PadLeft(4, '0');
                    LABLEINDEX++;
                }

                if (ASMCODE[ASMCODEINDEX].Contains("\t") && ASMCODE[ASMCODEINDEX].Length > 2)
                {
                    int ATINDEX;
                    if (ASMCODE[ASMCODEINDEX].Contains(IMMHEX) || ASMCODE[ASMCODEINDEX].Contains(ADDR))
                    {
                        if (ASMCODE[ASMCODEINDEX].Contains(IMMHEX))
                        {
                            ATINDEX = ASMCODE[ASMCODEINDEX].IndexOf(IMMHEX);
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Remove(ATINDEX, 1);
                        }
                        if (ASMCODE[ASMCODEINDEX].Contains(ADDR))
                        {
                            ATINDEX = ASMCODE[ASMCODEINDEX].IndexOf(ADDR);
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Remove(ATINDEX, 1);
                        }
                    }

                    if (AssemblerOps.UseKeyTags)
                    {
                        // setting the keywords to hex
                        if (ASMCODE[ASMCODEINDEX].Contains(ESCTAG))
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Replace(ESCTAG, "1B");

                        if (ASMCODE[ASMCODEINDEX].Contains(NLTAG))
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Replace(NLTAG, "0A");

                        if (ASMCODE[ASMCODEINDEX].Contains(ADOTAG))
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Replace(ADOTAG, "28");

                        if (ASMCODE[ASMCODEINDEX].Contains(AUPTAG))
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Replace(AUPTAG, "26");

                        if (ASMCODE[ASMCODEINDEX].Contains(ALETAG))
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Replace(ALETAG, "25");

                        if (ASMCODE[ASMCODEINDEX].Contains(ARITAG))
                            ASMCODE[ASMCODEINDEX] = ASMCODE[ASMCODEINDEX].Replace(ARITAG, "27");
                    }

                    MCCODE[PCADDR] = ASMCODE[ASMCODEINDEX];
                    for (int c = 0; c < ADDRCODES.Length; c++)
                    {
                        if (ASMCODE[ASMCODEINDEX].Split('\t', ' ')[1] == ADDRCODES[c])
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
            for (int i = 0; i < MCCODE.Length; i++)
            {
                if (MCCODE[i] != null && MCCODE[i].Contains(' '))
                {
                    if (AssemblerOps.UseLabels)
                    {
                        for (int l = 0; l < LABLEINDEX; l++)
                        {
                            if (Labels[l].name == MCCODE[i].Split(' ', 2)[1])
                            {
                                string HEXADDR = Convert.ToString(int.Parse(Labels[l].addr), 16);
                                MCCODE[i] = MCCODE[i].Split(' ', 2)[0] + " " + HEXADDR.PadLeft(4, '0');
                            }
                        }
                    }
                }
                if (AssemblerOps.UseVars)
                {
                    for (int l = 0; l < VARINDEX; l++)
                    {
                        if (MCCODE[i] != null)
                        {
                            string Str = MCCODE[i].Split(' ')[^1];
                            if (vars[l].name == Str)
                            {
                                string HEXADDR = Convert.ToString(int.Parse(vars[l].Value), 16);
                                if (HEXADDR.Length == 4)
                                    MCCODE[i] = MCCODE[i].Split(' ', 2)[0] + " " + HEXADDR.PadLeft(4, '0').ToUpper();
                                if (HEXADDR.Length == 2)
                                    MCCODE[i] = MCCODE[i].Split(' ', 2)[0] + " " + HEXADDR.PadLeft(2, '0').ToUpper();
                            }
                        }
                    }
                }
            }
            return;
            // end here
        }
        public void PreAssembler()
        {
            if (AssemblerOps.UseChars)
                SPLITCHAR();

            if (AssemblerOps.UseStrings)
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
                    string[] text = MCCODE[i].Split(' ', 2);
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
                        byte[] HEX = { (byte)MCCODE[i][index + S + 1] };
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
            bool ZP = false;
            string ARGS1 = "";
            string ARGS2 = "";
            string OPCODE;
            if (MCCODE[i].Contains('\t'))
            {
                string SPLIT = MCCODE[i].Split('\t')[1];
                SPLIT = SPLIT.Replace(',', ' ');
                if (SPLIT.Contains(' '))
                {
                    OPCODE = SPLIT.Split(' ')[0];
                    if (SPLIT.Contains('('))
                    {
                        if (AssemblerOps.UseStrings)
                            ARGS1 = SPLIT.Split('(', ')')[1];
                    }
                    else
                    {
                        ARGS1 = SPLIT.Split(' ')[1].PadLeft(2, '0');
                        if (AssemblerOps.UseRegs)
                        {

                            if (SPLIT.Split(' ').Length == 3)
                            {
                                if (SPLIT.Split(' ')[2].Contains(ZPC))
                                {
                                    string[] SPLITA = SPLIT.Split(' ');
                                    if (SPLITA[2][0] == ZPC && SPLITA[1][0] != ZPC)
                                    {
                                        ARGS1 = "01";
                                        if (SPLIT.Split(' ')[2].Split(ZPC)[1][0] == RA)
                                            ARGS2 = SPLIT.Split(' ')[1] + "00";
                                        if (SPLIT.Split(' ')[2].Split(ZPC)[1][0] == RB)
                                            ARGS2 = SPLIT.Split(' ')[1] + "01";
                                        if (SPLIT.Split(' ')[2].Split(ZPC)[1][0] == RC)
                                            ARGS2 = SPLIT.Split(' ')[1] + "02";
                                        if (SPLIT.Split(' ')[2].Split(ZPC)[1][0] == RD)
                                            ARGS2 = SPLIT.Split(' ')[1] + "03";
                                    }
                                    else if (SPLITA[2][0] == ZPC && SPLITA[1][0] == ZPC)
                                    {
                                        ARGS1 = "02";
                                        if (SPLITA[1].Split(ZPC)[1][0] == RA)
                                            ARGS2 = "00";
                                        if (SPLITA[1].Split(ZPC)[1][0] == RB)
                                            ARGS2 = "01";
                                        if (SPLITA[1].Split(ZPC)[1][0] == RC)
                                            ARGS2 = "02";
                                        if (SPLITA[1].Split(ZPC)[1][0] == RD)
                                            ARGS2 = "03";

                                        if (SPLITA[2].Split(ZPC)[1][0] == RA)
                                            ARGS2 += "00";
                                        if (SPLITA[2].Split(ZPC)[1][0] == RB)
                                            ARGS2 += "01";
                                        if (SPLITA[2].Split(ZPC)[1][0] == RC)
                                            ARGS2 += "02";
                                        if (SPLITA[2].Split(ZPC)[1][0] == RD)
                                            ARGS2 += "03";
                                    }
                                }
                                else
                                {
                                    ARGS2 = SPLIT.Split(' ')[2].PadLeft(4, '0');
                                }
                            }
                        }
                    }
                }
                else
                {
                    OPCODE = SPLIT.PadLeft(4, '0');
                }
                TOMCCODE(OPCODE, ARGS1.ToUpper(), ZP, ARGS2.ToUpper());
            }
        }
        public void TOMCCODE(string OPCODE, string ARGS1, bool ZP, string ARGS2 = "")
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
                        MCINDEX++;
                        return;
                    }
                    if (ARGS1 == "") // NULL
                    {
                        MCCODE[MCINDEX] = code + "00";
                        MCINDEX++;
                        return;
                    }
                    else if (ARGS1.Length == 4) // ADDR & ARGS2
                    {
                        if (iNSTR[i] == CPUItems.INSTR.STOI)
                            MCCODE[MCINDEX] = code + "01";
                        else
                            MCCODE[MCINDEX] = code + "00";
                        MCINDEX++;
                        MCCODE[MCINDEX] = ARGS1;
                        TriggerTO = true;
                        MCINDEX++;
                        return;
                    }
                    else if (ARGS1.Length > 4) // STRING & ARGS1
                    {
                        int len = ARGS1.Split(' ').Length;
                        MCCODE[MCINDEX] = code + len.ToString().PadLeft(2, '0') + " (" + ARGS1 + ")";
                        MCINDEX++;
                        return;
                    }
                    else // IMM & ARGS1
                    {
                        MCCODE[MCINDEX] = code + ARGS1;
                        MCINDEX++;
                        return;
                    }
                }
            }
            error += "ERROR at line " + MCINDEX + " with code " + OPCODE + " A1 " + ARGS1 + " A2 " + ARGS2 + " ZP = " + ZP + "\n";
        }
    }
}
