using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace CPUTing.ConsoleEngine
{
    class ConsoleEmu
    {
		private NativeMethods.CharInfo[] CharInfoBuffer { get; set; }

        private readonly SafeFileHandle h;

		readonly int width, height;

		public ConsoleEmu(int w, int he)
		{
			width = w;
			height = he;

			h = NativeMethods.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

			if (!h.IsInvalid)
			{
				CharInfoBuffer = new NativeMethods.CharInfo[width * height];
			}
		}
		public void SetBuffer(Glyph[,] GlyphBuffer, int defualtBackground)
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int i = (y * width) + x;

					if (GlyphBuffer[x, y].c == 0)
						GlyphBuffer[x, y].bg = defualtBackground;

					CharInfoBuffer[i].Attributes = (short)(GlyphBuffer[x, y].fg | (GlyphBuffer[x, y].bg << 4));
					CharInfoBuffer[i].UnicodeChar = GlyphBuffer[x, y].c;
				}
			}
		}

		public bool Blit()
		{
			NativeMethods.SmallRect rect = new NativeMethods.SmallRect() { Left = 0, Top = 0, Right = (short)width, Bottom = (short)height };

			return NativeMethods.WriteConsoleOutputW(h, CharInfoBuffer,
				new NativeMethods.Coord() { X = (short)width, Y = (short)height },
				new NativeMethods.Coord() { X = 0, Y = 0 }, ref rect);
		}
	}
	class NativeMethods
    {
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct CharInfo
		{
			[FieldOffset(0)] public char UnicodeChar;
			[FieldOffset(0)] public byte AsciiChar;
			[FieldOffset(2)] public short Attributes;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct Coord
		{
			public short X;
			public short Y;

			public Coord(short X, short Y)
			{
				this.X = X;
				this.Y = Y;
			}
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct SmallRect
		{
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;
		}
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteConsoleOutputW(
			SafeFileHandle hConsoleOutput,
			CharInfo[] lpBuffer,
			Coord dwBufferSize,
			Coord dwBufferCoord,
		ref SmallRect lpWriteRegion);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern SafeFileHandle CreateFile(
	string fileName,
	[MarshalAs(UnmanagedType.U4)] uint fileAccess,
	[MarshalAs(UnmanagedType.U4)] uint fileShare,
	IntPtr securityAttributes,
	[MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
	[MarshalAs(UnmanagedType.U4)] int flags,
IntPtr template);
	}
	public struct Glyph
	{
		public char c;
		public int fg;
		public int bg;

		public void Set(char c_, int fg_, int bg_) { c = c_; fg = fg_; bg = bg_; }
	}
	public class ConsoleFuns
    {
		private ConsoleEmu ConsoleBuffer { get; set; }
		private Glyph[,] GlyphBuffer { get; set; }

		public void Start(int width = 20, int height = 20)
        {
			Console.SetWindowSize(width, height);
			Console.SetBufferSize(width, height);
			ConsoleBuffer = new ConsoleEmu(width, height);
			GlyphBuffer = new Glyph[width, height];
			ClearBuffer();
		}
		public void Exit()
        {
			Console.SetWindowSize(40, 20);
			Console.SetBufferSize(40, 20);
			ConsoleBuffer = null;
			GlyphBuffer = null;
			ClearBuffer();
		}
		public void ClearBuffer()
		{
			for (int y = 0; y < GlyphBuffer.GetLength(1); y++)
				for (int x = 0; x < GlyphBuffer.GetLength(0); x++)
					GlyphBuffer[x, y] = new Glyph();
		}
		public void SetPixel(Point selectedPoint, int fgColor, int bgColor, char Char = (char)0x2588)
		{
			if (selectedPoint.X >= GlyphBuffer.GetLength(0) || selectedPoint.Y >= GlyphBuffer.GetLength(1)
				|| selectedPoint.X < 0 || selectedPoint.Y < 0) return;

			GlyphBuffer[selectedPoint.X, selectedPoint.Y].Set(Char, fgColor, bgColor);
		}
		public void PixelAt(Point selectedPoint, out Glyph glyph)
		{
			if (selectedPoint.X > 0 && selectedPoint.X < GlyphBuffer.GetLength(0) && selectedPoint.Y > 0 && selectedPoint.Y < GlyphBuffer.GetLength(1))
                _ = GlyphBuffer[selectedPoint.X, selectedPoint.Y];
			glyph = new Glyph();
		}
		public void DisplayBuffer()
		{
			ConsoleBuffer.SetBuffer(GlyphBuffer, 0);
			ConsoleBuffer.Blit();
		}
		public void WriteText(Point pos, string text, int fgColor)
		{
			for (int i = 0; i < text.Length; i++)
			{
				SetPixel(new Point(pos.X + i, pos.Y), fgColor, 0, text[i]);
			}
		}
	}
}
