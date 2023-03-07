using System.Drawing;

namespace MouseControllerThing.Utils;

public struct V2I {
	public int x;
	public int y;

	public V2I(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static V2I Zero => new V2I(0, 0);
	public static V2I One => new V2I(1, 1);

	public V2I(Point point) : this(point.X, point.Y) { }

	public float Dot(V2I other) => x * other.x + y * other.y;
	public float MagSqr => Dot(this);
	public float Mag => MathF.Sqrt(MagSqr);
	public V2F Norm => (V2F)this / Mag;

	public V2I EnsureMag(float otherMag) => Ceil(Norm * MathF.Max(Mag, otherMag));
	public static V2I Round(V2F other) => new((int)MathF.Round(other.x), (int)MathF.Round(other.y));
	public static V2I Ceil(V2F other) => new((int)MathF.Ceiling(other.x), (int)MathF.Ceiling(other.y));

	public override string ToString() => $"[{x},{y}]";

	public bool Equals(V2I other) {
		return x == other.x && y == other.y;
	}

	public override bool Equals(object? obj) {
		return obj is V2I other && Equals(other);
	}

	public override int GetHashCode() {
		return HashCode.Combine(x, y);
	}

	public static bool operator ==(V2I lhs, V2I rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
	public static bool operator !=(V2I lhs, V2I rhs) => lhs.x != rhs.x || lhs.y != rhs.y;
	public static V2I operator +(V2I lhs, V2I rhs) => new(lhs.x + rhs.x, lhs.y + rhs.y);
	public static V2I operator -(V2I lhs, V2I rhs) => new(lhs.x - rhs.x, lhs.y - rhs.y);
	public static V2I operator *(V2I lhs, V2I rhs) => new(lhs.x * rhs.x, lhs.y * rhs.y);
	public static V2I operator *(V2I lhs, int rhs) => new(lhs.x * rhs, lhs.y * rhs);
	public static V2I operator /(V2I lhs, V2I rhs) => new(lhs.x / rhs.x, lhs.y / rhs.y);
	public static V2I operator /(V2I lhs, int rhs) => new(lhs.x / rhs, lhs.y / rhs);
}
