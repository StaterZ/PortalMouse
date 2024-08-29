using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Utils.Math;

public struct V2Frac {
	public Frac x;
	public Frac y;

	public V2Frac(Frac x, Frac y) {
		this.x = x;
		this.y = y;
	}

	public readonly Frac Dot(V2Frac other) => x * other.x + y * other.y;
	public readonly Frac MagSqr => Dot(this);
	public readonly V2Frac Transpose() => new(y, x);

	public readonly V2Frac ToUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new UnreachableException(),
	};

	public readonly V2Frac FromUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new UnreachableException(),
	};

	public override readonly string ToString() => $"[{x},{y}]";
	public readonly bool Equals(V2Frac other) => x == other.x && y == other.y;
	public override readonly bool Equals(object? obj) => obj is V2Frac other && Equals(other);
	public override readonly int GetHashCode() => HashCode.Combine(x, y);

	public Frac this[Axis axis] {
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

	public static bool operator ==(V2Frac lhs, V2Frac rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
	public static bool operator !=(V2Frac lhs, V2Frac rhs) => lhs.x != rhs.x || lhs.y != rhs.y;
	public static V2Frac operator +(V2Frac lhs, V2Frac rhs) => new(lhs.x + rhs.x, lhs.y + rhs.y);
	public static V2Frac operator +(V2Frac lhs, Frac rhs) => new(lhs.x + rhs, lhs.y + rhs);
	public static V2Frac operator -(V2Frac lhs, V2Frac rhs) => new(lhs.x - rhs.x, lhs.y - rhs.y);
	public static V2Frac operator -(V2Frac lhs, Frac rhs) => new(lhs.x - rhs, lhs.y - rhs);
	public static V2Frac operator *(V2Frac lhs, V2Frac rhs) => new(lhs.x * rhs.x, lhs.y * rhs.y);
	public static V2Frac operator *(V2Frac lhs, Frac rhs) => new(lhs.x * rhs, lhs.y * rhs);
	public static V2Frac operator /(V2Frac lhs, V2Frac rhs) => new(lhs.x / rhs.x, lhs.y / rhs.y);
	public static V2Frac operator /(V2Frac lhs, Frac rhs) => new(lhs.x / rhs, lhs.y / rhs);

	public static implicit operator V2Frac(V2I self) => new(self.x, self.y);
	public static explicit operator V2I(V2Frac self) => new((int)self.x, (int)self.y);
}
