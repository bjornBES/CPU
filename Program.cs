﻿using CPUTing.HIGHLEVELLANG.Complier;
using System;
using System.IO;
using System.Collections.Generic;
using CrypticWizard.HexConverter;
using System.Numerics;
using CPUTing.CPUItems;
using CPUTing.AssemblerMan;
using System.Linq;
using System.Security.Cryptography;

namespace CPUTing
{
    internal class Program : ConsoleMenu
    {
        public string[] InportedPaths = new string[0xFFFF + 1];
        private PORT PORTs;
        private CPU CPU;
        private MEM MEM;
        private MEM MEM1;
        private InfoDecoder InfoDecoder;
        private FileSystem FileSystem;
        private MarcoMan MarcoMan;
        private Complier Complier;
        private readonly Assembler Assembler;
        private string[] code;
        string MarcoArgs;
        string FileName;
        private static void Main()
        {
            _ = new Program();
        }
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
            FileSystem = new FileSystem();
            InfoDecoder = new InfoDecoder(FileSystem.GetInfoText());
            UPDATE();
        }
        bool RUN = true;
        string User = "";
        private void UPDATE()
        {
            MarcoMan.SetPath(FileSystem.GetMarcoPath());
            MarcoMan.SplitCommands();
            Console.SetWindowSize(40, 20);
            Console.SetBufferSize(40, 20);
            while (RUN == true)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 19);
                User = Console.ReadLine();
                GetFuns(User);
            }
        }
        public override void Marco()
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
                if (limitedCursorPos != 1 && keyInfo.Key == ConsoleKey.UpArrow)
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
        public override void SetPath()
        {
            DirectoryInfo directory = new DirectoryInfo("C:");
        }
        public override void Build()
        {
            AssemblerSettings.InterpretString(User);
            Assembler.Reset();
            if (Info.useCompiler == true)
            {
                RUNCOMPILER();
            }
            else
            {
                RUNRAWCODE();
            }
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
            write.StartWrite(FileSystem.BZpath, FileName);
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
            FileInfo[] files = FileSystem.GetAllFiles();
            bool Exit = false;
            int CursorY = 2;
            int Page = 0;
            do
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("sel a marco and hit enter to run it");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                FileSystem.GetFiles();
                List<FileInfo> BZFiles = FileSystem.files.ToList();
                FileInfo file = BZFiles.ToArray()[CursorY - 2];
                Console.Clear();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    bool SubExit = false;
                    do
                    {
                        int[] Keys = { 0, 0 };
                        int SubCursorY = 0;
                        int CursorIndexDown = 0;
                        string[] Code = File.ReadAllText(file.FullName).Split('\n');
                        for (int i = Keys[1]; i < 10 + Keys[1]; i++)
                        {
                            if (Code.Length > i)
                            {
                                Console.WriteLine(Code[i]);
                            }
                        }
                        keyInfo = Console.ReadKey();
                        Console.Clear();
                        Console.WriteLine();
                        MoveCursorFiles(keyInfo, Code, SubCursorY, CursorIndexDown, out Keys);
                        if (keyInfo.Key == ConsoleKey.E)
                        {
                            write.StartWrite(file.FullName);
                        }
                        if (keyInfo.Key == ConsoleKey.Delete || keyInfo.Key == ConsoleKey.Escape)
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
                    FileStream fileStream = File.Create(FileSystem.BZpath + "/" + FileName + ".BZ", 100);
                    files = FileSystem.GetAllFiles();
                    fileStream.Close();
                    Console.Clear();
                }
                if (keyInfo.Key == ConsoleKey.D)
                {
                    Console.Clear();
                    Console.WriteLine("do you won't to delete " + file.Name + "? prees Y to do it");
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Y)
                    {
                        File.Delete(file.FullName);
                    }
                    files = FileSystem.GetAllFiles();
                    Console.Clear();
                }
                if (keyInfo.Key == ConsoleKey.E || keyInfo.Key == ConsoleKey.Escape)
                {
                    Exit = true;
                }
                Console.SetCursorPosition(1, 2);
                FileSystem.ShowFiles(files);
                Console.SetCursorPosition(0, CursorY);
                Console.Write('@');

                Console.SetCursorPosition(0, 13);
                Console.WriteLine("CursorY" + CursorY);
                Console.WriteLine("Page" + Page);
            } while (Exit == false);
        }
        public void MoveCursorFiles(ConsoleKeyInfo keyInfo, string[] Code, int SubCursorY, int CursorIndexDown, out int[] ints)
        {
            int[] outPut = new int[2];
            if (SubCursorY != 1 && keyInfo.Key == ConsoleKey.UpArrow)
            {
                SubCursorY--;
                if (CursorIndexDown != 0 && SubCursorY == 0)
                {
                    SubCursorY = 10;
                    CursorIndexDown--;
                    outPut[0] = SubCursorY;
                    outPut[1] = CursorIndexDown;
                }
            }
            if (SubCursorY != 10 && keyInfo.Key == ConsoleKey.DownArrow)
            {
                SubCursorY++;
                if (CursorIndexDown != Code.Length && SubCursorY == 10)
                {
                    SubCursorY = 0;
                    CursorIndexDown++;
                    outPut[0] = SubCursorY;
                    outPut[1] = CursorIndexDown;
                }
            }
            ints = outPut;
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
            code = File.ReadAllText(FileSystem.BMasmpath).Split("\r\n");
            for (int i = 0; i < code.Length; i++)
            {
                Assembler.ASMCODE[i] = code[i];
            }
            Assembler.CODELEN = code.Length;
        }
        void DoCompiler()
        {
            code = File.ReadAllText(FileSystem.BZpath + "/" + FileName).Split("\r\n");
            Complier.Start(code);
            code = File.ReadAllText(FileSystem.BZpath + "/" + FileName).Split("\r\n");
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
        public void ShowMarcoNames(int IndexCursor)
        {
            for (int i = 0; i < MarcoMan.GetName(IndexCursor, 10).Length; i++)
            {
                Console.WriteLine(" " + MarcoMan.GetName(IndexCursor, 10)[i]);
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
        public string[] _SetPath = new string[] { "Set", "set", "s", "S", "SP", "sp" };
        public string[] _Marcos = new string[] { "Marco", "marco", "M", "m", "RM", "rm" };
        public string[] _Build = new string[] { "Build", "build", "B", "b", "bfc", "Bjc" };
        public string[] _Debug = new string[] { "Debug", "debug", "DB", "db", "dB", "Db" };
        public string[] _RUN = new string[] { "Run", "run", "r", "R", "RUN", "ruN" };
        public string[] _Write = new string[] { "Write", "write", "w", "W", "WRITE", "writE" };
        public string[] _Dump = new string[] { "dump", "Dump", "d", "D", "DUMP", "dumP" };
        public string[] _DumpRam = new string[] { "dumpram", "DumpRam", "dr", "DR", "DUMPRAM", "dumpraM" };
        public string[] _Exit = new string[] { "Exit", "exit", "E", "e", "exiT", "EXIT" };
        public string[] _Inport = new string[] { "Inport", "inport", "I", "i", "IP", "ip" };
        public string[] _Files = new string[] { "Files", "files", "f", "F", "FI", "fi" };
        public void GetFuns(string user)
        {
            for (int i = 0; i < _Build.Length; i++)
            {
                if (_Build[i].Equals(user))
                {
                    Build();
                }
                if (_SetPath[i].Equals(user))
                {
                    SetPath();
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
                if (_Marcos[i].Equals(user))
                {
                    Marco();
                }
            }
        }
        public abstract void SetPath();
        public abstract void Marco();
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
