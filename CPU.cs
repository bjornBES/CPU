using CrypticWizard.HexConverter;
using System;
namespace CPUTing
{
    public struct CPU
    {
        public const byte SR_RUNNING = 0;
        public const byte SR_KEYHERE = 1;

        public const byte FG_ZERO = 0;
        public const byte FG_TRUE = 1;
        public const byte FG_LESS = 2;
        public const byte FG_OVER = 3;
        public const byte FG_INTS = 4;

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
        ///<summary>STACK POINTER ADDR</summary>
        public const ushort SP = 0xFC00;
        ///<summary>subroutine addr L</summary>
        public const ushort SUPL = 0xFB00;
        ///<summary>subroutine addr H</summary>
        public const ushort SUPH = 0xFA00;
        ///<summary>SOUND enbale ADDR</summary>
        public const ushort SOUNDADDR = 0x7100;
        ///<summary>The Addr for an INT</summary>
        public ushort INTVectorAddr;
        ///<summary>INT addr L</summary>
        public const ushort INTL = 0xFF00;
        ///<summary>INT addr H</summary>
        public const ushort INTH = 0xFF01;

        //char array len
        public byte CAL;
        //char array
        public char[] CA;
        //A REG
        public byte A;
        //B REG
        public byte B;
        //for the PORT IO
        public byte C;
        //D REG
        public byte D;
        public bool BA;
        public ushort PC;

        public byte[] FLAGS;
        public bool[] SR;

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
            MEM.ROM[0xFFFD] = "0800";
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

            SR[SR_KEYHERE] = Console.KeyAvailable;

