using Love;
using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Love.Utils.Ext;

public static class V2IExt {
	extension(V2I self) {
		public Vector2 ToLoveVector() => new(self.x, self.y);
		public Point ToLovePoint() => new(self.x, self.y);
		public Size ToLoveSize() => new(self.x, self.y);
	}
}