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

	public static V2I ToVec(this Direction self) => self switch {
		Direction.Left => new V2I(-1, 0),
		Direction.Right => new V2I(1, 0),
		Direction.Up => new V2I(0, -1),
		Direction.Down => new V2I(0, 1),
		_ => throw new UnreachableException()
	};
}
