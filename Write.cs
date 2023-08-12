using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CPUTing
{
    public class write
    {
        static List<string> code;
        static string[] OutCode;
        static int Y = 0, X = 0, PageY = 0;
        public static string[] GetSavedCode()
        {
            return OutCode;
        }
        public static void GetSavedCode(out string[] strings)
        {
            strings = OutCode;
        }
        static void Enter()
        {
            Y++;
            X = 0;
            code.Add("");
            for (int i = Y; i < code.Count - Y; i++)
            {
                code.Insert(i, code[i - 1]);
            }
        }
        static bool CursorMovement(ConsoleKeyInfo keyInfo)
        {
            if (Y != code.Count && keyInfo.Key == ConsoleKey.DownArrow)
            {
                Y++;
                return true;
            }
            else if (Y != 0 && keyInfo.Key == ConsoleKey.UpArrow)
            {
                Y--;
                return true;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                if (code[Y].Length != 0 && X != code[Y].Length)
                {
                    X++;
                    return true;
                }
            }
            else if (X != 0 && keyInfo.Key == ConsoleKey.LeftArrow)
            {
                X--;
                return true;
            }
            return false;
        }
        static void SetCursorPositionToInfo() => Console.SetCursorPosition(0, 19);
        static void GetAndPrintKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Modifiers != ConsoleModifiers.Control)
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Backspace:
                        break;
                    case ConsoleKey.Tab:
                        break;
                    case ConsoleKey.Clear:
                        break;
                    case ConsoleKey.Enter:
                        break;
                    case ConsoleKey.Pause:
                        break;
                    case ConsoleKey.Escape:
                        break;
                    case ConsoleKey.PageUp:
                        break;
                    case ConsoleKey.PageDown:
                        break;
                    case ConsoleKey.End:
                        break;
                    case ConsoleKey.Home:
                        break;
                    case ConsoleKey.LeftArrow:
                        break;
                    case ConsoleKey.UpArrow:
                        break;
                    case ConsoleKey.RightArrow:
                        break;
                    case ConsoleKey.DownArrow:
                        break;
                    case ConsoleKey.Select:
                        break;
                    case ConsoleKey.Print:
                        break;
                    case ConsoleKey.Execute:
                        break;
                    case ConsoleKey.PrintScreen:
                        break;
                    case ConsoleKey.Insert:
                        break;
                    case ConsoleKey.Delete:
                        break;
                    case ConsoleKey.Help:
                        break;
                    case ConsoleKey.LeftWindows:
                        break;
                    case ConsoleKey.RightWindows:
                        break;
                    case ConsoleKey.Applications:
                        break;
                    case ConsoleKey.Sleep:
                        break;
                    case ConsoleKey.F1:
                        break;
                    case ConsoleKey.F2:
                        break;
                    case ConsoleKey.F3:
                        break;
                    case ConsoleKey.F4:
                        break;
                    case ConsoleKey.F5:
                        break;
                    case ConsoleKey.F6:
                        break;
                    case ConsoleKey.F7:
                        break;
                    case ConsoleKey.F8:
                        break;
                    case ConsoleKey.F9:
                        break;
                    case ConsoleKey.F10:
                        break;
                    case ConsoleKey.F11:
                        break;
                    case ConsoleKey.F12:
                        break;
                    case ConsoleKey.F13:
                        break;
                    case ConsoleKey.F14:
                        break;
                    case ConsoleKey.F15:
                        break;
                    case ConsoleKey.F16:
                        break;
                    case ConsoleKey.F17:
                        break;
                    case ConsoleKey.F18:
                        break;
                    case ConsoleKey.F19:
                        break;
                    case ConsoleKey.F20:
                        break;
                    case ConsoleKey.F21:
                        break;
                    case ConsoleKey.F22:
                        break;
                    case ConsoleKey.F23:
                        break;
                    case ConsoleKey.F24:
                        break;
                    case ConsoleKey.BrowserBack:
                        break;
                    case ConsoleKey.BrowserForward:
                        break;
                    case ConsoleKey.BrowserRefresh:
                        break;
                    case ConsoleKey.BrowserStop:
                        break;
                    case ConsoleKey.BrowserSearch:
                        break;
                    case ConsoleKey.BrowserFavorites:
                        break;
                    case ConsoleKey.BrowserHome:
                        break;
                    case ConsoleKey.VolumeMute:
                        break;
                    case ConsoleKey.VolumeDown:
                        break;
                    case ConsoleKey.VolumeUp:
                        break;
                    case ConsoleKey.MediaNext:
                        break;
                    case ConsoleKey.MediaPrevious:
                        break;
                    case ConsoleKey.MediaStop:
                        break;
                    case ConsoleKey.MediaPlay:
                        break;
                    case ConsoleKey.LaunchMail:
                        break;
                    case ConsoleKey.LaunchMediaSelect:
                        break;
                    case ConsoleKey.LaunchApp1:
                        break;
                    case ConsoleKey.LaunchApp2:
                        break;
                    case ConsoleKey.Oem1:
                        break;
                    case ConsoleKey.OemPlus:
                        break;
                    case ConsoleKey.OemComma:
                        break;
                    case ConsoleKey.OemMinus:
                        break;
                    case ConsoleKey.OemPeriod:
                        break;
                    case ConsoleKey.Oem2:
                        break;
                    case ConsoleKey.Oem3:
                        break;
                    case ConsoleKey.Oem4:
                        break;
                    case ConsoleKey.Oem5:
                        break;
                    case ConsoleKey.Oem6:
                        break;
                    case ConsoleKey.Oem7:
                        break;
                    case ConsoleKey.Oem8:
                        break;
                    case ConsoleKey.Oem102:
                        break;
                    case ConsoleKey.Process:
                        break;
                    case ConsoleKey.Packet:
                        break;
                    case ConsoleKey.Attention:
                        break;
                    case ConsoleKey.CrSel:
                        break;
                    case ConsoleKey.ExSel:
                        break;
                    case ConsoleKey.EraseEndOfFile:
                        break;
                    case ConsoleKey.Play:
                        break;
                    case ConsoleKey.Zoom:
                        break;
                    case ConsoleKey.NoName:
                        break;
                    case ConsoleKey.Pa1:
                        break;
                    case ConsoleKey.OemClear:
                        break;
                    default:
                        code[Y] = code[Y].Insert(X, keyInfo.KeyChar.ToString());
                        X++;
                        break;
                }
            }
        }
        public static void Save()
        {
            OutCode = code.ToArray();
            Console.Clear();
            Thread.Sleep(250);
            Console.WriteLine("Saved");
        }
        public static void StartWrite(string BZpath, string File)
        {
            Write(BZpath + "/" +  File);
        }
        public static void StartWrite(string BZpath)
        {
            Write(BZpath);
        }
        static void CursorStart()
        {
            Console.CursorVisible = true;
            Console.CursorSize = 100;
            Console.Title = "Write Code";
            Console.ResetColor();
        }
        static void ResetCursor()
        {
            Console.CursorVisible = true;
            Console.CursorSize = 25;
            Console.Title = "CPU Console";
            Console.ResetColor();
        }
        public static void Write(string BZpath)
        {   
            CursorStart();
            bool MENU = true;
            code = File.ReadAllText(BZpath).Split("\r\n").ToList();
            do
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                Console.Clear();
                if(CursorMovement(keyInfo))
                {

                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Enter();
                }
                else if (keyInfo.Key == ConsoleKey.Delete)
                {
                    code.RemoveAt(Y);
                    Y--;
                    X = 0;
                }
                else if (X != 0 && keyInfo.Key == ConsoleKey.Backspace)
                {
                    X--;
                    code[Y] = code[Y].Remove(X, 1);
                }
                else if ((code[Y].Length != 0 || code[Y].Length != 1) && X > code[Y].Length)
                {
                    X = code[Y].Length;
                }
                else if(keyInfo.Key == ConsoleKey.End)
                {
                    MENU = false;
                    ResetCursor();
                    Console.Clear();
                    return;
                }
                else if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.S)
                {
                    Save();
                }
                else
                {
                    GetAndPrintKey(keyInfo);
                }
                if (Y == 18)
                {
                    PageY++;
                }
                if (Y == -1)
                {
                    PageY--;
                }
                SetCursorPositionToInfo();
                Console.WriteLine("X " + X + " Y " + Y);
                for (int i = PageY; i < 10; i++)
                {
                    if (code.Count > i)
                    {
                        for (int c = 0; c < Console.WindowWidth - 1; c++)
                        {
                            if (c != code[i].Length)
                            {
                                Console.SetCursorPosition(c + 1, i);
                                Console.Write(code[i][c]);
                            }
                            else
                            {
                                break;
                            }
                            if (c == Console.WindowWidth - 1)
                            {
                                break;
                            }
                        }
                        Console.WriteLine();
                    }
                }
                Console.SetCursorPosition(0, Y);
                Console.Write("@");
                Console.SetCursorPosition(X, Y);
            } while (MENU == true);
        }
    }
}
