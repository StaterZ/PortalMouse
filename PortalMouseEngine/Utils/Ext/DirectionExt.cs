using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Utils.Ext;

public static class DirectionExt {
	public static Axis ToAxis(this Direction self) => self switch {
		Direction.Left => Axis.Horizontal,
		Direction.Right => Axis.Horizontal,
		Direction.Up => Axis.Vertical,
		Direction.Down => Axis.Vertical,
		_ => throw new UnreachableException()
	};
}
