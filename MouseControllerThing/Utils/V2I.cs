using System.Drawing;

namespace MouseControllerThing.Utils;

public struct V2I {
	public int x;
	public int y;

	public V2I(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public V2I(Point point) : this(point.X, point.Y) { }

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
	public static V2I operator /(V2I lhs, V2I rhs) => new(lhs.x / rhs.x, lhs.y / rhs.y);
}
