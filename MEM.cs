namespace CPUTing
{
    public struct MEM
    {
        public byte[] RAM;
        public string[] ROM;
        public void START()
        {
            int MAX = ushort.MaxValue + 1; // we are doing 0xFFFF + 1 to get 0xFFFF in the mem
            RAM = new byte[MAX];
            ROM = new string[MAX];
            for (int i = 0; i < MAX; i++)
            {
                RAM[i] = 0;
                ROM[i] = "";
            }
        }
    }
}