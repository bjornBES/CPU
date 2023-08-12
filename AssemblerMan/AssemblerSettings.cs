using System;
using System.Collections.Generic;
using System.Text;

namespace CPUTing.AssemblerMan
{
    public static class AssemblerSettings
    {
        public static void InterpretString(string Line)
        {
            if(Line.Contains("-F"))
            {
                string[] ASSOPS = Line.Split(' ', 2)[1].Split(' ');
                for (int i = 0; i < ASSOPS.Length; i++)
                {
                    if (ASSOPS[i].Contains("-UseR"))
                    {
                        AssemblerOps.UseRegs = true;
                    }
                    if (ASSOPS[i].Contains("-UseDCommands"))
                    {
                        AssemblerOps.UseDots = true;
                    }
                    if (ASSOPS[i].Contains("-UseCharSrtings"))
                    {
                        AssemblerOps.UseChars = true;
                    }
                    if (ASSOPS[i].Contains("-UseStrings"))
                    {
                        AssemblerOps.UseStrings = true;
                    }
                    if (ASSOPS[i].Contains("-UseKeyTags"))
                    {
                        AssemblerOps.UseKeyTags = true;
                    }
                    if (ASSOPS[i].Contains("-UseLabelTags"))
                    {
                        AssemblerOps.UseLabels = true;
                    }
                    if (ASSOPS[i].Contains("-UseVarTags"))
                    {
                        AssemblerOps.UseVars = true;
                    }
                }
            }
            if(Line.Contains("-A"))
            {
                AssemblerOps.UseRegs = true;
                AssemblerOps.UseDots = true;
                AssemblerOps.UseChars = true;
                AssemblerOps.UseStrings = true;
                AssemblerOps.UseKeyTags = true;
                AssemblerOps.UseLabels = true;
                AssemblerOps.UseVars = true;
            }
        }
    }
}
