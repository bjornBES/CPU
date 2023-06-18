using System;
using System.Threading;

namespace CPUTing
{
    public class PORT
    {
        PORTMEM PORTMEM;
        public char[] ConsoleReg;
        public char OUTINPUT;
        public int INDEX = 0;
        public char KEYREG = ' ';

        public void START()
        {
            PORTMEM = new PORTMEM();
            PORTMEM.START();
            INDEX = 0;
            KEYREG = ' ';
            ConsoleReg = new char[0xFF];
            for (int i = 0; i < ConsoleReg.Length; i++)
            {
                ConsoleReg[i] = ' ';
            }
        }
        public void CLS()
        {
            INDEX = 0;
        }
        public void LOADWORD(char C)
        {
            ConsoleReg[INDEX] = C;
            INDEX++;
        }
        public void LOADWORD(char C, int index)
        {
            ConsoleReg[index] = C;
        }
        public void DISPLAY()
        {
            for (int i = 0; i < INDEX; i++)
            {
                if (ConsoleReg[i].ToString().ToUpper() == "\\" && ConsoleReg[i + 1].ToString().ToUpper() == "N")
                {
                    Console.WriteLine(" ");
                    i++;
                    i++;
                }
                else
                    Console.Write(ConsoleReg[i].ToString().ToUpper());
            }
        }
        public void GETKEYINPUT()
        {
            //get input here
            OUTINPUT = Console.ReadKey().KeyChar;
        }
    }
    public struct PORTMEM
    {
        public byte[] RAM;
        public void START()
        {
            ushort MAX = 0xFFFF;
            RAM = new byte[MAX];
            for (int i = 0; i < MAX; i++)
            {
                RAM[i] = 0;
            }
        }
    }
}
