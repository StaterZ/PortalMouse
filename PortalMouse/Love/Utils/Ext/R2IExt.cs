using Love;
using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Love.Utils.Ext;

public static class R2IExt {
	public static Rectangle ToLove(this R2I self) => new(self.Pos.ToLovePoint(), self.Size.ToLoveSize());
}