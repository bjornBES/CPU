namespace CPUTing
{
    public enum INSTR
    {
        ANDI = 0x25,
        ADDR = 0x22,
        ANDR = 0x29,
        ADDI = 0x20,

        CALL = 0x02,
        CMPA = 0x13,
        CMPB = 0x17,
        CMPC = 0x1B,
        CMPD = 0x1F,
        CMPF = 0x24,
        
        DECR = 0x11,
        
        GROM = 0x3A,
        
        HLTC = 0x00,
        
        INBY = 0x04,
        INTB = 0x2E,
        INTR = 0x33,
        INCR = 0x12,

        JUMP = 0x2F,
        JINZ = 0x30,
        JINT = 0x31,
        JIFT = 0x32,
        JIFP = 0x38,
        JINP = 0x39,
        
        LODA = 0x0E,
        LOAA = 0x0F,
        LODC = 0x18,
        LOAC = 0x19,
        LODB = 0x14,
        LOAB = 0x15,
        LODD = 0x1C,
        LOAD = 0x1D,
        
        MOVA = 0x06,
        MOVB = 0x07,
        MOVC = 0x08,
        MOBA = 0x2D,
        NORI = 0x28,
        
        NOTR = 0x2A,
        NOOP = 0x01,
        NORR = 0x2C,
        NOTI = 0x26,
        
        OUTB = 0x05,
        ORIM = 0x27,
        ORRE = 0x2B,
        
        PUHI = 0x0A,
        PUHR = 0x0B,
        PUHS = 0x0C,
        POPR = 0x0D,
        
        RESR = 0x03,
        REIN = 0x34,
        ROLR = 0x36,
        RORR = 0x37,
        
        STOI = 0x09,
        STOA = 0x10,
        STOB = 0x16,
        STOC = 0x1A,
        STOD = 0x1E,
        SUBI = 0x21,
        SUBR = 0x23,
        
        UINT = 0x35,
        
    }       
    public enum Reg
    {
        A = 0x00,
        B = 0x01,
        C = 0x02,
        D = 0x03,

    }
    public enum LOGICOP
    {
        ADD,
        NOT,
        OR,
        NOR
    }
}