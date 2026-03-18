using System;

namespace PortalMouse.Engine.Utils.Math;
using Math = System.Math;

public struct R1F {
	public static readonly R1F Zero = new(0, 0);
	public static readonly R1F One = new(0, 1);

	public float Begin;
	public float End;

	public readonly float Size => End - Begin;

	public R1F(float begin, float end) {
		Begin = begin;
		End = end;
	}
	
	public readonly float Clamp(float value) => Math.Clamp(value, Begin, End);

	public readonly bool Contains(float point) => Begin <= point && point < End;

	public static R1F InitBeginSize(float begin, float size) => new(begin, begin + size);

	public readonly override string ToString() => $"[X:{Begin},W:{Size}]";
	public readonly bool Equals(R1F other) => Begin.Equals(other.Begin) && End.Equals(other.End);
	public readonly override bool Equals(object? obj) => obj is R1F other && Equals(other);
	public readonly override int GetHashCode() => HashCode.Combine(Begin, End);
	
	public static R1F operator +(R1F lhs, float rhs) => new(lhs.Begin + rhs, lhs.End + rhs);
	public static R1F operator -(R1F lhs, float rhs) => new(lhs.Begin - rhs, lhs.End - rhs);
	
	public static implicit operator R1F(R1I self) => new(self.Begin, self.End);
	public static explicit operator R1I(R1F self) => new((int)self.Begin, (int)self.End);
	public static explicit operator R1F(R1Frac self) => new((float)self.Begin, (float)self.End);
	public static explicit operator R1Frac(R1F self) => new((Frac)self.Begin, (Frac)self.End);
}
