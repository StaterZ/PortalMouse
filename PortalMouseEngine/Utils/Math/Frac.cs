using System;
using System.Diagnostics;
using System.Numerics;

namespace PortalMouse.Engine.Utils.Math;
using Math = System.Math;

public struct Frac : IComparable, IComparable<Frac>, IEquatable<Frac> {
	public static readonly Frac Zero = 0;
	public static readonly Frac One = 1;
	public static readonly Frac Half = new(1, 2);

	public int Numerator;
	public int Denominator;

	public Frac(int numerator, int denominator) {
		Numerator = numerator;
		Denominator = denominator;
	}

	public readonly Frac InvLerp(R1Frac range) => (this - range.Begin) / range.Size;
	public readonly Frac InvLerp(LineSeg1Frac line) => (this - line.Begin) / line.Delta;
	public readonly Frac Lerp(R1Frac range) => range.Begin + range.Size * this;
	public readonly Frac Lerp(LineSeg1Frac line) => line.Begin + line.Delta * this;
	public readonly Frac Map(R1Frac from, R1Frac to) => InvLerp(from).Lerp(to);

	// Adapted to C# from: https://en.wikipedia.org/wiki/Binary_GCD_algorithm
	private static uint GCD(uint u, uint v) {
		// Base cases: gcd(n, 0) = gcd(0, n) = n
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

	private Frac Simplify() {
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

	public static Frac operator -(Frac self) => new(-self.Numerator, self.Denominator);
	
	public static implicit operator Frac(int self) => new(self, 1);
	public static explicit operator int(Frac self) {
		int bias = Math.Sign(self.Numerator) * Math.Abs(self.Denominator) / 2;
		return (self.Numerator + bias) / self.Denominator;
	}

	public static explicit operator Frac(float self) {
		if (float.IsNaN(self)) throw new ArgumentException("NaN cannot be represented as a fraction.");
		if (float.IsInfinity(self))  throw new ArgumentException("Infinity cannot be represented as a fraction.");
		if (self == 0f) return Zero;

		//extract IEEE 754 components
		int bits     = BitConverter.SingleToInt32Bits(self);
		int sign     = (bits >> 31) == 0 ? 1 : -1;
		int exponent = ((bits >> 23) & 0xFF) - 127;   //biased exponent -> actual exponent
		int mantissa = exponent == -127               //subnormal?
			? (bits & 0x7FFFFF) << 1
			: (bits & 0x7FFFFF) | 0x800000; //restore implicit leading 1

		//self = sign * mantissa * 2^(exponent - 23)
		Frac frac = new(sign * mantissa, 1);
		int shift = exponent - 23;
		if (shift >= 0) {
			frac.Numerator <<= shift;
		} else {
			frac.Denominator <<= -shift;
		}
		return frac;
	}
	public static explicit operator float(Frac self) => (float)self.Numerator / self.Denominator;
}
