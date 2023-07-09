using CPUTing.HIGHLEVELLANG.Complier;
using System;
using System.IO;
using System.Collections.Generic;
using CrypticWizard.HexConverter;
namespace CPUTing
{
    internal class Program : ConsoleMenu
    {
        public string[] InportedPaths = new string[0xFFFF + 1];
        private PORT PORTs;
        private CPU CPU;
        private MEM MEM;
        private MEM MEM1;
        private Complier Complier;
        private readonly Assembler Assembler;
        private string[] code;
        private readonly bool useCompiler = false;
        private readonly string BMasmpath = "D:/2019/repos/CPUTing/BMASM/PROGRAM.BMasm";
        private readonly string BZpath = "D:/2019/repos/CPUTing/HIGHLEVELLANG/Porgram.BZ";
        private readonly string MarcoCommands = "D:/2019/repos/CPUTing/MarcoCommands.txt";
        private const string CHARS = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKKLZXCVBNM1234567890#@";
        string MarcoArgs;
        private static void Main()
        {
            _ = new Program();
        }
        bool RUN = true;
        private Program()
        {
            Assembler = new Assembler();
            CPU = new CPU();
            MEM = new MEM();
            MEM1 = new MEM();
            PORTs = new PORT();
            Complier = new Complier();
            Console.SetWindowSize(40, 20);
            Console.SetBufferSize(40, 20);
            Console.CursorSize = 25;
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
            UPDATE();
        }
        string[] ASSOPS;
        private void UPDATE()
        {
            Console.SetWindowSize(40, 20);
            Console.SetBufferSize(40, 20);
            while (RUN == true)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 19);
                string User = Console.ReadLine();
                if (User.Contains(' '))
                {
                    ASSOPS = User.Split(' ');
                }
                if(User == "M" || User == "Marco" || User == "m" || User == "marco")
                {
                    string[] SplitMarcoCode = File.ReadAllText(MarcoCommands).Split("\r\n");
                    for (int i = 0; i < SplitMarcoCode.Length; i++)
                    {
                        if(SplitMarcoCode[i].Contains(' '))
                        {
                            MarcoArgs = SplitMarcoCode[i].Split(' ', 2)[1];
                            GetFuns(SplitMarcoCode[i].Split(' ', 2)[0]);
                        }
                        else
                        {
                            GetFuns(SplitMarcoCode[i]);
                        }
                    }
                }
                GetFuns(User);
            }
        }
        public override void Build()
        {
            AssemblerOps.UseRegs = true;
            AssemblerOps.UseDots = true;
            AssemblerOps.UseChars = true;
            AssemblerOps.UseStrings = true;
            AssemblerOps.UseKeyTags = true;
            AssemblerOps.UseLables = true;
            AssemblerOps.UseVars = true;
            Assembler.Reset();
            if (useCompiler == true)
            {
                RUNCOMPILER();
            }
            else
            {
                RUNRAWCODE();
            }
            /*
            for (int i = 0; i < ASSOPS.Length; i++)
            {
                if(ASSOPS[i] == "-I")
                {

                }
                if (ASSOPS[i] == "-o")
                {

                }
                if (ASSOPS[i] == "-F")
                {
                    //formats in AssmblerOps.txt
                }
                if (ASSOPS[i] == "-z")
                {

                }
            }
            */
            Assembler.Start();

            if (Assembler.error == "")
            {
                Console.WriteLine(Assembler.error);
                Console.WriteLine("there is " + Assembler.error.Split("\n").Length + "errors ");
            }

            FILLCODE();
        }
        public override void Debug() => CPU.Debug = !CPU.Debug;
        public override void Inport()
        {
            string Dir;
            string FileName = "";
            int Cursor = 0;
            if (MarcoArgs == "")
            {
                Console.WriteLine("write the Dir to the file");
                Dir = Console.ReadLine();
            }
            else
            {
                FileName = MarcoArgs.Split(' ')[0];
                Dir = MarcoArgs.Split(' ')[1];
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(Dir);
            FileInfo[] files = directoryInfo.GetFiles();
            int Ext = 10;
            if(files.Length <= 10)
            {
                Ext = files.Length;
            }
            for (int i = Cursor; i < Cursor + Ext; i++)
            {
                if (MarcoArgs == "")
                {
                    do
                    {
                        ConsoleKey KEY = Console.ReadKey().Key;
                        if (Cursor != 0 && KEY == ConsoleKey.UpArrow)
                        {
                            Cursor--;
                        }
                        if (Cursor != files.Length - 10 && KEY == ConsoleKey.DownArrow)
                        {
                            Cursor++;
                        }
                        if (KEY == ConsoleKey.Enter)
                        {
                            FileName = files[Cursor].FullName;
                        }
                    } while (FileName != "");
                }
                else
                {
                    if(files[i].Name == FileName.Split('.')[0])
                    {

                    }
                }
            }
        }
        public override void Run()
        {
            CPU.SReset();
            PORTs.STARTVIDEO();
            Console.SetWindowSize(20, 20);
            Console.SetBufferSize(20, 20);
            RUNCPU();
            Console.SetWindowSize(40, 20);
            Console.SetBufferSize(40, 20);
            Console.CursorSize = 25;
            Console.ResetColor();
        } 
        public override void Write()
        {
            int DownLInes = 0;
            Console.CursorVisible = true;
            Console.CursorSize = 100;
            int LINE = 0;
            int XLINE = 0;
            int EDIT = 0;
            int MENU = 1;
            code = File.ReadAllText(BZpath).Split("\r\n");
            //Loop
            while (MENU == 1)
            {
                ConsoleKeyInfo KEY = Console.ReadKey();
                Console.Clear();
                DoCompiler();
                for (int i = DownLInes; i < code.Length; i++)
                {
                    Console.WriteLine(code[i].TrimStart('\t'));
                }
                if (KEY.Key == ConsoleKey.Insert)
                {
                    EDIT = 1;
                    // Edit
                }
                if (KEY.Key == ConsoleKey.Escape)
                {
                    MENU = 0;
                }
                if (LINE != -1 && KEY.Key == ConsoleKey.UpArrow)
                {
                    LINE--;
                }
                if (LINE != 20 && KEY.Key == ConsoleKey.DownArrow)
                {
                    LINE++;
                }
                if (LINE == 20)
                {
                    LINE = 19;
                    DownLInes++;
                }
                if (LINE == -1)
                {
                    LINE = 0;
                    if (DownLInes != 0)
                        DownLInes--;
                }
                Console.SetCursorPosition(0, LINE);
                #region EDIT
                int Write = 0;
                string WriteText = "";
                while (EDIT == 1)
                {
                    KEY = Console.ReadKey();
                    Console.Clear();
                    if (Write == 0)
                    {
                        for (int i = DownLInes; i < code.Length; i++)
                        {
                            Console.WriteLine(code[i].TrimStart('\t'));
                        }
                    }
                    if (KEY.Key == ConsoleKey.Delete)
                    {
                        EDIT = 0;
                        XLINE = 0;
                        //exit edit
                    }
                    if (KEY.Key == ConsoleKey.Backspace)
                    {
                        code[LINE] = code[LINE].Remove(1);
                    }
                    MOVECursor(KEY, XLINE, LINE, out XLINE, out LINE);
                    if (KEY.Key == ConsoleKey.F1)
                    {
                        Write = 1;
                        WriteText = code[LINE];
                    }
                    #region Write
                    while (Write == 1)
                    {
                        KEY = Console.ReadKey();
                        MOVECursor(KEY, XLINE, LINE, out XLINE, out LINE);
                        for (int t = 0; t < CHARS.Length; t++)
                        {
                            if (KEY.KeyChar == CHARS[t])
                            {
                                WriteText = WriteText.Replace(WriteText[XLINE], CHARS[t]);
                                XLINE++;
                            }
                        }
                        if (KEY.Key == ConsoleKey.Backspace)
                        {
                            WriteText = WriteText.Remove(XLINE, XLINE + 1); //todo this not work :(
                            XLINE--;
                        }
                        if (KEY.Key == ConsoleKey.Escape)
                        {
                            Write = 0;
                            if (code[LINE] != WriteText)
                                code[LINE] = WriteText;
                        }
                        Console.SetCursorPosition(XLINE, LINE);
                    }
                    #endregion
                    Console.SetCursorPosition(XLINE, LINE);
                }
                #endregion
            }
        }
        public override void Dump()
        {
            bool EXIT = false;
            ushort CursorY = 0;
            ConsoleKeyInfo KEY;
            do
            {
                KEY = Console.ReadKey();
                Console.Clear();
                DUMPer(CursorY);
                if (KEY.Key == ConsoleKey.Escape)
                {
                    EXIT = true;
                }
                if (CursorY != 0 && KEY.Key == ConsoleKey.UpArrow)
                {
                    CursorY--;
                }
                if (CursorY != Assembler.MCCODE.Length - 10 && KEY.Key == ConsoleKey.DownArrow)
                {
                    CursorY++;
                }
                if(CursorY >= 0xFFFF - 10)
                {
                    CursorY = 0xFFFF - 10;
                }
                if(KEY.Key == ConsoleKey.J)
                {
                    Console.WriteLine("Write a line number");
                    string JumpLine = Console.ReadLine();
                    ushort line;
                    if (JumpLine.Contains('#'))
                    {
                        line = Convert.ToUInt16(JumpLine.Remove(0, 1), 16);
                    }
                    else
                    {
                        line = ushort.Parse(JumpLine);
                    }
                    CursorY = line;
                }
                if (KEY.Key == ConsoleKey.F)
                {
                    Console.WriteLine("Find");
                    string FindString = Console.ReadLine();
                    Console.Clear();
                    for (int i = 0; i < Assembler.MCCODE.Length; i++)
                    {
                        if(Assembler.MCCODE[i] == FindString)
                        {
                            Console.WriteLine("At line " + Convert.ToString(i, 16));
                        }
                    }
                }
                if (KEY.Key == ConsoleKey.E)
                {
                    Console.WriteLine("Find");
                    string FindString = Console.ReadLine();
                    Console.Clear();
                    for (ushort i = 0; i < Assembler.MCCODE.Length; i++)
                    {
                        if (Assembler.MCCODE[i] == FindString)
                        {
                            Console.WriteLine("At line " + Convert.ToString(i, 16));
                            CursorY = i;
                            break;
                        }
                    }
                }
            } while (EXIT == false);
        }
        public override void DumpRam()
        {
            bool EXIT = false;
            ushort CursorY = 0;
            ConsoleKeyInfo KEY;
            do
            {
                KEY = Console.ReadKey();
                Console.Clear();
                DUMPerRam(CursorY);
                if (KEY.Key == ConsoleKey.Escape)
                {
                    EXIT = true;
                }
                if (CursorY != 0 && KEY.Key == ConsoleKey.UpArrow)
                {
                    CursorY--;
                }
                if (CursorY != Assembler.MEMRAM.Length - 10 && KEY.Key == ConsoleKey.DownArrow)
                {
                    CursorY++;
                }
                if (CursorY >= 0xFFFF - 10)
                {
                    CursorY = 0xFFFF - 10;
                }
                if (KEY.Key == ConsoleKey.J)
                {
                    Console.WriteLine("Write a line number");
                    string JumpLine = Console.ReadLine();
                    ushort line;
                    if (JumpLine.Contains('#'))
                    {
                        line = Convert.ToUInt16(JumpLine.Remove(0, 1), 16);
                    }
                    else
                    {
                        line = ushort.Parse(JumpLine);
                    }
                    CursorY = line;
                }
                if (KEY.Key == ConsoleKey.F)
                {
                    Console.WriteLine("Find value");
                    string FindString = Console.ReadLine();
                    Console.Clear();
                    for (int i = 0; i < Assembler.MEMRAM.Length; i++)
                    {
                        if (Assembler.MEMRAM[i] == byte.Parse(FindString))
                        {
                            Console.WriteLine("At line " + Convert.ToString(i, 16));
                        }
                    }
                }
                if (KEY.Key == ConsoleKey.E)
                {
                    Console.WriteLine("Find");
                    string FindString = Console.ReadLine();
                    Console.Clear();
                    for (ushort i = 0; i < Assembler.MEMRAM.Length; i++)
                    {
                        if (Assembler.MEMRAM[i] == byte.Parse(FindString))
                        {
                            Console.WriteLine("At line " + Convert.ToString(i, 16));
                            CursorY = i;
                            break;
                        }
                    }
                }
            } while (EXIT == false);
        }
        public override void Exit()
        {
            RUN = false;
        }
        public void MOVECursor(ConsoleKeyInfo KEY, int XLINE, int LINE, out int XLINEO, out int LINEO)
        {
            if (XLINE != Console.WindowWidth && KEY.Key == ConsoleKey.RightArrow)
            {
                XLINE++;
            }
            if (XLINE != 0 && KEY.Key == ConsoleKey.LeftArrow)
            {
                XLINE--;
            }
            Console.SetCursorPosition(XLINE, LINE);
            XLINEO = XLINE;
            LINEO = LINE;
        }
        public void DUMPer(ushort CursorY)
        {
            for (int i = CursorY; i < CursorY + 10; i++)
            {
                if (!(CursorY >= 0xFFFF) && Assembler.MCCODE[i] != null)
                {
                    Console.Write(Convert.ToString(i, 16).PadLeft(4, '0') + " ");
                    Console.Write(Assembler.MCCODE[i] + "   ");
                    Console.WriteLine("");
                }
            }
        }
        public void DUMPerRam(ushort CursorY)
        {
            for (int i = CursorY; i < CursorY + 10; i++)
            {
                if (!(CursorY >= 0xFFFF) && Assembler.MEMRAM[i].ToString() != null)
                {
                    Console.Write(Convert.ToString(i, 16).PadLeft(4, '0') + " ");
                    Console.Write(Convert.ToString(Assembler.MEMRAM[i], 16) + "   ");
                    Console.WriteLine("");
                }
            }
        }
        private void RUNRAWCODE()
        {
            code = File.ReadAllText(BMasmpath).Split("\r\n");
            for (int i = 0; i < code.Length; i++)
            {
                Assembler.ASMCODE[i] = code[i];
            }
            Assembler.CODELEN = code.Length;
        }
        void DoCompiler()
        {
            code = File.ReadAllText(BZpath).Split("\r\n");
            Complier.Start(code);
            code = File.ReadAllText(BZpath).Split("\r\n");
            //Complier.Start(code, false);
        }
        string[] ASMCODE;
        string ERORRS;
        private void RUNCOMPILER()
        {
            DoCompiler();
            //Complier.GetCode(out ASMCODE);
            //Complier.GetErorrs(out ERORRS);
            if (ERORRS == "")
            {
                for (int i = 0; i < code.Length; i++)
                {
                    Assembler.ASMCODE[i] = code[i];
                }
            }
            else
            {
                Console.WriteLine("ERORRS LEN:" + (ERORRS.Split(',').Length - 1));
                for (int i = 0; i < ERORRS.Split(',').Length; i++)
                {
                    Console.WriteLine(ERORRS.Split(',')[i]);
                }
            }
        }
        private void FILLCODE()
        {
            PORTs.START();
            MEM.START();
            MEM1.START();
            CPU.START(MEM, out MEM);
            MEM.RAM = Assembler.MEMRAM;
            // todo Assembler is not working the lens and 0xFFFF + 1
            for (int i = 0; i < Assembler.MCCODE.Length; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(i / 100 + " Lines in");
                if (Assembler.MCCODE[i] != null)
                {
                    MEM.ROM[i] = Assembler.MCCODE[i].ToUpper();
                }
            }
        }
        private void RUNCPU()
        {
            //running the CPU
            while (CPU.SR[CPU.SR_RUNNING] == true)
            {
                CPU.TICK(MEM, PORTs);
            }
            CPU.RESET(MEM, out MEM);
            return;
        }
    }
    public abstract class ConsoleMenu
    { 
        public string[] _Build = new string[] { "Build", "build", "B", "b", "bfc", "Bjc" };
        public string[] _Debug = new string[] { "Debug", "debug", "DB", "db", "dB", "Db" };
        public string[] _RUN = new string[] { "Run", "run", "r", "R", "RUN", "ruN"};
        public string[] _Write = new string[] { "Write", "write", "w", "W", "WRITE", "writE" };
        public string[] _Dump = new string[] { "dump", "Dump", "d", "D", "DUMP", "dumP" };
        public string[] _DumpRam = new string[] { "dumpram", "DumpRam", "dr", "DR", "DUMPRAM", "dumpraM" };
        public string[] _Exit = new string[] { "Exit", "exit", "E", "e", "exiT", "EXIT" };
        public string[] _Inport = new string[] { "Inport", "inport", "I", "i", "IP", "ip" };
        public void Start()
        {
        }
        public void GetFuns(string user)
        {
            for (int i = 0; i < _Build.Length; i++)
            {
                if (_Build[i].Equals(user))
                {
                    Build();
                }
                if (_Debug[i].Equals(user))
                {
                    Debug();
                }
                if (_Inport[i].Equals(user))
                {
                    Inport();
                }
                if (_RUN[i].Equals(user))
                {
                    Run();
                }
                if (_Write[i].Equals(user))
                {
                    Write();
                }
                if (_Dump[i].Equals(user))
                {
                    Dump();
                }
                if (_DumpRam[i].Equals(user))
                {
                    DumpRam();
                }
                if (_Exit[i].Equals(user))
                {
                    Exit();
                }
            }
        }
        public abstract void Inport();
        public abstract void Debug();
        public abstract void Build();
        public abstract void Run();
        public abstract void Write();
        public abstract void Dump();
        public abstract void DumpRam();
        public abstract void Exit();
    }
}
