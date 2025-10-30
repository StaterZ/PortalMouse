using Love;

namespace PortalMouse.Love.Utils.Ext;

public static class RectangleFExt {
	public static Vector2 Lerp(this RectangleF self, Vector2 pivot) =>
		self.Location + (Vector2)self.Size * pivot;
}
