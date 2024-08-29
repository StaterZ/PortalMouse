using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Utils.Ext;

public static class AxisExt {
	public static Axis Opposite(this Axis self) => self switch {
		Axis.Horizontal => Axis.Vertical,
		Axis.Vertical => Axis.Horizontal,
		_ => throw new UnreachableException()
	};
}
