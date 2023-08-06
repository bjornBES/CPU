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
        private MarcoMan MarcoMan;
        private Complier Complier;
        private readonly Assembler Assembler;
        private string[] code;
        private readonly bool useCompiler = false;
        private readonly string BMasmpath = "D:/2019/repos/CPUTing/BMASM/PROGRAM.BMasm";
        private readonly string BZpath = "D:/2019/repos/CPUTing/HIGHLEVELLANG";
        private readonly string MarcoCommands = "D:/2019/repos/CPUTing/MarcoCommands.txt";
        private const string CHARS = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKKLZXCVBNM1234567890#@";
        string MarcoArgs;
        string FileName;
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
            MarcoMan = new MarcoMan();
            Console.SetWindowSize(40, 20);
            Console.SetBufferSize(40, 20);
            Console.CursorSize = 25;
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
            FileInfo[] files;
            DirectoryInfo directory;
            if (Directory.Exists(BZpath))
            {
                directory = Directory.CreateDirectory(BZpath);
            }
            else
            {
                directory = new DirectoryInfo(BZpath);
            }
            files = directory.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name == "Main.BZ")
                {
                    FileName = "Main.BZ";
                }
            }
            UPDATE();
        }
        string[] ASSOPS;
        //this value is for the marcos
        int ShowDisDown = 0;
        private void UPDATE()
        {
            MarcoMan.SetPath(MarcoCommands);
            MarcoMan.SplitCommands();
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
                    const int MaxUp = 0;
                    const int MaxDown = 246;
                    bool Exit = false;
                    int IndexedCursor = 0;
                    int limitedCursorPos = 2;
                    ShowMarcoNames(0);
                    Console.Clear();
                    Console.SetCursorPosition(0, 2);
                    do
                    {
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("sel a marco and hit enter to run it");
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        Console.Clear();
                        if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            string[] SplitMarcoCode = MarcoMan.GetCommands(IndexedCursor);
                            for (int i = 0; i < SplitMarcoCode.Length; i++)
                            {
                                if (SplitMarcoCode[i].Contains(' '))
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
                            if (limitedCursorPos != 2 && keyInfo.Key == ConsoleKey.UpArrow)
                            {
                                limitedCursorPos--;
                                limitedCursorPos = Math.Clamp(limitedCursorPos, 2, 11);
                                if (IndexedCursor != MaxUp && limitedCursorPos == 2)
                                    IndexedCursor--;
                            }
                            if (IndexedCursor != MaxUp && keyInfo.Key == ConsoleKey.PageUp)
                            {
                                limitedCursorPos = Math.Clamp(limitedCursorPos, 2, 11);
                                IndexedCursor--;
                            }
                        if (IndexedCursor != MaxDown)
                        {
                            if (keyInfo.Key == ConsoleKey.DownArrow)
                            {
                                limitedCursorPos++;
                                limitedCursorPos = Math.Clamp(limitedCursorPos, 2, 11);
                                if (IndexedCursor != MaxDown && limitedCursorPos == 11)
                                    IndexedCursor++;
                            }
                            if (keyInfo.Key == ConsoleKey.PageDown)
                            {
                                limitedCursorPos = Math.Clamp(limitedCursorPos, 2, 11);
                                IndexedCursor++;
                            }
                        }
                        if (keyInfo.Key == ConsoleKey.E || keyInfo.Key == ConsoleKey.Escape)
                        {
                            Exit = true;
                        }
                        Console.SetCursorPosition(0, 2);
                        ShowMarcoNames(IndexedCursor);
                        Console.SetCursorPosition(0, limitedCursorPos);
                        Console.Write('@');

                        Console.SetCursorPosition(0, 13);
                        Console.WriteLine("IndexedCursor" + IndexedCursor);
                        Console.WriteLine("limitedCursorPos" + limitedCursorPos);
                    } while (Exit == false);
                }
                GetFuns(User);
            }
        }
        public void ShowMarcoNames(int IndexCursor)
        {
            for (int i = 0; i < MarcoMan.GetName(IndexCursor, 10).Length; i++)
            {
                Console.WriteLine(" " + MarcoMan.GetName(IndexCursor, 10)[i]);
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


            code = File.ReadAllText(BZpath + "/" + FileName).Split("\r\n");
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
        public override void Files()
        {
            FileInfo[] files = GetFiles();
            bool Exit = false;
            int CursorY = 2;
            int Page = 0;
            do
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("sel a marco and hit enter to run it");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                Console.Clear();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    List<FileInfo> BZFiles = new List<FileInfo>();
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].Extension == ".BZ")
                        {
                            BZFiles.Add(files[i]);
                        }
                    }
                    bool SubExit = false;
                    do
                    {
                        int SubCursorY = 0;
                        int CursorIndexDown = 0;
                        string[] Code = File.ReadAllText(BZFiles.ToArray()[CursorY].FullName).Split('\n');
                        for (int i = CursorIndexDown; i < 10 + CursorIndexDown; i++)
                        {

                        }
                        keyInfo = Console.ReadKey();
                        Console.Clear();
                        Console.WriteLine();
                        if (SubCursorY != 1 && keyInfo.Key == ConsoleKey.UpArrow)
                        {
                            SubCursorY--;
                            if (CursorIndexDown != 0 && SubCursorY == 0)
                            {
                                SubCursorY = 10;
                                CursorIndexDown--;
                            }
                        }
                        if (SubCursorY != 10 && keyInfo.Key == ConsoleKey.DownArrow)
                        {
                            SubCursorY++;
                            if (CursorIndexDown != Code.Length && SubCursorY == 10)
                            {
                                SubCursorY = 0;
                                CursorIndexDown++;
                            }
                        }
                        if (keyInfo.Key == ConsoleKey.E || keyInfo.Key == ConsoleKey.Escape)
                        {
                            SubExit = true;
                        }
                    } while (SubExit == false);
                }
                if (CursorY != 1 && keyInfo.Key == ConsoleKey.UpArrow)
                {
                    CursorY--;
                    if (Page != 0 && CursorY == 1)
                    {
                        CursorY = 12;
                        Page--;
                    }
                }
                if (CursorY != 13 && keyInfo.Key == ConsoleKey.DownArrow)
                {
                    CursorY++;
                    if (Page != Math.Ceiling((double)(files.Length / 10)) && CursorY == 13)
                    {
                        CursorY = 2;
                        Page++;
                    }
                }
                if (keyInfo.Key == ConsoleKey.N)
                {
                    Console.Clear();
                    Console.WriteLine("Write the Name of the File");
                    string FileName = Console.ReadLine();
                    FileStream fileStream = File.Create(BZpath + "/" + FileName + ".BZ", 100);
                    files = GetFiles();
                    fileStream.Close();
                    Console.Clear();
                }
                if (keyInfo.Key == ConsoleKey.D)
                {
                    Console.Clear();
                    List<FileInfo> BZFiles = new List<FileInfo>();
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].Extension == ".BZ")
                        {
                            BZFiles.Add(files[i]);
                        }
                    }
                    Console.WriteLine("do you won't to delete " + BZFiles.ToArray()[CursorY - 3].Name + "? prees Y to do it");
                    ConsoleKey key = Console.ReadKey().Key;
                    if(key == ConsoleKey.Y)
                    {
                        File.Delete(BZFiles.ToArray()[CursorY - 3].FullName);
                    }
                    files = GetFiles();
                    Console.Clear();
                }
                if (keyInfo.Key == ConsoleKey.E || keyInfo.Key == ConsoleKey.Escape)
                {
                    Exit = true;
                }
                Console.SetCursorPosition(1, 2);
                ShowFiles(files);
                Console.SetCursorPosition(0, CursorY);
                Console.Write('@');

                Console.SetCursorPosition(0, 13);
                Console.WriteLine("CursorY" + CursorY);
                Console.WriteLine("Page" + Page);
            } while (Exit == false);
        }
        public FileInfo[] GetFiles()
        {
            DirectoryInfo directory;
            if (Directory.Exists(BZpath))
            {
                directory = Directory.CreateDirectory(BZpath);
            }
            else
            {
                directory = new DirectoryInfo(BZpath);
            }
            return directory.GetFiles();
        }
        public void ShowFiles(FileInfo[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if(files[i].Extension == ".BZ")
                {
                    Console.SetCursorPosition(1, 2 + i);
                    Console.WriteLine(files[i].Name);
                }
            }
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
            code = File.ReadAllText(BZpath + "/" + FileName).Split("\r\n");
            Complier.Start(code);
            code = File.ReadAllText(BZpath + "/" + FileName).Split("\r\n");
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
        public string[] _Files = new string[] { "Files", "files", "f", "F", "FI", "fi" };
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
                if (_Files[i].Equals(user))
                {
                    Files();
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
        public abstract void Files();
    }
}
