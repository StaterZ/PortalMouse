using Love;

namespace PortalMouse.Love.Utils.Ext;

public static class Vector2Ext {
	extension(Vector2 self) {
		public Vector2 Abs() => new(Mathf.Abs(self.X), Mathf.Abs(self.Y));
		public Vector2 Floor() => new(Mathf.Floor(self.X), Mathf.Floor(self.Y));
	}
}
