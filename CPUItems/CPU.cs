using CrypticWizard.HexConverter;
using System;
using System.Numerics;
namespace CPUTing.CPUItems
{
    public struct CPU
    {
        public bool Debug { get; set; }
        public const byte SR_RUNNING = 0;
        public const byte SR_KEYHERE = 1;

        public const byte FG_ZERO = 0;
        public const byte FG_TRUE = 1;
        public const byte FG_LESS = 2;
        public const byte FG_OVER = 3;
        public const byte FG_INTS = 4;
        public const byte FG_PLUS = 5;
        public const byte FG_CARR = 6;

        ///<summary>PC Start ADDR</summary>
        public const ushort PCSTARTADDR = 0xFFF7;
        ///<summary>CUSOR POS X</summary>
        public const ushort CXpos = 0xFE00;
        ///<summary>CUSOR POS Y</summary>
        public const ushort CYpos = 0xFE01;
        ///<summary>Cursor Style ADDR</summary>
        public const ushort CursorStyle = 0xFE02;
        ///<summary>Console BG Color ADDR</summary>
        public const ushort ConsoleBGColor = 0xFE03;
        ///<summary>Clear ADDR</summary>
        public const ushort ClearConsole = 0xFE04;
        ///<summary>if 1 the user can get a key else never set a key</summary>
        public const ushort GetAKey = 0xFE05;
        ///<summary>STACK POINTER ADDR</summary>
        public const ushort SP = 0xFC00;
        ///<summary>subroutine addr L</summary>
        public const ushort SUPL = 0xFB00;
        ///<summary>subroutine addr H</summary>
        public const ushort SUPH = 0xFB01;
        ///<summary>SOUND enbale ADDR</summary>
        public const ushort SOUNDADDR = 0x7100;
        ///<summary>The Addr for an INT</summary>
        public ushort INTVectorAddr;
        ///<summary>INT addr L</summary>
        public const ushort INTL = 0xFF00;
        ///<summary>INT addr H</summary>
        public const ushort INTH = 0xFF01;

        //char array len
        public byte CAL { get; private set; }
        //char array
        public char[] CA { get; private set; }
        //A REG
        public byte A { get; private set; }
        //B REG
        public byte B { get; private set; }
        //for the PORT IO
        public byte C { get; private set; }
        //D REG
        public byte D { get; private set; }
        INSTR LastINSTR;
        public bool BA { get; private set; }
        public ushort PC { get; private set; }

        public byte[] FLAGS { get; private set; }
        public bool[] SR { get; private set; }
        public bool LoadedAddr { get; private set; }
        public bool ReadWrite { get; private set; }
        public byte DATABUS;
        public ushort ADDRBUS;
        public void START(MEM MEM, out MEM Omem)
        {
            RESET(MEM, out Omem);
        }
        public void RESET(MEM MEM, out MEM OMEM)
        {
            FLAGS = new byte[8];
            SR = new bool[8];
            CA = new char[0xFF];
            for (int i = 0; i < FLAGS.Length; i++)
            {
                SR[i] = false;
                FLAGS[i] = 0;
            }
            for (int i = 0; i < CA.Length; i++)
            {
                CA[i] = ' '; // char array
            }
            SR[SR_RUNNING] = true;
            CAL = 0; // Char array len
            A = 0;
            B = 0;
            C = 0;
            D = 0;
            BA = false;
            PC = PCSTARTADDR;
            MEM.ROM[0xFFF7] = "0901";
            MEM.ROM[0xFFF8] = "FE04";
            MEM.ROM[0xFFF9] = "0900";
            MEM.ROM[0xFFFA] = "FE00";
            MEM.ROM[0xFFFB] = "0900";
            MEM.ROM[0xFFFC] = "FE01";
            MEM.ROM[0xFFFD] = "3500";
            MEM.ROM[0xFFFE] = "2F00";
            MEM.ROM[0xFFFF] = "0000";
            MEM.RAM[SP] = 0x00;
            OMEM = MEM;
            DATABUS = 0;
        }
        public void SReset()
        {
            for (int i = 0; i < FLAGS.Length; i++)
            {
                SR[i] = false;
                FLAGS[i] = 0;
            }
            for (int i = 0; i < CA.Length; i++)
            {
                CA[i] = ' '; // char array
            }
            SR[SR_RUNNING] = true;
            CAL = 0; // Char array len
            A = 0;
            B = 0;
            C = 0;
            D = 0;
            BA = false;
            PC = PCSTARTADDR;
            DATABUS = 0;
        }
        public void TICK(MEM MEM, PORT PORT)
        {
            if (LastINSTR == INSTR.UINT)
                FLAGS[FG_INTS] = 0;
            string test = "";
            test += MEM.RAM[INTH].ToString().PadLeft(2, '0'); //H
            test += MEM.RAM[INTL].ToString().PadLeft(2, '0'); //L
            INTVectorAddr = ushort.Parse(test);

            if (MEM.RAM[CursorStyle] != 0b00000000)
            {
                string Bytes = Convert.ToString(MEM.RAM[CursorStyle], 2).PadRight(8, '0');
                if (Bytes[0] == '1')
                    Console.CursorVisible = true;
                else
                    Console.CursorVisible = false;

                if (Bytes[1] == '1')
                    Console.CursorSize = 25;

                if (Bytes[2] == '1')
                    Console.CursorSize = 50;

                if (Bytes[3] == '1')
                    Console.CursorSize = 100;

                if (Bytes[4] == '1')
                    Console.ForegroundColor = ConsoleColor.Yellow;

                if (Bytes[5] == '1')
                    Console.ForegroundColor = ConsoleColor.White;

                if (Bytes[6] == '1')
                    Console.ForegroundColor = ConsoleColor.Green;

                if (Bytes[7] == '1')
                    Console.ForegroundColor = ConsoleColor.Red;
            }
            if (MEM.RAM[ConsoleBGColor] >= 15)
            {
                ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), MEM.RAM[ConsoleBGColor].ToString());
                Console.BackgroundColor = color;
            }
            if (MEM.RAM[SOUNDADDR] == 1)
            {
                Console.Beep();
                MEM.RAM[SOUNDADDR] = 0;
            }
            if (MEM.RAM[ClearConsole] == 1)
            {
                Console.Clear();
                MEM.RAM[ClearConsole] = 0;
            }

