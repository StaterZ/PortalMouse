using System.Diagnostics;
using System.Numerics;

namespace PortalMouse.Engine.Utils.Math;
using Math = System.Math;

public readonly struct Frac : IComparable, IComparable<Frac>, IEquatable<Frac> {
	public static readonly Frac Zero = new(0, 1);
	public static readonly Frac One = new(1, 1);

	public readonly int Numerator;
	public readonly int Denominator;

	public Frac(int numerator, int denominator) {
		Numerator = numerator;
		Denominator = denominator;
	}

	public Frac Lerp(R1Frac range) =>
		range.Begin + range.Size * this;

	public Frac Lerp(LineSeg1Frac line) =>
		line.Begin + line.Delta * this;

	// Adapted to C# from: https://en.wikipedia.org/wiki/Binary_GCD_algorithm
	private static uint GCD(uint u, uint v) {
		// Base cases
		if (u == 0) return v;
		if (v == 0) return u;

		// Count the number of trailing zeros in both u and v
		int i = BitOperations.TrailingZeroCount(u);
		int j = BitOperations.TrailingZeroCount(v);
		int k = Math.Min(i, j);

		// Remove trailing zeros from u and v
		u >>= i;
		v >>= j;

		while (true) {
			// u and v are odd at the start of the loop
			Debug.Assert(u % 2 == 1, "u should be odd");
			Debug.Assert(v % 2 == 1, "v should be odd");

			// Swap if necessary so u ≤ v
			if (u > v) {
				(u, v) = (v, u);
			}

			// Identity 4: gcd(u, v) = gcd(u, v-u) as u ≤ v and u, v are both odd 
			v -= u;
			// v is now even

			if (v == 0) {
				// Identity 1: gcd(u, 0) = u
				// The shift by k is necessary to add back the 2ᵏ factor that was removed before the loop
				return u << k;
			}

			// Identity 3: gcd(u, 2ʲ v) = gcd(u, v) as u is odd
			v >>= BitOperations.TrailingZeroCount(v);
		}
	}

	Frac Simplify() {
		uint gcd = GCD((uint)Math.Abs(Numerator), (uint)Math.Abs(Denominator));
		if (gcd is 0 or 1) return this;
		return new((int)(Numerator / gcd), (int)(Denominator / gcd));
	}

	public bool Equals(Frac other) => this == other;
	public override bool Equals(object? obj) => obj is Frac other && Equals(other);
	public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);
	public override string ToString() => $"{Numerator}/{Denominator} ({(float)this})";

	public int CompareTo(Frac other) => (Numerator * other.Denominator).CompareTo(other.Numerator * Denominator);
	public int CompareTo(object? obj) => obj is Frac other ? CompareTo(other) : 0;

	public static bool operator ==(Frac lhs, Frac rhs) => lhs.Numerator * rhs.Denominator == rhs.Numerator * lhs.Denominator;
	public static bool operator !=(Frac lhs, Frac rhs) => !(lhs == rhs);
	public static bool operator <(Frac lhs, Frac rhs) => lhs.Numerator * rhs.Denominator < rhs.Numerator * lhs.Denominator;
	public static bool operator >(Frac lhs, Frac rhs) => lhs.Numerator * rhs.Denominator > rhs.Numerator * lhs.Denominator;
	public static bool operator <=(Frac lhs, Frac rhs) => lhs.Numerator * rhs.Denominator <= rhs.Numerator * lhs.Denominator;
	public static bool operator >=(Frac lhs, Frac rhs) => lhs.Numerator * rhs.Denominator >= rhs.Numerator * lhs.Denominator;

	public static Frac operator +(Frac lhs, Frac rhs) => new Frac(lhs.Numerator * rhs.Denominator + rhs.Numerator * lhs.Denominator, lhs.Denominator * rhs.Denominator).Simplify();
	public static Frac operator -(Frac lhs, Frac rhs) => new Frac(lhs.Numerator * rhs.Denominator - rhs.Numerator * lhs.Denominator, lhs.Denominator * rhs.Denominator).Simplify();
	public static Frac operator *(Frac lhs, Frac rhs) => new Frac(lhs.Numerator * rhs.Numerator, lhs.Denominator * rhs.Denominator).Simplify();
	public static Frac operator /(Frac lhs, Frac rhs) {
		Debug.Assert(rhs != Zero);
		return new Frac(lhs.Numerator * rhs.Denominator, lhs.Denominator * rhs.Numerator).Simplify();
	}

	public static implicit operator Frac(int self) => new(self, 1);
	public static explicit operator int(Frac self) => self.Numerator / self.Denominator;
	public static explicit operator float(Frac self) => (float)self.Numerator / self.Denominator;
}
