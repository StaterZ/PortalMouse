using Love;

namespace PortalMouse.Love.Utils.Ext;

public static class PointExt {
	public static Point Negate(this Point self) => new(-self.X, -self.Y);
	public static Point Min(Point a, Point b) => new(Mathf.Min(a.X, b.X), Mathf.Min(a.Y, b.Y));
	public static Point Max(Point a, Point b) => new(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y));
}
