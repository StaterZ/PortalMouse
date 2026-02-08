using System;

namespace PortalMouse.Engine.Utils.Math;
using Math = System.Math;

public struct R1I {
	public static readonly R1I Zero = new(0, 0);
	public static readonly R1I One = new(0, 1);

	public int Begin;
	public int End;

	public readonly int Size => End - Begin;

	public R1I(int begin, int end) {
		Begin = begin;
		End = end;
	}

	public readonly int Clamp(int value) => Math.Clamp(value, Begin, End);

	public readonly bool Contains(int point) => Begin <= point && point < End;

	public static R1I InitBeginSize(int begin, int size) => new(begin, begin + size);

	public readonly override string ToString() => $"[X:{Begin},W:{Size}]";
	public readonly bool Equals(R1I other) => Begin == other.Begin && End == other.End;
	public readonly override bool Equals(object? obj) => obj is R1I other && Equals(other);
	public readonly override int GetHashCode() => HashCode.Combine(Begin, End);
	
	public static R1I operator +(R1I lhs, int rhs) => new(lhs.Begin + rhs, lhs.End + rhs);
	public static R1I operator -(R1I lhs, int rhs) => new(lhs.Begin - rhs, lhs.End - rhs);
}
