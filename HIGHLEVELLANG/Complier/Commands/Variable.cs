using System;
using System.Collections.Generic;
using System.Text;
using CrypticWizard.HexConverter;

namespace CPUTing.HIGHLEVELLANG.Complier.Commands
{
    public struct VariableTemp
    {
        public string VariableName;
        public string VariableValue;
    }
    public static class Variable
    {
        public static byte VarADDRPOINTER = 0x00;
        public static VariableTemp[] variables = new VariableTemp[0xff];
        static string Code;
        public static string SetNewVariable(string value, string name)
        {
            // STOI value 10 + VarADDRPOINTER
            byte[] HEX = { 10, VarADDRPOINTER };
            string SHEX = HexConverter.GetHexString(HEX);
            variables[VarADDRPOINTER].VariableName = name;
            variables[VarADDRPOINTER].VariableValue = value;
            VarADDRPOINTER++;
            return "\tSTOI #" + value + " &" + SHEX;
        }
        public static void SetVariable(string NewValue, int index)
        {

        }
        public static ushort GetVariableAddr(string Name)
        {
            for (int i = 0; i < VarADDRPOINTER; i++)
            {
                if(variables[i].VariableName == Name)
                {
                    return (ushort)i;    
                }
            }
            return 0;
        }
        public static void SetVariable(string NewValue, string name)
        {

        }
        public static void DecVariable(string name)
        {
            for (int i = 0; i < VarADDRPOINTER; i++)
            {
                if (variables[i].VariableName == name)
                {
                    byte[] HEX = { 10, (byte)i };
                    string SHEX = HexConverter.GetHexString(HEX);
                    Code = "\tLOAD &" + SHEX.PadLeft(4, '0') + "|";
                    Code += "\tDECR #03|";
                    Code += "\tSTOD &" + SHEX.PadLeft(4, '0');
                }
            }
        }
        public static void IncVariable(string name)
        {
            for (int i = 0; i < VarADDRPOINTER; i++)
            {
                if (variables[i].VariableName == name)
                {
                    byte[] HEX = { 10, (byte)i };
                    string SHEX = HexConverter.GetHexString(HEX);
                    Code = "\tLOAD &" + SHEX.PadLeft(4, '0') + "|";
                    Code += "\tINCR #03|";
                    Code += "\tSTOD &" + SHEX.PadLeft(4, '0');
                }
            }
        }
        public static int GetLen()
        {
            return Code.Split('|').Length;
        }
        public static string GetCode(int Index)
        {
            return Code.Split('|')[Index];
        }
    }
}
