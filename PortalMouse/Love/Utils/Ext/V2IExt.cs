using Love;
using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Love.Utils.Ext;

public static class V2IExt {
	public static Vector2 ToLove(this V2I self) => new(self.x, self.y);
}