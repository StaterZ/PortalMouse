using PortalMouse.Engine.Core;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Utils.Ext;

public static class SideExt {
	public static Side Opposite(this Side self) => self switch {
		Side.Left => Side.Right,
		Side.Right => Side.Left,
		Side.Top => Side.Bottom,
		Side.Bottom => Side.Top,
		_ => throw new UnreachableException()
	};

	public static Direction ToDirection(this Side self) => self switch {
		Side.Left => Direction.Left,
		Side.Right => Direction.Right,
		Side.Top => Direction.Up,
		Side.Bottom => Direction.Down,
		_ => throw new UnreachableException()
	};

	public static V2I ToVec(this Side self) => self switch {
		Side.Left => new V2I(0, 0),
		Side.Right => new V2I(1, 0),
		Side.Top => new V2I(0, 0),
		Side.Bottom => new V2I(0, 1),
		_ => throw new UnreachableException()
	};
}
