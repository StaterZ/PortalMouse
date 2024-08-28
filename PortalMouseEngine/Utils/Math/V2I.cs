namespace PortalMouse.Engine.Utils.Math;

public struct V2I {
	public int x;
	public int y;

	public V2I(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public readonly float Dot(V2I other) => x * other.x + y * other.y;
	public readonly float MagSqr => Dot(this);
	public readonly V2I Transpose() => new(y, x);

	public readonly V2I ToUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new ArgumentOutOfRangeException(nameof(axis)),
	};

	public readonly V2I FromUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new ArgumentOutOfRangeException(nameof(axis)),
	};

	public override readonly string ToString() => $"[{x},{y}]";
	public readonly bool Equals(V2I other) => x == other.x && y == other.y;
	public override readonly bool Equals(object? obj) => obj is V2I other && Equals(other);
	public override readonly int GetHashCode() => HashCode.Combine(x, y);

	public int this[Axis axis] {
		readonly get => axis switch {
			Axis.Horizontal => x,
			Axis.Vertical => y,
			_ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null),
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
					throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
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

	public static V2I operator *(V2I lhs, Frac rhs) => new(lhs.x * rhs, lhs.y * rhs);
	public static V2I operator /(V2I lhs, Frac rhs) => new(lhs.x / rhs, lhs.y / rhs);
}