            if (Debug == true)
            {
                DebugerForCPU.UpdateCPU(this, MEM, PORT);
            }

            SR[SR_KEYHERE] = Console.KeyAvailable;

            INSTR instr = DECODE(MEM.ROM[PC], false);
            EXE(instr, MEM, PORT);
            LastINSTR = instr;
            PC++;
        }
        public INSTR DECODE(string STR, bool JMP, bool ZP = false, bool REG = false)
        {
            byte[] HEXBYTES;
            byte BYTES;
            ushort ADDR;
            if (LoadedAddr == false && REG == true)
            {
                HEXBYTES = HexConverter.GetBytes(STR);
                string REGSTR1;
                RegInput(HEXBYTES[0], out REGSTR1);
                string REGSTR2;
                RegInput(HEXBYTES[1], out REGSTR2);
                string ALL = REGSTR1 + REGSTR2;
                ADDR = Convert.ToUInt16(ALL, 16);
                ADDRBUS = ADDR;
                return INSTR.NOOP;
            }
            if (LoadedAddr == false && ZP == true)
            {
                HEXBYTES = HexConverter.GetBytes(STR);
                string ZPSTR;
                RegInput(HEXBYTES, out ZPSTR);
                ADDR = Convert.ToUInt16(ZPSTR, 16);
                ADDRBUS = ADDR;
                return INSTR.NOOP;
            }
            if (JMP == false)
            {
                string decode = STR.Remove(2, 2);
                string ARGS = STR.Remove(0, 2);
                string SUBSTR;
                if (STR.Contains('(')) // to decode a SIM
                {
                    string[] vs = STR.Split(' ', 2);
                    SUBSTR = vs[0];
                    string HEX = vs[1].Split('(', ')')[1];
                    HEXBYTES = HexConverter.GetBytes(HEX.Split(' ')); //todo \ <- this char dose not work in BZasm as a Char look in WOZMON.BZasm
                    for (int i = 0; i < HEXBYTES.Length; i++)
                    {
                        CA[i] = Convert.ToChar(HEXBYTES[i]);
                        CAL = (byte)i;
                    }
                }
                else //IMM
                {
                    SUBSTR = STR;
                }

                DATABUS = byte.Parse(HexConverter.GetBytes(ARGS)[0].ToString());
                if (LoadedAddr == false)
                {
                    HEXBYTES = HexConverter.GetBytes(SUBSTR);
                    BYTES = (byte)(HEXBYTES[0] + HEXBYTES[1]);
                    ADDRBUS = BYTES;
                }
                else
                {
                    LoadedAddr = false;
                }
                return (INSTR)Enum.Parse(typeof(INSTR), HexConverter.GetBytes(decode)[0].ToString());
            }
            else // AIM
            {
                _ = HexConverter.GetBytes(STR);
                ADDR = Convert.ToUInt16(STR, 16);
                ADDRBUS = ADDR;
                return INSTR.NOOP;
            }
        }
        void RegInput(byte[] HEXBYTES, out string OUTSTR)
        {
            if (HEXBYTES[1] == 00)
            {
                OUTSTR = Convert.ToString(HEXBYTES[0], 16).PadRight(2, '0') + Convert.ToString(A, 16).PadLeft(2, '0');
                return;
            }
            if (HEXBYTES[1] == 01)
            {
                OUTSTR = Convert.ToString(HEXBYTES[0], 16).PadRight(2, '0') + Convert.ToString(B, 16).PadLeft(2, '0');
                return;
            }
            if (HEXBYTES[1] == 02)
            {
                OUTSTR = Convert.ToString(HEXBYTES[0], 16).PadRight(2, '0') + Convert.ToString(C, 16).PadLeft(2, '0');
                return;
            }
            if (HEXBYTES[1] == 03)
            {
                OUTSTR = Convert.ToString(HEXBYTES[0], 16).PadRight(2, '0') + Convert.ToString(D, 16).PadLeft(2, '0');
                return;
            }
            OUTSTR = "";
        }
        void RegInput(byte HEXBYTES, out string OUTSTR)
        {
            if (HEXBYTES == 00)
            {
                OUTSTR = Convert.ToString(A, 16).PadLeft(2, '0');
                return;
            }
            if (HEXBYTES == 01)
            {
                OUTSTR = Convert.ToString(B, 16).PadLeft(2, '0');
                return;
            }
            if (HEXBYTES == 02)
            {
                OUTSTR = Convert.ToString(C, 16).PadLeft(2, '0');
                return;
            }
            if (HEXBYTES == 03)
            {
                OUTSTR = Convert.ToString(D, 16).PadLeft(2, '0');
                return;
            }
            OUTSTR = "";
        }
        public void EXE(INSTR INSTR, MEM MEM, PORT PORTs)
        {
            byte[] TOHEX;
            byte PORTINDEX = 0;
            byte PORTINSTR = 0;
            switch (INSTR)
            {
                case INSTR.HLTC:
                    SR[SR_RUNNING] = false;
                    break;
                case INSTR.NOOP:
                    break;
                case INSTR.CALL:
                    PC++;
                    PUSHPC(MEM);
                    DECODE(MEM.ROM[PC], true);
                    PC = ADDRBUS;
                    PC--;
                    break;
                case INSTR.RESR:
                    POPPC(MEM);
                    break;
                case INSTR.INBY:
                    if (DATABUS != 0xE0 && DATABUS != 0xF0)
                    {
                        string StringBus = DATABUS.ToString();
                        if (StringBus.Length == 1)
                        {
                            PORTINSTR = 0;
                            PORTINDEX = DATABUS;
                        }
                        else
                        {
                            TOHEX = new byte[] { byte.Parse(DATABUS.ToString()[0].ToString() + DATABUS.ToString()[1].ToString()) };
                            PORTINSTR = byte.Parse(HexConverter.GetHexString(TOHEX)[0].ToString()); //H
                            PORTINDEX = byte.Parse(HexConverter.GetHexString(TOHEX)[1].ToString()); //L
                        }
                    }
                    else
                    {
                        PORTINSTR = DATABUS;
                    }
                    switch (PORTINSTR)
                    {
                        case 0:
                            PORTs.LOADTEXT(PORTINDEX);
                            break;
                        case 1:
                            PORTs.LoadKEY(PORTINDEX, MEM);
                            break;
                        case 2:
                            PORTs.LoadVideo(PORTINDEX);
                            break;
                        case 3:
                            PORTs.LoadSOUND(PORTINDEX);
                            break;
                        case 0xe0:
                            PORTs.InputReg = C;
                            break;
                        case 0xf0:
                            LOAD(PORTs.OutputReg, Reg.C);
                            break;
                    }
                    break;
                case INSTR.OUTB:
                    //now outputing an 8 bit number aka byte
                    LOAD(PORTs.OutputReg, Reg.C); // here keys in \u+001B and not ascii 0x1B
                    break;
                case INSTR.MOVA:
                    A = MOV(A, GetReg(DATABUS));
                    break;
                case INSTR.MOVB:
                    B = MOV(B, GetReg(DATABUS));
                    break;
                case INSTR.MOVC:
                    C = MOV(C, Reg.A);
                    break;
                case INSTR.STOI:
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    MEM.RAM[ADDRBUS] = DATABUS;
                    break;
                case INSTR.PUHI:
                    PUSH(MEM, DATABUS);
                    break;
                case INSTR.PUHR:
                    PUHR(MEM, DATABUS);
                    break;
                case INSTR.PUHS:
                    PUHS(MEM, CA, CAL);
                    break;
                case INSTR.POPR:
                    POPR(MEM, DATABUS);
                    break;
                case INSTR.LODA:
                    LOAD(DATABUS, Reg.A);
                    break;
                case INSTR.LOAA:
                    GETADDR(MEM);
                    LODADDR(MEM, Reg.A, ADDRBUS);
                    break;
                case INSTR.STOA:
                    GETADDR(MEM);
                    STORE(MEM, Reg.A, ADDRBUS);
                    break;
                case INSTR.DECR:
                    DECR(DATABUS);
                    break;
                case INSTR.INCR:
                    INCR(DATABUS);
                    break;
                case INSTR.CMPA:
                    CMP(A, DATABUS);
                    break;
                case INSTR.LODB:
                    LOAD(DATABUS, Reg.B);
                    break;
                case INSTR.LOAB:
                    GETADDR(MEM);
                    LODADDR(MEM, Reg.B, ADDRBUS);
                    break;
                case INSTR.STOB:
                    GETADDR(MEM);
                    STORE(MEM, Reg.B, ADDRBUS);
                    break;
                case INSTR.CMPB:
                    CMP(B, DATABUS);
                    break;
                case INSTR.LODC:
                    LOAD(DATABUS, Reg.C);
                    break;
                case INSTR.LOAC:
                    GETADDR(MEM);
                    LODADDR(MEM, Reg.C, ADDRBUS);
                    break;
                case INSTR.STOC:
                    GETADDR(MEM);
                    STORE(MEM, Reg.C, ADDRBUS);
                    break;
                case INSTR.CMPC:
                    CMP(C, DATABUS);
                    break;
                case INSTR.LODD:
                    LOAD(DATABUS, Reg.D);
                    break;
                case INSTR.LOAD:
                    GETADDR(MEM);
                    LODADDR(MEM, Reg.D, ADDRBUS);
                    break;
                case INSTR.STOD:
                    GETADDR(MEM);
                    STORE(MEM, Reg.D, ADDRBUS);
                    break;
                case INSTR.CMPD:
                    CMP(D, DATABUS);
                    break;
                case INSTR.ADDI:
                    ADD(true, DATABUS);
                    break;
                case INSTR.SUBI:
                    SUB(true, DATABUS);
                    break;
                case INSTR.ADDR:
                    ADD(false, DATABUS);
                    break;
                case INSTR.SUBR:
                    SUB(false, DATABUS);
                    break;
                case INSTR.CMPF:
                    CMPF(DATABUS);
                    break;
                case INSTR.ANDI:
                    Logic(DATABUS, false, LOGICOPenum.ADD);
                    break;
                case INSTR.NOTI:
                    Logic(DATABUS, false, LOGICOPenum.NOT);
                    break;
                case INSTR.ORIM:
                    Logic(DATABUS, false, LOGICOPenum.OR);
                    break;
                case INSTR.NORI:
                    Logic(DATABUS, false, LOGICOPenum.NOR);
                    break;
                case INSTR.ANDR:
                    Logic(DATABUS, true, LOGICOPenum.ADD);
                    break;
                case INSTR.NOTR:
                    Logic(DATABUS, true, LOGICOPenum.NOT);
                    break;
                case INSTR.ORRE:
                    Logic(DATABUS, true, LOGICOPenum.OR);
                    break;
                case INSTR.NORR:
                    Logic(DATABUS, true, LOGICOPenum.NOR);
                    break;
                case INSTR.MOBA:
                    if (BA == true)
                        BA = false;
                    else
                        BA = true;
                    break;
                case INSTR.INTA:
                    PC++;
                    CALL(MEM, (ushort)(PC + 1));
                    DECODE(MEM.ROM[PC], true);
                    PC = ADDRBUS;
                    PC--;
                    break;
                case INSTR.JUMP:
                    Jump(MEM, 1);
                    break;
                case INSTR.JINZ:
                    Jump(MEM, 0, FG_ZERO);
                    break;
                case INSTR.JINT:
                    Jump(MEM, 0, FG_TRUE);
                    break;
                case INSTR.JIFT:
                    Jump(MEM, 1, FG_TRUE);
                    break;
                case INSTR.INTR:
                    PUSHPC(MEM);
                    PC = INTVectorAddr;
                    PC--;
                    break;
                case INSTR.REIN:
                    POPPC(MEM);
                    break;
                case INSTR.UINT:
                    FLAGS[FG_INTS] = 1;
                    byte INTINDEX;
                    byte INTINSTR;
                    if (DATABUS.ToString().Length == 1)
                    {
                        INTINSTR = DATABUS;
                    }
                    else
                    {
                        TOHEX = new byte[] { byte.Parse(DATABUS.ToString()[0].ToString() + DATABUS.ToString()[1].ToString()) };
                        INTINSTR = byte.Parse(HexConverter.GetHexString(TOHEX)[0].ToString()); //H
                        INTINDEX = byte.Parse(HexConverter.GetHexString(TOHEX)[1].ToString()); //L
                    }
                    UpdateINT(INTINSTR, MEM, PORTs);
                    break;
                case INSTR.ROLR:
                    Rotate(DATABUS, true);
                    break;
                case INSTR.RORR:
                    Rotate(DATABUS, false);
                    break;
                case INSTR.JIFC:
                    Jump(MEM, 1, FG_CARR);
                    break;
                case INSTR.JINO:
                    Jump(MEM, 0, FG_OVER);
                    break;
                case INSTR.GROM:
                    GETADDR(MEM);
                    GETROM(MEM, ADDRBUS);
                    break;
                case INSTR.LSLR:
                    LogicalShift(DATABUS, true);
                    break;
                case INSTR.LSRR:
                    LogicalShift(DATABUS, false);
                    break;
                case INSTR.LODF:
                    LODF(DATABUS);
                    break;
                case INSTR.LOBS:
                    if (ReadWrite == false)
                    {
                        PC--;
                    }
                    else
                    {
                        LOAD(DATABUS, Reg.A);
                        ReadWrite = false;
                    }
                    break;
                case INSTR.LOAR:
                    GETADDR(MEM);
                    LoadedAddr = true;
                    break;
                case INSTR.LOGC:
                    DebugerForCPU.LOG();
                    break;
                default:
                    break;
            }
        }
        public void Jump(MEM MEM, byte result, byte Flag = 8)
        {
            if (Flag >= 8)
            {
                GETADDR(MEM);
                JMP(ADDRBUS);
                PC--;
            }
            else
            {
                if (FLAGS[Flag] == result)
                {
                    GETADDR(MEM);
                    JMP(ADDRBUS);
                    PC--;
                }
                else
                {
                    PC++;
                }
            }
        }
        public void UpdateINT(byte INSTR, MEM MEM, PORT PORT)
        {
            if (INSTR == 0)
                UpdateCursorPos(MEM);
            if (INSTR == 1)
            {
                if (MEM.RAM[GetAKey] == 1)
                    if (SR[SR_KEYHERE] == true)
                        PORT.GETKEYINPUT();
            }
        }
        public void PUSHPC(MEM MEM)
        {
            string IMMS = Convert.ToString(PC + 1).PadLeft(4, '0');
            MEM.RAM[MEM.RAM[SP]] = byte.Parse(IMMS.Remove(0, 2)); //L
            MEM.RAM[SP]++;
            MEM.RAM[MEM.RAM[SP]] = byte.Parse(IMMS.Remove(2, 2)); //H
            MEM.RAM[SP]++;
        }
        public void POPPC(MEM MEM)
        {
            string test = "";
            MEM.RAM[SP]--;
            test += MEM.RAM[MEM.RAM[SP]].ToString().PadLeft(2, '0'); //H
            MEM.RAM[SP]--;
            test += MEM.RAM[MEM.RAM[SP]].ToString().PadLeft(2, '0'); //L
            ushort ADDR = Convert.ToUInt16(test, 10);
            PC = (ushort)(ADDR - 1);
        }
        public void LODADDR(MEM mem, Reg reg, ushort Addr)
        {
            switch (reg)
            {
                case Reg.A:
                    A = mem.RAM[Addr];
                    break;
                case Reg.B:
                    B = mem.RAM[Addr];
                    break;
                case Reg.C:
                    C = mem.RAM[Addr];
                    break;
                case Reg.D:
                    D = mem.RAM[Addr];
                    break;
                default:
                    break;
            }
        }
        public void GETROM(MEM MEM, ushort Addr)
        {
            if (BA == true)
            {
                byte[] HEX = HexConverter.GetBytes(MEM.ROM[Addr]);
                MEM.RAM[SUPH] = HEX[0];
                MEM.RAM[SUPL] = HEX[1];
            }
        }
        public void STORE(MEM mem, Reg reg, ushort Addr)
        {
            if (BA == true)
                mem.ROM[Addr] = A.ToString() + B.ToString();
            if (BA == false)
            {
                switch (reg)
                {
                    case Reg.A:
                        mem.RAM[Addr] = A;
                        break;
                    case Reg.B:
                        mem.RAM[Addr] = B;
                        break;
                    case Reg.C:
                        mem.RAM[Addr] = C;
                        break;
                    case Reg.D:
                        mem.RAM[Addr] = D;
                        break;
                    default:
                        break;
                }
            }
        }
        public void ADD(bool IMMOrBREG, byte imm)
        {
            if (IMMOrBREG == false)
            {
                switch (GetReg(imm))
                {
                    case Reg.A:
                        A = (byte)(A + A);
                        ALUCMP(A, A);
                        break;
                    case Reg.B:
                        A = (byte)(A + B);
                        ALUCMP(A, B);
                        break;
                    case Reg.C:
                        A = (byte)(A + C);
                        ALUCMP(A, C);
                        break;
                    case Reg.D:
                        A = (byte)(A + D);
                        ALUCMP(A, D);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                A += imm;
                ALUCMP(A, imm);
            }
        }
        public void SUB(bool IMMOrBREG, byte imm)
        {
            if (IMMOrBREG == false)
            {
                switch (GetReg(imm))
                {
                    case Reg.A:
                        A = (byte)(A - A);
                        ALUCMP(A, A);
                        break;
                    case Reg.B:
                        A = (byte)(A - B);
                        ALUCMP(A, B);
                        break;
                    case Reg.C:
                        A = (byte)(A - C);
                        ALUCMP(A, C);
                        break;
                    case Reg.D:
                        A = (byte)(A - D);
                        ALUCMP(A, D);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                A -= imm;
                ALUCMP(A, imm);
            }
        }
        public void ALUCMP(byte REGData, byte IMM)
        {
            if (REGData + IMM > 0xFF || REGData + IMM < 0x00)
            {
                FLAGS[FG_OVER] = 1;
                FLAGS[FG_CARR] = 1;
            }
            else
            {
                FLAGS[FG_OVER] = 0;
                FLAGS[FG_CARR] = 0;
            }
        }
        public void UpdateCursorPos(MEM MEM)
        {
            Console.SetCursorPosition(MEM.RAM[CXpos], MEM.RAM[CYpos]);
        }
        public void PUHR(MEM MEM, byte Imm)
        {
            byte regData = 0;
            switch (GetReg(Imm))
            {
                case Reg.A:
                    regData = A;
                    break;
                case Reg.B:
                    regData = B;
                    break;
                case Reg.C:
                    regData = C;
                    break;
                case Reg.D:
                    regData = D;
                    break;
                default:
                    break;
            }
            MEM.RAM[MEM.RAM[SP]] = regData;
            MEM.RAM[SP]++;
        }
        public void PUSH(MEM MEM, byte Imm)
        {
            MEM.RAM[MEM.RAM[SP]] = Imm;
            MEM.RAM[SP]++;
        }
        public void PUHS(MEM MEM, char[] Imm, byte LEN)
        {
            for (int i = LEN; i > -1; i--)
            {
                MEM.RAM[MEM.RAM[SP]] = (byte)Imm[i];
                MEM.RAM[SP]++;
            }
        }
        public void CALL(MEM MEM, ushort Imm)
        {
            string IMMS = Convert.ToString(Imm).PadLeft(4, '0');
            MEM.RAM[SUPL] = byte.Parse(IMMS.Remove(0, 2)); //L
            MEM.RAM[SUPH] = byte.Parse(IMMS.Remove(2, 2)); //H
        }
        public void RESR(MEM MEM)
        {
            string test = "";
            test += MEM.RAM[SUPH].ToString().PadLeft(2, '0'); //H
            test += MEM.RAM[SUPL].ToString().PadLeft(2, '0'); //L
            ushort ADDR = Convert.ToUInt16(test, 10);
            PC = (ushort)(ADDR - 1);
        }
        public void POPR(MEM MEM, byte Imm)
        {
            MEM.RAM[SP]--;
            switch (GetReg(Imm))
            {
                case Reg.A:
                    A = MEM.RAM[MEM.RAM[SP]];
                    break;
                case Reg.B:
                    B = MEM.RAM[MEM.RAM[SP]];
                    break;
                case Reg.C:
                    C = MEM.RAM[MEM.RAM[SP]];
                    break;
                case Reg.D:
                    D = MEM.RAM[MEM.RAM[SP]];
                    break;
                default:
                    break;
            }
        }
        public void LOAD(byte Imm, Reg reg)
        {
            switch (reg)
            {
                case Reg.A:
                    A = Imm;
                    CMPREGS(A);
                    break;
                case Reg.B:
                    B = Imm;
                    CMPREGS(B);
                    break;
                case Reg.C:
                    C = Imm;
                    CMPREGS(C);
                    break;
                case Reg.D:
                    D = Imm;
                    CMPREGS(D);
                    break;
                default:
                    break;
            }
        }
        public void LODF(byte Imm)
        {
            string Bin = Convert.ToString(Imm, 2);
            for (int i = 0; i < FLAGS.Length; i++)
            {
                FLAGS[i] = 0;
            }
            for (int i = 0; i < Bin.Length; i++)
            {
                FLAGS[i] = byte.Parse(Bin[i].ToString());
            }

        }
        void CMPREGS(byte RegByte)
        {
            if (RegByte == 0)
                FLAGS[FG_ZERO] = 1;
            else
                FLAGS[FG_ZERO] = 0;
        }
        void CMPINCANDDEC(byte RegData, byte PreReg)
        {
            if (PreReg == 0xFF && RegData == 0)
            {
                FLAGS[FG_OVER] = 1;
            }
            else
            {
                FLAGS[FG_OVER] = 0;
            }
            if (PreReg == 0 && RegData == 0xFF)
            {
                FLAGS[FG_PLUS] = 1;
            }
            else
            {
                FLAGS[FG_PLUS] = 0;
            }
        }
        public void DECR(byte IMM)
        {
            byte Pre = 0;
            switch (GetReg(IMM))
            {
                case Reg.A:
                    A--;
                    CMPREGS(A);
                    Pre = (byte)(A + 1);
                    CMPINCANDDEC(A, Pre);
                    break;
                case Reg.B:
                    B--;
                    CMPREGS(B);
                    Pre = (byte)(B + 1);
                    CMPINCANDDEC(B, Pre);
                    break;
                case Reg.C:
                    C--;
                    CMPREGS(C);
                    Pre = (byte)(C + 1);
                    CMPINCANDDEC(C, Pre);
                    break;
                case Reg.D:
                    D--;
                    CMPREGS(D);
                    Pre = (byte)(D + 1);
                    CMPINCANDDEC(D, Pre);
                    break;
                default:
                    break;
            }
        }
        public void INCR(byte IMM)
        {
            byte Pre = 0;
            switch (GetReg(IMM))
            {
                case Reg.A:
                    A++;
                    CMPREGS(A);
                    Pre = (byte)(A - 1);
                    CMPINCANDDEC(A, Pre);
                    break;
                case Reg.B:
                    B++;
                    CMPREGS(B);
                    Pre = (byte)(B - 1);
                    CMPINCANDDEC(B, Pre);
                    break;
                case Reg.C:
                    C++;
                    CMPREGS(C);
                    Pre = (byte)(C - 1);
                    CMPINCANDDEC(C, Pre);
                    break;
                case Reg.D:
                    D++;
                    CMPREGS(D);
                    Pre = (byte)(D - 1);
                    CMPINCANDDEC(D, Pre);
                    break;
                default:
                    break;
            }
        }
        public void JMP(ushort ADDR)
        {
            PC = ADDR;
        }
        public void CMP(byte REGData, byte imm)
        {
            if (REGData == imm)
            {
                FLAGS[FG_TRUE] = 1;
            }
            else
            {
                FLAGS[FG_TRUE] = 0;
            }
            if (REGData == 0)
            {
                FLAGS[FG_ZERO] = 1;
            }
            else
            {
                FLAGS[FG_ZERO] = 0;
            }
            if (REGData < imm)
            {
                FLAGS[FG_LESS] = 1;
            }
            else
            {
                FLAGS[FG_LESS] = 0;
            }
        }
        public void Logic(byte Imm, bool RegORImm, LOGICOPenum lOGICOP)
        {
            if (RegORImm == true) //REG
            {
                switch (GetReg(Imm))
                {
                    case Reg.A:
                        A = LOGICOP(lOGICOP, A, A);
                        break;
                    case Reg.B:
                        A = LOGICOP(lOGICOP, A, B);
                        break;
                    case Reg.C:
                        A = LOGICOP(lOGICOP, A, C);
                        break;
                    case Reg.D:
                        A = LOGICOP(lOGICOP, A, D);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                A = LOGICOP(lOGICOP, A, Imm);
            }
        }
        byte LOGICOP(LOGICOPenum lOGICOP, byte F, byte L)
        {
            switch (lOGICOP)
            {
                case LOGICOPenum.ADD:
                    return (byte)(F & L);
                case LOGICOPenum.NOT:
                    return NOT(F);
                case LOGICOPenum.OR:
                    return (byte)(F | L);
                case LOGICOPenum.NOR:
                    return NOT((byte)(F | L));
                default:
                    break;
            }
            return 0;
        }
        byte NOT(byte A)
        {
            string RE = "";
            string SA = Convert.ToString(A, 2);
            for (int i = 0; i < SA.Length; i++)
            {
                if (SA.ToString()[i] == '1')
                    RE += "0";
                else
                    RE += "1";
            }
            return Convert.ToByte(RE, 2);
        }
        public void CMPF(byte imm)
        {
            string Simm = Convert.ToString(imm, 2);
            byte result;
            for (int i = 0; i < Simm.Length; i++)
            {
                result = (byte)(byte.Parse(Simm[i].ToString()) & FLAGS[i]);
                if (result == 1)
                {
                    FLAGS[FG_TRUE] = 1;
                }
                else
                {
                    FLAGS[FG_TRUE] = 0;
                }
            }
        }
        public byte MOV(byte F, Reg L)
        {
            byte Buffer = 0;
            switch (L)
            {
                case Reg.A:
                    Buffer = A;
                    A = F;
                    break;
                case Reg.B:
                    Buffer = B;
                    B = F;
                    break;
                case Reg.C:
                    Buffer = C;
                    C = F;
                    break;
                case Reg.D:
                    Buffer = D;
                    D = F;
                    break;
            }
            return Buffer;

        }
        public void Rotate(byte RegByte, bool left)
        {
            switch (GetReg(RegByte))
            {
                case Reg.A:
                    A = rot(A, left);
                    break;
                case Reg.B:
                    B = rot(B, left);
                    break;
                case Reg.C:
                    C = rot(C, left);
                    break;
                case Reg.D:
                    D = rot(D, left);
                    break;
            }
        }
        public void LogicalShift(byte RegByte, bool Left)
        {
            byte Input = 0;
            switch (GetReg(RegByte))
            {
                case Reg.A:
                    Input = A;
                    break;
                case Reg.B:
                    Input = B;
                    break;
                case Reg.C:
                    Input = C;
                    break;
                case Reg.D:
                    Input = D;
                    break;
            }
            char[] RegBin = new char[8];
            string BinString = Convert.ToString(Input, 2).PadLeft(8, '0');
            for (int i = 0; i < BinString.ToCharArray().Length; i++)
            {
                RegBin[i] = BinString.ToCharArray()[i];
            }
            if (Left == false)
            {
                for (int i = RegBin.Length; i < 0; i--)
                {
                    RegBin[i] = RegBin[i - 1];
                }
                /*
                RegBin[7] = RegBin[6];
                RegBin[6] = RegBin[5];
                RegBin[5] = RegBin[4];
                RegBin[4] = RegBin[3];
                RegBin[3] = RegBin[2];
                RegBin[2] = RegBin[1];
                RegBin[1] = RegBin[0];
                */
                RegBin[0] = '0';
            }
            else
            {
                for (int i = 0; i < RegBin.Length; i++)
                {
                    RegBin[i] = RegBin[i + 1];
                }
                /*
                RegBin[0] = RegBin[1];
                RegBin[1] = RegBin[2];
                RegBin[2] = RegBin[3];
                RegBin[3] = RegBin[4];
                RegBin[4] = RegBin[5];
                RegBin[5] = RegBin[6];
                RegBin[6] = RegBin[7];
                */
                RegBin[7] = '0';
            }
            for (int i = 0; i < BinString.Length; i++)
            {
                BinString += RegBin[i];
            }

            byte OutPut = Convert.ToByte(BinString, 2);
            switch (GetReg(RegByte))
            {
                case Reg.A:
                    A = OutPut;
                    break;
                case Reg.B:
                    B = OutPut;
                    break;
                case Reg.C:
                    C = OutPut;
                    break;
                case Reg.D:
                    D = OutPut;
                    break;
            }
        }
        public byte rot(byte RegValues, bool left)
        {
            if (left == true)
            {
                return (byte)BitOperations.RotateLeft(RegValues, 1);
            }
            else
            {
                return (byte)BitOperations.RotateRight(RegValues, 1);
            }
        }
        public void WriteIn(byte Data)
        {
            ReadWrite = true;
            DATABUS = Data;
        }
        public void ReadIn(out byte Data)
        {
            ReadWrite = false;
            Data = DATABUS;
        }
        public void GETADDR(MEM MEM)
        {
            PC++;
            if (DATABUS == 0) //ASB ADDR
            {
                DECODE(MEM.ROM[PC], true);
            }
            else if (DATABUS == 1) //ZP ADDR
            {
                DECODE(MEM.ROM[PC], false, true);
            }
            else if (DATABUS == 2)
            {
                DECODE(MEM.ROM[PC], false, true);
            }
        }
        public Reg GetReg(byte IMM)
        {
            Reg reg = (Reg)Enum.Parse(typeof(Reg), IMM.ToString());
            return reg;
        }
    }
    public static class DebugerForCPU
    {
        public static CPU CPU;
        public static MEM MEM;
        public static PORT PORT;
        public static void UpdateCPU(CPU cPU, MEM mEM, PORT pORT)
        {
            CPU = cPU;
            MEM = mEM;
            PORT = pORT;
        }
        public static void LOG()
        {
            Console.SetWindowSize(40, 20);
            Console.SetBufferSize(40, 20);
            Console.Clear();
            Console.WriteLine("CPU");
            Console.WriteLine("A:" + CPU.A + "(" + Convert.ToString(CPU.A, 16) + ") " +
                              "B:" + CPU.B + "(" + Convert.ToString(CPU.B, 16) + ") " +
                              "C:" + CPU.C + "(" + Convert.ToString(CPU.C, 16) + ") " +
                              "D:" + CPU.D + "(" + Convert.ToString(CPU.D, 16) + ")");

            Console.WriteLine("PC:" + CPU.PC + "(" + Convert.ToString(CPU.PC, 16) + ") " +
                              "RW:" + CPU.ReadWrite +
                              "DB:" + CPU.DATABUS + "(" + Convert.ToString(CPU.DATABUS, 16) + ") " +
                              "AB:" + CPU.ADDRBUS + "(" + Convert.ToString(CPU.ADDRBUS, 16) + ") ");

            Console.WriteLine("MEM");
            Console.WriteLine("CX:" + MEM.RAM[CPU.CXpos] + "(" + Convert.ToString(MEM.RAM[CPU.CXpos], 16) + ") " +
                              "CY:" + MEM.RAM[CPU.CYpos] + "(" + Convert.ToString(MEM.RAM[CPU.CYpos], 16) + ") " +
                              "CS:" + MEM.RAM[CPU.CursorStyle] + "(" + Convert.ToString(MEM.RAM[CPU.CursorStyle], 16) + ") " +
                              "CC:" + MEM.RAM[CPU.ConsoleBGColor] + "(" + Convert.ToString(MEM.RAM[CPU.ConsoleBGColor], 16) + ")");

            Console.WriteLine("SP:" + MEM.RAM[CPU.SP] + "(" + Convert.ToString(MEM.RAM[CPU.SP], 16) + ") " +
                              "AL:" + MEM.RAM[CPU.SUPL] + "(" + Convert.ToString(MEM.RAM[CPU.SUPL], 16) + ") " +
                              "AH:" + MEM.RAM[CPU.SUPH] + "(" + Convert.ToString(MEM.RAM[CPU.SUPH], 16) + ") " +
                              "IL:" + MEM.RAM[CPU.INTL] + "(" + Convert.ToString(MEM.RAM[CPU.INTL], 16) + ") " +
                              "IH:" + MEM.RAM[CPU.INTH] + "(" + Convert.ToString(MEM.RAM[CPU.INTH], 16) + ") ");

            Console.WriteLine("PORT");
            Console.WriteLine("IR:" + PORT.InputReg + "(" + Convert.ToString(PORT.InputReg, 16) + ") " +
                              "OR:" + PORT.OutputReg + "(" + Convert.ToString(PORT.OutputReg, 16) + ") " +
                              "IN:" + PORT.INDEX + "(" + Convert.ToString(PORT.INDEX, 16) + ") ");

            Console.ReadKey();
            Console.SetWindowSize(20, 20);
            Console.SetBufferSize(20, 20);
            Console.Clear();
        }
    }
}
