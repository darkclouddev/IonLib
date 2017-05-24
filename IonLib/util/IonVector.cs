namespace IonLib.util
{
	public struct IonVector
	{
		public int x, y, z;

		public static readonly IonVector Zero = new IonVector(0, 0, 0);
		public static readonly IonVector One = new IonVector(1, 1, 1);
		public static readonly IonVector Forward = new IonVector(0, 0, 1);
		public static readonly IonVector Back = new IonVector(0, 0, -1);
		public static readonly IonVector Up = new IonVector(0, 1, 0);
		public static readonly IonVector Down = new IonVector(0, -1, 0);
		public static readonly IonVector Left = new IonVector(-1, 0, 0);
		public static readonly IonVector Right = new IonVector(1, 0, 0);

		static readonly string ToStringFormat = nameof(IonVector) + "({0} {1} {2})";

		public IonVector(int x, int y, int z = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public int DistanceSquared(IonVector v)
		{
			return DistanceSquared(this, v);
		}

		public static int DistanceSquared(IonVector a, IonVector b)
		{
			int dx = b.x - a.x;
			int dy = b.y - a.y;
			int dz = b.z - a.z;

			return dx * dx + dy * dy + dz * dz;
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
		}

		public override bool Equals(object other)
		{
			if (!(other is IonVector)) return false;

			IonVector vector = (IonVector)other;
			return x == vector.x && y == vector.y && z == vector.z;
		}

		public override string ToString()
		{
			return string.Format(ToStringFormat, x, y, z);
		}

		public static bool operator ==(IonVector a, IonVector b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(IonVector a, IonVector b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z;
		}

		public static IonVector operator -(IonVector a, IonVector b)
		{
			return new IonVector(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static IonVector operator +(IonVector a, IonVector b)
		{
			return new IonVector(a.x + b.x, a.y + b.y, a.z + b.z);
		}
	}
}