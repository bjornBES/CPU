using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CPUTing
{
    public static class Info
    {
        public static bool useCompiler = false;
        public static bool useDev = false;
    }
    public class InfoDecoder
    {
        public InfoDecoder(string Text) 
        {
            string[] SplitedText = Text.Split("\r\n");
            for (int i = 1; i < SplitedText.Length; i++)
            {
                CheakInfo(SplitedText[i], "UseCompiler:");
                CheakInfo(SplitedText[i], "UseDeveloper:");
                CheakInfo(SplitedText[i], "Name:");
            }
        }
        public void CheakInfo(string SplitedText, string CheakFor)
        {
            if (SplitedText.Contains(CheakFor))
            {
                string[] value = SplitedText.Split(':');
                DecodeOne(value[1], value[0]);
            }
        }
        public void DecodeOne(string value, string Type)
        {
            if(Type == "UseCompiler")
            {
                Info.useCompiler = TrueOrFalse(value);
            }
            if (Type == "UseDeveloper")
            {
                Info.useDev = TrueOrFalse(value);
            }
            if (Type == "Name")
            {
                if (value == "BESIA")
                {
                    Console.WriteLine("What is this?");
                    Console.ForegroundColor = GetRandomColor();
                    Thread.Sleep(500);
                    Console.ForegroundColor = GetRandomColor();
                    Thread.Sleep(500);
                    Console.ForegroundColor = GetRandomColor();
                    Thread.Sleep(500);
                    Console.ForegroundColor = GetRandomColor();
                    Thread.Sleep(500);
                    Console.ForegroundColor = GetRandomColor();
                    Console.ResetColor();
                }
            }
        }
        ConsoleColor GetRandomColor()
        {
            Random random = new Random();
            int Number = random.Next(0, 15);
            object Result = Enum.Parse(typeof(ConsoleColor), Number.ToString());
            return (ConsoleColor)Result;
        }
        public bool TrueOrFalse(string value)
        {
            if (value == "true")
            {
                return true;
            }
            return false;
        }
    }
}
