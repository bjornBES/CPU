using System;
using CPUTing.ConsoleEngine;
using System.Threading;

namespace CPUTing
{
    public class PORT
    {
        ConsoleFuns ConsoleFuns;
        SoundEmu SoundEmu;
        public byte[] ConsoleCharBuffer;
        public bool[] PrintRaw;
        public int INDEX = 0;
        private byte OUTINPUT = 0;
        public byte OutputReg = 0;
        public byte InputReg = 0;
        public void START()
        {
            SoundEmu = new SoundEmu();
            ConsoleCharBuffer = new byte[20 * 20];
            ConsoleCharBuffer.Initialize();
            PrintRaw = new bool[ConsoleCharBuffer.Length];
            PrintRaw.Initialize();
        }
        #region Text
        public void LOADTEXT(byte index)
        {
            switch (index)
            {
                case 0x0:
                    LOADCHAR(InputReg);
                    break;
                case 0x1:
                    LOADRAW(InputReg);
                    break;
                case 0x2:
                    break;
                case 0x3:
                    OUTINPUT = (byte)ConsoleCharBuffer[InputReg];
                    break;
                case 0x4:
                    if (INDEX != 0xff)
                        INDEX++;
                    break;
                case 0x5:
                    DISPLAYBUFFER(InputReg);
                    break;
                case 0x6:
                    DISPLAYONE(InputReg);
                    break;
                case 0x7:
                    OutputReg = 0;
                    Console.Clear();
                    break;
                case 0x8:
                    OutputReg = 0;
                    INDEX = 0;
                    break;
                case 0x9:
                    if(INDEX != 0)
                        INDEX--;
                    break;
                case 0xa:
                    break;
                case 0xb:
                    break;
                case 0xc:
                    break;
                case 0xd:
                    break;
                case 0xe:
                    break;
                case 0xf:
                    break;
            }
        }
        void LOADCHAR(byte C)
        {
            ConsoleCharBuffer[INDEX] = C;
            PrintRaw[INDEX] = false;
            if (INDEX < ConsoleCharBuffer.Length - 1)
                INDEX++;
            else
                INDEX = 0;
        }
        void LOADRAW(byte C)
        {
            ConsoleCharBuffer[INDEX] = C;
            PrintRaw[INDEX] = true;
            if (INDEX < ConsoleCharBuffer.Length - 1)
                INDEX++;
            else
                INDEX = 0;
        }
        void DISPLAYBUFFER(byte CONTROLE = 0)
        {
            for (int i = 0; i < INDEX; i++)
            {
                if (ConsoleCharBuffer[i].ToString().ToUpper() == "\n")
                {
                    Console.WriteLine(" ");
                    i++;
                }
                else
                {
                    if(PrintRaw[i] == true)
                    {
                        if(CONTROLE == 0) //dec
                        {
                            Console.Write(ConsoleCharBuffer[i].ToString().ToUpper());
                        }
                        else if(CONTROLE == 1) //hex
                        {
                            Console.Write(Convert.ToString(ConsoleCharBuffer[i], 16).ToUpper());
                        }
                    }
                    else
                    {
                        char Char = (char)ConsoleCharBuffer[i];
                        Console.Write(Char);
                    }
                }
            }
        }
        void DISPLAYONE(byte CONTROLE = 0)
        {
            if (ConsoleCharBuffer[INDEX].ToString().ToUpper() == "\n")
            {
                Console.WriteLine(" ");
                INDEX++;
            }
            else
            {
                if (PrintRaw[INDEX] == true)
                {
                    if (CONTROLE == 0) //dec
                    {
                        Console.Write(ConsoleCharBuffer[INDEX].ToString().ToUpper());
                    }
                    else if (CONTROLE == 1) //hex
                    {
                        Console.Write(Convert.ToString(ConsoleCharBuffer[INDEX], 16).ToUpper());
                    }
                }
                else
                {
                    char Char = (char)ConsoleCharBuffer[INDEX];
                    Console.Write(Char);
                }
            }
        }
        #endregion
        #region Video
        public void LoadVideo(byte Input)
        {
            switch (Input)
            {
                case 0x0:
                    STARTVIDEO();
                    break;
                case 0x1:
                    break;
                case 0x2:
                    LOADWORD(InputReg);
                    break;
                case 0x3:
                    break;
                case 0x4:
                    break;
                case 0x5:
                    DISPLAY();
                    break;
                case 0x6:
                    break;
                case 0x7:
                    break;
                case 0x8:
                    break;
                case 0x9:
                    break;
                case 0xa:
                    break;
                case 0xb:
                    break;
                case 0xc:
                    break;
                case 0xd:
                    break;
                case 0xe:
                    break;
                case 0xf:
                    Exit();
                    break;
            }
        }
        public void STARTVIDEO()
        {
            Console.SetWindowSize(20, 20);
            Console.SetBufferSize(20, 20);
            ConsoleFuns = new ConsoleFuns();
            ConsoleFuns.Start(20, 20);
        }
        public void Exit()
        {
            ConsoleFuns.Exit();
        }
        public void LOADWORD(byte C)
        {
            ConsoleFuns.WriteText(new Point(Console.CursorLeft, Console.CursorTop), Convert.ToChar(C).ToString(), 7);
        }
        public void SRPIXEL(byte Color)
        {
            ConsoleFuns.SetPixel(new Point(Console.CursorLeft, Console.CursorTop), Color, 0);
        }
        public void LOADWORD(char C, Point index)
        {
            ConsoleFuns.WriteText(index, Convert.ToChar(C).ToString(), 7);
        }
        public void DISPLAY()
        {
            ConsoleFuns.DisplayBuffer();
        }
        #endregion
        #region Keyboard
        public void GETKEYINPUT()
        {
            ConsoleKeyInfo Key = Console.ReadKey();
            switch (Key.Key)
            {
                case ConsoleKey.Backspace:
                    OUTINPUT = 0x26;
                    break;
                case ConsoleKey.Tab:
                    break;
                case ConsoleKey.Clear:
                    break;
                case ConsoleKey.Enter:
                    OUTINPUT = 0x0A;
                    break;
                case ConsoleKey.Pause:
                    break;
                case ConsoleKey.Escape:
                    OUTINPUT = 0x1B;
                    break;
                case ConsoleKey.Spacebar:
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
                    OUTINPUT = 0x25;
                    break;
                case ConsoleKey.UpArrow:
                    OUTINPUT = 0x26;
                    break;
                case ConsoleKey.RightArrow:
                    OUTINPUT = 0x27;
                    break;
                case ConsoleKey.DownArrow:
                    OUTINPUT = 0x28;
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
                    OUTINPUT = 0x2E;
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
                OUTINPUT = (byte)Key.Key;
                    break;
            }
        }
        public void LoadKEY(byte Input,MEM MEM)
        {
            switch (Input)
            {
                case 0x0:
                    break;
                case 0x1:
                    break;
                case 0x2:
                    if (MEM.RAM[0xFE05] == 1)
                        GETKEYINPUT();
                    break;
                case 0x3:
                    break;
                case 0x4:
                    break;
                case 0x5:
                    OutputReg = OUTINPUT;
                    break;
                case 0x6:
                    break;
                case 0x7:
                    break;
                case 0x8:
                    break;
                case 0x9:
                    break;
                case 0xa:
                    break;
                case 0xb:
                    break;
                case 0xc:
                    break;
                case 0xd:
                    break;
                case 0xe:
                    break;
                case 0xf:
                    break;
            }
        }
        #endregion
        #region Sound
        public void PlaySound()
        {
            SoundEmu.UpdateVoices();
        }
        public void LoadSOUND(byte Input)
        {
            switch (Input)
            {
                case 0x0:
                    SoundEmu.SetVoice(InputReg);
                    break;
                case 0x1:
                    break;
                case 0x2:
                    PlaySound();
                    break;
                case 0x3:
                    break;
                case 0x4:
                    SoundEmu.Play(InputReg);
                    break;
                case 0x5:
                    break;
                case 0x6:
                    break;
                case 0x7:
                    break;
                case 0x8:
                    break;
                case 0x9:
                    break;
                case 0xa:
                    break;
                case 0xb:
                    break;
                case 0xc:
                    break;
                case 0xd:
                    break;
                case 0xe:
                    break;
                case 0xf:
                    break;
            }
        }
        #endregion
    }
}