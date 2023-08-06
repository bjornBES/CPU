using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CPUTing
{
    public class Marco
    {
        public string Name;
        public string[] Commands;
    }
    public class MarcoMan
    {
        Marco[] Marcos = new Marco[256];
        string Path;
        int MarcoIndex;
        public MarcoMan()
        {
            for (int i = 0; i < Marcos.Length; i++)
            {
                Marcos[i] = new Marco()
                {
                    Commands = new string[255],
                    Name = ""
                };
            }
        }
        public void SetPath(string path)
        {
            Path = path;
        }
        public void SplitCommands()
        {
            int index = 0;
            string[] preCommands = new string[0xff];
            string[] SplitedMarco = File.ReadAllText(Path).Split("\r\n");
            for (int i = 0; i < SplitedMarco.Length; i++)
            {
                if (SplitedMarco[i] != "\t" && SplitedMarco[i] != "" && SplitedMarco[i] != " ")
                {
                    if (SplitedMarco[i].Contains('@'))
                    {
                        string test = SplitedMarco[i].TrimStart('@');
                        Marcos[MarcoIndex].Name = test;
                    }
                    else if (SplitedMarco[i] != "End")
                    {
                        preCommands[index] = SplitedMarco[i].TrimStart('\t');
                        index++;
                    }
                    else
                    {
                        Marcos[MarcoIndex].Commands = preCommands;
                        index = 0;
                        MarcoIndex++;
                    }
                }
            }
        }
        public string GetName(int index)
        {
            return Marcos[index].Name;
        }
        public string[] GetName(int Start, int Count)
        {
            if (Start > 256 || Count > 256 || Start < 0 || Count < 0)
                return new string[0];
            int preCount = Count;
            if (Count > MarcoIndex)
                preCount = MarcoIndex;
            string[] Names = new string[preCount];
            for (int i = Start; i < preCount + Start; i++)
            {
                Names[i - Start] = Marcos[i].Name;
            }
            return Names;
        }
        public string[] GetCommands(int Index)
        {
            if (Index > 256)
                return new string[0];
            return Marcos[Index].Commands;
        }
    }
}
