namespace MouseControllerThing.Utils.Maths;

public struct V2I {
	public int x;
	public int y;

	public V2I(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static V2I Zero => new(0, 0);
	public static V2I One => new(1, 1);

	public float Dot(V2I other) => x * other.x + y * other.y;
	public float MagSqr => Dot(this);
	public float Mag => MathF.Sqrt(MagSqr);
	public V2F Norm => this == Zero ? V2F.Zero : (V2F)this / Mag;
	public float DistSqr(V2I other) => (other - this).MagSqr;
	public float Dist(V2I other) => (other - this).Mag;
	public V2I Transpose() => new(y, x);

	public V2I EnsureMag(float otherMag) => Ceil(Norm * MathF.Max(Mag, otherMag));
	public static V2I Round(V2F other) => new((int)MathF.Round(other.x), (int)MathF.Round(other.y));
	public static V2I Ceil(V2F other) => new((int)MathF.Ceiling(other.x), (int)MathF.Ceiling(other.y));

	public override string ToString() => $"[{x},{y}]";
	public bool Equals(V2I other) => x == other.x && y == other.y;
	public override bool Equals(object? obj) => obj is V2I other && Equals(other);
	public override int GetHashCode() => HashCode.Combine(x, y);

	public int this[Axis axis] {
		get => axis switch {
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

	public static explicit operator V2I(Point point) => new(point.X, point.Y);
	public static explicit operator Point(V2I point) => new(point.x, point.y);
}
