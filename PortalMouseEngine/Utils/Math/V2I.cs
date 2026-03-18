using System;
using PortalMouse.Engine.Utils.Ext;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Utils.Math;

public struct V2I {
	public static readonly V2I Zero = new(0, 0);
	public static readonly V2I One = new(1, 1);

	public int x;
	public int y;

	public V2I(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public V2I Clamp(R2I range) => new(
		range.X.Clamp(x),
		range.Y.Clamp(y) 
	);
	
	public readonly V2I Lerp(R2I range) => new(
		x.Lerp(range.X),
		y.Lerp(range.Y)
	);

	public readonly int Dot(V2I other) => x * other.x + y * other.y;
	public readonly int MagSqr => Dot(this);
	public readonly V2I Transpose() => new(y, x);

	public readonly V2I ToUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new UnreachableException(),
	};
	public readonly V2I FromUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new UnreachableException(),
	};

	public readonly override string ToString() => $"[{x},{y}]";
	public readonly bool Equals(V2I other) => x == other.x && y == other.y;
	public readonly override bool Equals(object? obj) => obj is V2I other && Equals(other);
	public readonly override int GetHashCode() => HashCode.Combine(x, y);

	public int this[Axis axis] {
		readonly get => axis switch {
			Axis.Horizontal => x,
			Axis.Vertical => y,
			_ => throw new UnreachableException(),
		};
		set {
			switch (axis) {
				case Axis.Horizontal:
					x = value;
					break;
				case Axis.Vertical:
					y = value;
					break;
				default:
					throw new UnreachableException();
			};
		}
	}

	public static bool operator ==(V2I lhs, V2I rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
	public static bool operator !=(V2I lhs, V2I rhs) => lhs.x != rhs.x || lhs.y != rhs.y;
	public static V2I operator +(V2I lhs, V2I rhs) => new(lhs.x + rhs.x, lhs.y + rhs.y);
	public static V2I operator +(V2I lhs, int rhs) => new(lhs.x + rhs, lhs.y + rhs);
	public static V2I operator -(V2I lhs, V2I rhs) => new(lhs.x - rhs.x, lhs.y - rhs.y);
	public static V2I operator -(V2I lhs, int rhs) => new(lhs.x - rhs, lhs.y - rhs);
	public static V2I operator *(V2I lhs, V2I rhs) => new(lhs.x * rhs.x, lhs.y * rhs.y);
	public static V2I operator *(V2I lhs, int rhs) => new(lhs.x * rhs, lhs.y * rhs);
	public static V2I operator /(V2I lhs, V2I rhs) => new(lhs.x / rhs.x, lhs.y / rhs.y);
	public static V2I operator /(V2I lhs, int rhs) => new(lhs.x / rhs, lhs.y / rhs);
	
	public static V2I operator -(V2I self) => new(-self.x, -self.y);
}
