using System;

namespace PortalMouse.Engine.Utils.Math;

public struct R1Frac {
	public static readonly R1Frac Zero = new(0, 0);
	public static readonly R1Frac One = new(0, 1);

	public Frac Begin;
	public Frac End;

	public readonly Frac Size => End - Begin;

	public R1Frac(Frac begin, Frac end) {
		Begin = begin;
		End = end;
	}

	public readonly Frac Clamp(Frac value) =>
		MathX.Clamp(value, new R1Frac(Begin, MathX.Max(Begin, End - 1)));

	public readonly bool Contains(Frac point) =>
		Begin <= point && point < End;

	public static R1Frac InitBeginSize(Frac begin, Frac size) => new(begin, begin + size);

	public readonly override string ToString() => $"[X:{Begin},W:{Size}]";
	public readonly bool Equals(R1Frac other) => Begin.Equals(other.Begin) && End.Equals(other.End);
	public readonly override bool Equals(object? obj) => obj is R1Frac other && Equals(other);
	public readonly override int GetHashCode() => HashCode.Combine(Begin, End);

	public static R1Frac operator +(R1Frac lhs, Frac rhs) => new(lhs.Begin + rhs, lhs.End + rhs);
	public static R1Frac operator -(R1Frac lhs, Frac rhs) => new(lhs.Begin - rhs, lhs.End - rhs);

	public static implicit operator R1Frac(R1I self) => new(self.Begin, self.End);
	public static explicit operator R1I(R1Frac self) => new((int)self.Begin, (int)self.End);
}
