namespace MouseControllerThing.Utils.Maths;

public struct V2F {
	public float x;
	public float y;

	public V2F(float x, float y) {
		this.x = x;
		this.y = y;
	}

	public static V2F Zero => new(0, 0);
	public static V2F One => new(1, 1);

	public V2F(Point point) : this(point.X, point.Y) { }

	public float Dot(V2F other) => x * other.x + y * other.y;
	public float MagSqr => Dot(this);
	public float Mag => MathF.Sqrt(MagSqr);
	public V2F Norm => this == Zero ? Zero : this / Mag;

	public V2F EnsureMag(float otherMag) => Norm * MathF.Max(Mag, otherMag);

	public override string ToString() => $"[{x},{y}]";
	public bool Equals(V2F other) => x == other.x && y == other.y;
	public override bool Equals(object? obj) => obj is V2F other && Equals(other);
	public override int GetHashCode() => HashCode.Combine(x, y);

	public static explicit operator V2F(V2I self) => new(self.x, self.y);
	public static bool operator ==(V2F lhs, V2F rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
	public static bool operator !=(V2F lhs, V2F rhs) => lhs.x != rhs.x || lhs.y != rhs.y;
	public static V2F operator +(V2F lhs, V2F rhs) => new(lhs.x + rhs.x, lhs.y + rhs.y);
	public static V2F operator +(V2F lhs, float rhs) => new(lhs.x + rhs, lhs.y + rhs);
	public static V2F operator -(V2F lhs, V2F rhs) => new(lhs.x - rhs.x, lhs.y - rhs.y);
	public static V2F operator -(V2F lhs, float rhs) => new(lhs.x - rhs, lhs.y - rhs);
	public static V2F operator *(V2F lhs, V2F rhs) => new(lhs.x * rhs.x, lhs.y * rhs.y);
	public static V2F operator *(V2F lhs, float rhs) => new(lhs.x * rhs, lhs.y * rhs);
	public static V2F operator /(V2F lhs, V2F rhs) => new(lhs.x / rhs.x, lhs.y / rhs.y);
	public static V2F operator /(V2F lhs, float rhs) => new(lhs.x / rhs, lhs.y / rhs);
}
