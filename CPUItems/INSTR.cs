﻿namespace CPUTing.CPUItems
{
    public enum INSTR
    {
        HLTC = 0x00,
        NOOP = 0x01,
        CALL = 0x02,
        RESR = 0x03,
        INBY = 0x04,
        OUTB = 0x05,
        MOVA = 0x06,
        MOVB = 0x07,
        MOVC = 0x08,
        STOI = 0x09,
        PUHI = 0x0A,
        PUHR = 0x0B,
        PUHS = 0x0C,
        POPR = 0x0D,
        LODA = 0x0E,
        LOAA = 0x0F,
        STOA = 0x10,
        DECR = 0x11,
        INCR = 0x12,
        CMPA = 0x13,
        LODB = 0x14,
        LOAB = 0x15,
        STOB = 0x16,
        CMPB = 0x17,
        LODC = 0x18,
        LOAC = 0x19,
        STOC = 0x1A,
        CMPC = 0x1B,
        LODD = 0x1C,
        LOAD = 0x1D,
        STOD = 0x1E,
        CMPD = 0x1F,
        ADDI = 0x20,
        SUBI = 0x21,
        ADDR = 0x22,
        SUBR = 0x23,
        CMPF = 0x24,
        ANDI = 0x25,
        NOTI = 0x26,
        ORIM = 0x27,
        NORI = 0x28,
        ANDR = 0x29,
        NOTR = 0x2A,
        ORRE = 0x2B,
        NORR = 0x2C,
        MOBA = 0x2D,
        INTA = 0x2E,
        JUMP = 0x2F,
        JINZ = 0x30,
        JINT = 0x31,
        JIFT = 0x32,
        INTR = 0x33,
        REIN = 0x34,
        UINT = 0x35,
        ROLR = 0x36,
        RORR = 0x37,
        JIFC = 0x38,
        JINO = 0x39,
        GROM = 0x3A,
        LSLR = 0x3B,
        LSRR = 0x3C,
        LODF = 0x3D,
        LOBS = 0x3E,
        LOAR = 0x3F,
        LOGC = 0x40,
    }
    public enum Reg
    {
        A = 0x00,
        B = 0x01,
        C = 0x02,
        D = 0x03,

    }
    public enum LOGICOPenum
    {
        ADD,
        NOT,
        OR,
        NOR
    }
}