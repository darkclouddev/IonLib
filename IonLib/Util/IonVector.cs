namespace IonLib.Util
{
	public struct IonVector
	{
		public int X, Y, Z;

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
			X = x;
			Y = y;
			Z = z;
		}

		public int DistanceSquared(IonVector v) => DistanceSquared(this, v);

		public static int DistanceSquared(IonVector a, IonVector b)
		{
			int dx = b.X - a.X;
			int dy = b.Y - a.Y;
			int dz = b.Z - a.Z;

			return dx * dx + dy * dy + dz * dz;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2;
		}

		public override bool Equals(object other)
		{
			if (other is IonVector vector)
				return X == vector.X && Y == vector.Y && Z == vector.Z;

			return false;
		}

		public override string ToString()
		{
			return string.Format(ToStringFormat, X, Y, Z);
		}

		public static bool operator ==(IonVector a, IonVector b)
		{
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}

		public static bool operator !=(IonVector a, IonVector b)
		{
			return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
		}

		public static IonVector operator -(IonVector a, IonVector b)
		{
			return new IonVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static IonVector operator +(IonVector a, IonVector b)
		{
			return new IonVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
	}
}