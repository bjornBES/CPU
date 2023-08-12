using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CPUTing
{
    public class FileSystem
    {
        string[] NeededFileName =
        {
            "Main.BZ",
            "MarcoCommands.txt"
        };
        private string LocalApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public string BMasmpath;
        public string BZpath;
        public string MarcoCommands;
        public DirectoryInfo BZCode;
        public FileInfo[] files;
        public FileSystem()
        {
            Console.WriteLine("Write the path to the programs");
            BZpath = Console.ReadLine();
            if (BZpath == "\r\n" || BZpath == "\n" || BZpath == "")
            {
                BZpath = LocalApp + "/CPU/BZCode";
                MarcoCommands = LocalApp + "/CPU/Marcos";
                if (Directory.Exists(BZpath) == false)
                {
                    Directory.CreateDirectory(BZpath);
                }
                if (Directory.Exists(MarcoCommands) == false)
                {
                    Directory.CreateDirectory(MarcoCommands);
                }

                if (File.Exists(BZpath + "/" + NeededFileName[0]) == false)
                {
                    FileStream file = File.Create(BZpath + "/" + NeededFileName[0], 1000);
                    file.Close();
                }
                if (File.Exists(MarcoCommands + "/" + NeededFileName[1]) == false)
                {
                    FileStream file = File.Create(MarcoCommands + "/" + NeededFileName[1], 1000);
                    file.Close();
                }
            }
            if (Directory.Exists(BZpath))
            {
                BZCode = new DirectoryInfo(BZpath);
            }
            else
            {
                BZCode = Directory.CreateDirectory(BZpath);
            }
            int FileIndex = 0;
            FileInfo[] TempFileInfo = BZCode.GetFiles();
            for (int i = 0; i < TempFileInfo.Length; i++)
            {
                if (TempFileInfo[i].Extension == ".BZ")
                {
                    files[FileIndex] = TempFileInfo[i];
                    FileIndex++;
                }
            }
        }
        public void GetFiles()
        {
            int FileIndex = 0;
            FileInfo[] TempFileInfo = BZCode.GetFiles();
            for (int i = 0; i < TempFileInfo.Length; i++)
            {
                if (TempFileInfo[i].Extension == ".BZ")
                {
                    files[FileIndex] = TempFileInfo[i];
                    FileIndex++;
                }
            }

        }
    }
}
