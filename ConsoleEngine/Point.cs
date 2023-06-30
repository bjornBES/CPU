using System;

namespace CPUTing.ConsoleEngine
{
    public struct Point
	{
		public int X { get; set; }
		public int Y { get; set; }

		public static Point Zero { get; private set; } = new Point(0, 0);

		public Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
		public override string ToString() => String.Format("({0}, {1})", X, Y);

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Point operator +(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}
		public static Point operator -(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}
		public static Point operator +(Point a, int b)
		{
			return new Point(a.X + b, a.Y + b);
		}
		public static Point operator -(Point a, int b)
		{
			return new Point(a.X - b, a.Y - b);
		}

		public static Point operator /(Point a, float b)
		{
			return new Point((int)(a.X / b), (int)(a.Y / b));
		}
		public static Point operator *(Point a, float b)
		{
			return new Point((int)(a.X * b), (int)(a.Y * b));
		}
		public static bool operator ==(Point a, Point b)
		{
			return a.X == b.X && a.Y == b.Y;
		}
		public static bool operator !=(Point a, Point b)
		{
			return a.X != b.X || a.Y != b.Y;
		}
		public static bool operator <=(Point a, Point b)
		{
			return a.X <= b.X && a.Y <= b.Y;
		}
		public static bool operator >=(Point a, Point b)
		{
			return a.X >= b.X && a.Y >= b.Y;
		}
		public static bool operator <(Point a, Point b)
		{
			return a.X < b.X && a.Y < b.Y;
		}
		public static bool operator >(Point a, Point b)
		{
			return a.X > b.X && a.Y > b.Y;
		}
	}
}
