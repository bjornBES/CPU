using CPUTing.HIGHLEVELLANG.Complier;
using System;
using System.IO;
using System.Collections.Generic;
using CrypticWizard.HexConverter;
namespace CPUTing
{
    internal class Program : ConsoleMenu
    {
        private readonly PORT PORTs;
        private CPU CPU;
        private MEM MEM;
        private readonly Assembler Assembler;
        public Complier Complier;
        private string[] code;
        private readonly bool useCompiler = false;
        private readonly string BMasmpath = "D:/2019/repos/CPUTing/BMASM/PROGRAM.BMasm";
        private readonly string BZpath = "D:/2019/repos/CPUTing/HIGHLEVELLANG/Porgram.BZ";
        private const string CHARS = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKKLZXCVBNM1234567890#@";
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
        private void UPDATE()
        {
            Console.SetWindowSize(40, 20);
            Console.SetBufferSize(40, 20);
            while (RUN == true)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 19);
                string User = Console.ReadLine();
                GetFuns(User);
            }
        }
        public override void Build()
        {
            if (useCompiler == true)
            {
                RUNCOMPILER();
            }
            else
            {
                RUNRAWCODE();
            }

            Assembler.Start();

            FILLCODE();
        }
        public override void Run()
        {
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
            ConsoleKeyInfo KEY = new ConsoleKeyInfo();
            code = File.ReadAllText(BZpath).Split("\r\n");
            string[] CODEBUFFER = new string[1000];
            //Loop
            while (MENU == 1)
            {
                KEY = Console.ReadKey();
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
            // todo this also not work like all my other code
            bool EXIT = false;
            int CursorY = 0;
            Assembler.Reset();
            if(useCompiler == true)
            {
                RUNCOMPILER();
            }
            else
            {
                RUNRAWCODE();
            }
            Assembler.Start();
            do
            {
                ConsoleKeyInfo KEY = Console.ReadKey();
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
                if (CursorY != Assembler.MCCODE.Length && KEY.Key == ConsoleKey.DownArrow)
                {
                    CursorY++;
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
        public void DUMPer(int CursorY)
        {
            for (int i = CursorY; i < Assembler.MCCODE.Length; i++)
            {
                if (Assembler.MCCODE[i] != null)
                {
                    Console.Write(Convert.ToString(i, 16).PadLeft(4, '0') + " ");
                    Console.Write(Assembler.MCCODE[i] + "   ");
                    Console.WriteLine("");
                }
            }
        }
        private void INTIT()
        {
            if (useCompiler == true)
            {
                RUNCOMPILER();
            }
            else
            {
                RUNRAWCODE();
            }

            Assembler.Start();

            FILLCODE();
            RUNCPU();
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
            Complier.Start(code, true);
            code = File.ReadAllText(BZpath).Split("\r\n");
            Complier.Start(code, false);
        }
        string[] ASMCODE;
        string ERORRS;
        private void RUNCOMPILER()
        {
            DoCompiler();
            Complier.GetCode(out ASMCODE);
            Complier.GetErorrs(out ERORRS);
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
            MEM.START();
            PORTs.START();
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
            CPU.RESET();
            return;
        }
    }
    public abstract class ConsoleMenu
    { 
        public string[] _Build = new string[] { "Build", "build", "B", "b" };
        public string[] _RUN = new string[] { "Run", "run", "r", "R"};
        public string[] _Write = new string[] { "Write", "write", "w", "W" };
        public string[] _Dump = new string[] { "dump", "Dump", "d", "D" };
        public string[] _Exit = new string[] { "Exit", "exit", "E", "e" };
        public void Start()
        {
        }
        public void GetFuns(string user)
        {
            for (int i = 0; i < _Build.Length; i++)
            {
                if(_Build[i].Equals(user))
                {
                    Build();
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
                if (_Exit[i].Equals(user))
                {
                    Exit();
                }
            }
        }
        public abstract void Build();
        public abstract void Run();
        public abstract void Write();
        public abstract void Dump();
        public abstract void Exit();
    }
}
