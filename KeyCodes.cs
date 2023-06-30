using System;
using System.Collections.Generic;
using System.Text;

namespace CPUTing
{
    public enum KeyCodes
    {
        A = 00,
        B = 01,
        C = 02,
        D = 03,
        E = 04,
        F = 05,
        G = 06,
        H = 07,
        I = 08,
        J = 09,
        K = 10,
        L = 11,
        M = 12,
        N = 13,
        O = 14,
        P = 15,
        Q = 16,
        R = 17,
        S = 18,
        T = 19,
        U = 20,
        V = 21,
        W = 22,
        X = 23,
        Y = 24,
        Z = 25,
        Enter = 26,
        Backspace = 27,
        Escape = 28,
        Tab = 29,
        Spacebar = 30,
        PageUp = 31,
        PageDown = 32,
        End = 33,
        Home = 34,
        PrintScreen = 35,
        Insert = 36,
        LeftArrow = 37,
        UpArrow = 38,
        RightArrow = 39,
        DownArrow = 40,
        D0 = 41,
        D1 = 42,
        D2 = 43,
        D3 = 44,
        D4 = 45,
        D5 = 46,
        D6 = 47,
        D7 = 48,
        D8 = 49,
        D9 = 50,
        Delete = 51,
        LeftWindows = 52,
        RightWindows = 52,
        Multiply = 53,
        Add = 54,
        Separator = 55,
        Subtract = 56,
        Decimal = 57,
        Divide = 58,
        F1 = 60,
        F2 = 62,
        F3 = 63,
        F4 = 64,
        F5 = 65,
        F6 = 66,
        F7 = 67,
        F8 = 68,
        F9 = 69,
        F10 = 70,
        F11 = 71,
        F12 = 72,
        none,
    }
    public class KEYS
    {
        public KeyCodes ConvToKeyCodes(ConsoleKey consoleKey)
        {
            ConsoleKey[] ConsoleKEYs = (ConsoleKey[])Enum.GetValues(typeof(ConsoleKey));
            string[] KEYCODES = Enum.GetNames(typeof(KeyCodes));
            for (int l = 0; l < ConsoleKEYs.Length; l++)
            {
                if (consoleKey == ConsoleKEYs[l])
                {
                    for (int i = 0; i < KEYCODES.Length; i++)
                    {
                        if (Enum.GetNames(typeof(ConsoleKey))[l] == KEYCODES[i])
                        {
                            return (KeyCodes)Enum.GetValues(typeof(KeyCodes)).GetValue(i);
                        }
                    }
                }
            }
            return KeyCodes.none;
        }
    }
}
