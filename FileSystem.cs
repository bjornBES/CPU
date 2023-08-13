using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CPUTing
{
    public class FileSystem
    {
        string[] NeededFileName =
        {
            "Main.BZ",
            "MarcoCommands.txt",
            "CPUSettings.info"
        };
        private string LocalApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public string BMasmpath;
        public string BZpath;
        public string MarcoCommands;
        public string InfoPath;
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
                InfoPath = LocalApp + "/CPU";
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
                if (File.Exists(InfoPath + "/" + NeededFileName[2]) == false)
                {
                    string Text = 
                        "CPU Info\r\n" +
                        "UseCompiler:false\r\n" +
                        "UseDeveloper:false";
                    FileStream file = File.Create(InfoPath + "/" + NeededFileName[2], 1000);
                    for (int i = 0; i < Text.Length; i++)
                    {
                        char value = Text[i];
                        file.WriteByte(Convert.ToByte(value));
                    }
                    file.Close();
                }
            }
            GetDirectory();
            GetFiles();
        }
        public string GetMarcoPath()
        {
            return MarcoCommands + "/" + NeededFileName[1];
        }
        public string GetInfoText()
        {
            return File.ReadAllText(InfoPath + "/" + NeededFileName[2]);
        }
        public void GetDirectory()
        {
            if (Directory.Exists(BZpath))
            {
                BZCode = new DirectoryInfo(BZpath);
            }
            else
            {
                BZCode = Directory.CreateDirectory(BZpath);
            }
        }
        public void ShowFiles(FileInfo[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Extension == ".BZ")
                {
                    Console.SetCursorPosition(1, 2 + i);
                    Console.WriteLine(files[i].Name);
                }
            }
        }
        public FileInfo[] GetAllFiles()
        {
            GetFiles();
            return BZCode.GetFiles();
        }
        public void GetFiles()
        {
            GetDirectory();
            int FileIndex = 0;
            List<FileInfo> TempFileInfo = new List<FileInfo>();
            for (int i = 0; i < BZCode.GetFiles().Length; i++)
            {
                if (BZCode.GetFiles()[i].Extension == ".BZ")
                {
                    TempFileInfo.Add(BZCode.GetFiles()[i]);
                    FileIndex++;
                }
            }
            files = TempFileInfo.ToArray();
        }
    }
}