            INSTR instr = DECODE(MEM.ROM[PC], false);
            EXE(instr, MEM, PORT);
            PC++;
        }
        public INSTR DECODE(string STR, bool JMP)
        {
            byte[] HEXBYTES;
            byte BYTES;
            ushort ADDR;
            if (JMP == false)
            {
                string decode = STR[0].ToString() + STR[1].ToString();
                string ARGS = STR[2].ToString() + STR[3].ToString();
                string SUBSTR;
                if (STR.Contains('('))
                {
                    string[] vs = STR.Split(' ', 2);
                    SUBSTR = vs[0];
                    string HEX = vs[1].Split('(', ')')[1];
                    HEXBYTES = HexConverter.GetBytes(HEX.Split(' '));
                    for (int i = 0; i < HEXBYTES.Length; i++)
                    {
                        CA[i] = Convert.ToChar(HEXBYTES[i]);
                        CAL = (byte)i;
                    }
                }
                else
                {
                    SUBSTR = STR;
                }

                DATABUS = byte.Parse(HexConverter.GetBytes(ARGS)[0].ToString());
                HEXBYTES = HexConverter.GetBytes(SUBSTR);
                BYTES = (byte)(HEXBYTES[0] + HEXBYTES[1]);
                ADDRBUS = BYTES;
                return (INSTR)Enum.Parse(typeof(INSTR), HexConverter.GetBytes(decode)[0].ToString());
            }
            else
            {
                _ = HexConverter.GetBytes(STR);
                ADDR = Convert.ToUInt16(STR, 16);
                ADDRBUS = ADDR;
                return INSTR.NOOP;
            }
        }
        public void EXE(INSTR INSTR, MEM MEM, PORT PORTs)
        {
            byte PORTINDEX = DATABUS;
            byte PORTINSTR;
            switch (INSTR)
            {
                case INSTR.HLTC:
                    SR[SR_RUNNING] = false;
                    break;
                case INSTR.NOOP:
                    break;
                case INSTR.CALL:
                    PC++;
                    CALL(MEM, (ushort)(PC + 1));
                    DECODE(MEM.ROM[PC], true);
                    PC = ADDRBUS;
                    PC--;
                    break;
                case INSTR.RESR:
                    RESR(MEM);
                    break;
                case INSTR.INBY:
                    if (DATABUS.ToString().Length == 1)
                    {
                        PORTINSTR = DATABUS;
                    }
                    else
                    {
                        byte[] TOHEX = { byte.Parse(DATABUS.ToString()[0].ToString() + DATABUS.ToString()[1].ToString()) };
                        PORTINSTR = byte.Parse(HexConverter.GetHexString(TOHEX)[0].ToString()); //H
                        PORTINDEX = byte.Parse(HexConverter.GetHexString(TOHEX)[1].ToString()); //L
                    }
                    switch (PORTINSTR)
                    {
                        case 0:
                            PORTs.LOADWORD(C);
                            break;
                        case 1:
                            PORTs.DISPLAY();
                            break;
                        case 2:
                            if (SR[SR_KEYHERE] == true)
                            {
                                PORTs.GETKEYINPUT();
                            }

                            break;
                        case 3:
                            PORTs.OUTINPUT = 0;
                            Console.Clear();
                            break;
                        case 4:
                            PORTs.OUTINPUT = 0;
                            PORTs.CLS();
                            break;
                    }
                    break;
                case INSTR.OUTB:
                    if (DATABUS.ToString().Length == 1)
                    {
                        PORTINSTR = DATABUS;
                    }
                    else
                    {
                        byte[] TOHEX = { byte.Parse(DATABUS.ToString()[0].ToString() + DATABUS.ToString()[1].ToString()) };
                        PORTINSTR = byte.Parse(HexConverter.GetHexString(TOHEX)[0].ToString()); //H
                        PORTINDEX = byte.Parse(HexConverter.GetHexString(TOHEX)[1].ToString()); //L
                    }
                    switch (PORTINSTR)
                    {
                        case 0:
                            //now outputing an 8 bit number aka byte
                            LOAD(PORTs.OUTINPUT, Reg.C); // here keys in \u+001B and not ascii 0x1B
                            break;
                    }
                    break;
                case INSTR.MOVA:
                    MOV(A, GetReg(DATABUS), out A);
                    break;
                case INSTR.MOVB:
                    MOV(B, GetReg(DATABUS), out B);
                    break;
                case INSTR.UCUR:
                    UpdateCursorPos(MEM);
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
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    LODADDR(MEM, Reg.A, ADDRBUS);
                    PC--;
                    break;
                case INSTR.STOA:
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    STORE(MEM, Reg.A, ADDRBUS);
                    PC--;
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
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    LODADDR(MEM, Reg.B, ADDRBUS);
                    PC--;
                    break;
                case INSTR.STOB:
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    STORE(MEM, Reg.B, ADDRBUS);
                    PC--;
                    break;
                case INSTR.CMPB:
                    CMP(B, DATABUS);
                    break;
                case INSTR.LODC:
                    LOAD(DATABUS, Reg.C);
                    break;
                case INSTR.LOAC:
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    LODADDR(MEM, Reg.C, ADDRBUS);
                    PC--;
                    break;
                case INSTR.STOC:
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    STORE(MEM, Reg.C, ADDRBUS);
                    PC--;
                    break;
                case INSTR.CMPC:
                    CMP(C, DATABUS);
                    break;
                case INSTR.LODD:
                    LOAD(DATABUS, Reg.D);
                    break;
                case INSTR.LOAD:
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    LODADDR(MEM, Reg.D, ADDRBUS);
                    PC--;
                    break;
                case INSTR.STOD:
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    STORE(MEM, Reg.D, ADDRBUS);
                    PC--;
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
                    Logic(DATABUS, false, CPUTing.LOGICOP.ADD);
                    break;
                case INSTR.NOTI:
                    Logic(DATABUS, false, CPUTing.LOGICOP.NOT);
                    break;
                case INSTR.ORIM:
                    Logic(DATABUS, false, CPUTing.LOGICOP.OR);
                    break;
                case INSTR.NORI:
                    Logic(DATABUS, false, CPUTing.LOGICOP.NOR);
                    break;
                case INSTR.ANDR:
                    Logic(DATABUS, true, CPUTing.LOGICOP.ADD);
                    break;
                case INSTR.NOTR:
                    Logic(DATABUS, true, CPUTing.LOGICOP.NOT);
                    break;
                case INSTR.ORRE:
                    Logic(DATABUS, true, CPUTing.LOGICOP.OR);
                    break;
                case INSTR.NORR:
                    Logic(DATABUS, true, CPUTing.LOGICOP.NOR);
                    break;
                case INSTR.MOBA:
                    if (BA == true)
                        BA = false;
                    else
                        BA = true;
                    break;
                case INSTR.INTB:
                    PC++;
                    CALL(MEM, (ushort)(PC + 1));
                    DECODE(MEM.ROM[PC], true);
                    PC = ADDRBUS;
                    PC--;
                    break;
                case INSTR.JUMP:
                    PC++;
                    DECODE(MEM.ROM[PC], true);
                    JMP(ADDRBUS);
                    PC--;
                    break;
                case INSTR.JINZ:
                    if (FLAGS[FG_ZERO] == 0)
                    {
                        PC++;
                        DECODE(MEM.ROM[PC], true);
                        JMP(ADDRBUS);
                        PC--;
                    }
                    else
                    {
                        PC++;
                    }

                    break;
                case INSTR.JINT:
                    if (FLAGS[FG_TRUE] == 0)
                    {
                        PC++;
                        DECODE(MEM.ROM[PC], true);
                        JMP(ADDRBUS);
                        PC--;
                    }
                    else
                    {
                        PC++;
                    }

                    break;
                case INSTR.JIFT:
                    if (FLAGS[FG_TRUE] == 1)
                    {
                        PC++;
                        DECODE(MEM.ROM[PC], true);
                        JMP(ADDRBUS);
                        PC--;
                    }
                    else
                    {
                        PC++;
                    }

                    break;
                case INSTR.INTR:
                    PUSHPC(MEM);
                    PC = INTVectorAddr;
                    PC--;
                    break;
                case INSTR.REIN:
                    POPPC(MEM);
                    break;
                default:
                    break;
            }
        }
        public void PUSHPC(MEM MEM)
        {
            string IMMS = Convert.ToString(PC + 1).PadLeft(4, '0');
            MEM.RAM[MEM.RAM[SP]] = byte.Parse(IMMS.Remove(2, 2)); //H
            MEM.RAM[SP]++;
            MEM.RAM[MEM.RAM[SP]] = byte.Parse(IMMS.Remove(0, 2)); //L
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
            PC = (ushort)(ADDR);
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
                        mem.RAM[Addr] = (byte)C;
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
                        A = (byte)(A + (byte)C);
                        ALUCMP(A, (byte)C);
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
                        A = (byte)(A - (byte)C);
                        ALUCMP(A, (byte)C);
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
        public void ALUCMP(byte REG, byte IMM)
        {
            if (REG + IMM > 0xFF || REG + IMM < 0x00)
            {
                FLAGS[FG_OVER] = 1;
            }
            else
            {
                FLAGS[FG_OVER] = 0;
            }
        }
        public void UpdateCursorPos(MEM MEM)
        {
            Console.SetCursorPosition(MEM.RAM[CXpos], MEM.RAM[CYpos]);
        }
        public void PUHR(MEM MEM, byte Imm)
        {
            byte reg = 0;
            switch (GetReg(Imm))
            {
                case Reg.A:
                    reg = A;
                    break;
                case Reg.B:
                    reg = B;
                    break;
                case Reg.C:
                    reg = (byte)C;
                    break;
                case Reg.D:
                    reg = D;
                    break;
                default:
                    break;
            }
            MEM.RAM[MEM.RAM[SP]] = reg;
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
            MEM.RAM[SUPL] = byte.Parse(IMMS.Remove(0,2)); //L
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
                    CMPREGS((byte)C);
                    break;
                case Reg.D:
                    D = Imm;
                    CMPREGS(D);
                    break;
                default:
                    break;
            }
        }
        void CMPREGS(byte Reg)
        {
            if (Reg == 0)
                FLAGS[FG_ZERO] = 1;
            else
                FLAGS[FG_ZERO] = 0;
        }
        public void DECR(byte IMM)
        {
            switch (GetReg(IMM))
            {
                case Reg.A:
                    A--;
                    CMPREGS(A);
                    break;
                case Reg.B:
                    B--;
                    CMPREGS(B);
                    break;
                case Reg.C:
                    C--;
                    CMPREGS((byte)C);
                    break;
                case Reg.D:
                    D--;
                    CMPREGS(D);
                    break;
                default:
                    break;
            }
        }
        public void INCR(byte IMM)
        {
            switch (GetReg(IMM))
            {
                case Reg.A:
                    A++;
                    CMPREGS(A);
                    break;
                case Reg.B:
                    B++;
                    CMPREGS(B);
                    break;
                case Reg.C:
                    C++;
                    CMPREGS((byte)C);
                    break;
                case Reg.D:
                    D++;
                    CMPREGS(D);
                    break;
                default:
                    break;
            }
        }
        public void JMP(ushort ADDR)
        {
            PC = ADDR;
        }
        public void CMP(byte REG, byte imm)
        {
            if (REG == imm)
            {
                FLAGS[FG_TRUE] = 1;
            }
            else
            {
                FLAGS[FG_TRUE] = 0;
            }
            if (REG == 0)
            {
                FLAGS[FG_ZERO] = 1;
            }
            else
            {
                FLAGS[FG_ZERO] = 0;
            }
            if (REG < imm)
            {
                FLAGS[FG_LESS] = 1;
            }
            else
            {
                FLAGS[FG_LESS] = 0;
            }
        }
        public void Logic(byte Imm, bool RegORImm, LOGICOP lOGICOP)
        {
            if(RegORImm == true) //REG
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
                        A = LOGICOP(lOGICOP, A, (byte)C);
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
        byte LOGICOP(LOGICOP lOGICOP, byte F, byte L)
        {
            switch (lOGICOP)
            {
                case CPUTing.LOGICOP.ADD:
                    return (byte)(F & L);
                case CPUTing.LOGICOP.NOT:
                    return NOT(F);
                case CPUTing.LOGICOP.OR:
                    return (byte)(F | L);
                case CPUTing.LOGICOP.NOR:
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
            string SFLAGS = "";
            for (int i = 0; i < FLAGS.Length; i++)
            {
                SFLAGS += FLAGS[i].ToString();
            }
            string ByteString = Convert.ToString(imm, 2).PadLeft(8, '0');
            for (int i = 0; i < SFLAGS.Length; i++)
            {
                if (SFLAGS[i] == '0' && SFLAGS[i] == '0')
                {
                    FLAGS[FG_TRUE] = 0;
                }
                else
                {
                    if (ByteString[i] == SFLAGS[i])
                    {
                        FLAGS[FG_TRUE] = 1;
                        break;
                    }
                }
            }
        }
        public void MOV(byte F, Reg L, out byte FREG)
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
            FREG = Buffer;

        }
        public Reg GetReg(byte IMM)
        {
            Reg reg = (Reg)Enum.Parse(typeof(Reg), IMM.ToString());
            return reg;
        }
    }
}
