namespace PortalMouse.Utils.Math;

public readonly struct Frac {
	public static readonly Frac Zero = new(0, 1);
	public static readonly Frac One = new(1, 1);

	public readonly int Numerator;
	public readonly int Denominator;

	public Frac(int numerator, int denominator) {
		Numerator = numerator;
		Denominator = denominator;
	}

	public int Lerp(R1I range) =>
		range.Begin + range.Size * Numerator / Denominator;

	public bool Equals(Frac other) => this == other;
	public override bool Equals(object? obj) => obj is Frac other && Equals(other);
	public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);
	public override string ToString() => $"{Numerator}/{Denominator}";

	public static bool operator ==(Frac lhs, Frac rhs) => lhs.Numerator * rhs.Denominator == rhs.Numerator * lhs.Denominator;
	public static bool operator !=(Frac lhs, Frac rhs) => !(lhs == rhs);

	public static int operator *(int lhs, Frac rhs) => lhs * rhs.Numerator / rhs.Denominator;

	public static explicit operator float(Frac self) => (float)self.Numerator / self.Denominator;
}
