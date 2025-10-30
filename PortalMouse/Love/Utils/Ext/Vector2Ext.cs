using Love;

namespace PortalMouse.Love.Utils.Ext;

public static class Vector2Ext {
	public static Vector2 Abs(this Vector2 self) => new(Mathf.Abs(self.X), Mathf.Abs(self.Y));
	public static Vector2 Floor(this Vector2 self) => new(Mathf.Floor(self.X), Mathf.Floor(self.Y));
}
