using PortalMouse.Core;
using PortalMouse.Utils.Math;

namespace PortalMouse.Utils.Ext;

public static class SideExt {
	public static Direction ToDirection(this Side self) => self switch {
		Side.Left => Direction.Left,
		Side.Right => Direction.Right,
		Side.Top => Direction.Up,
		Side.Bottom => Direction.Down,
		_ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
	};

	public static V2I ToVec(this Side self) => self switch {
		Side.Left => new V2I(0, 0),
		Side.Right => new V2I(1, 0),
		Side.Top => new V2I(0, 0),
		Side.Bottom => new V2I(0, 1),
		_ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
	};
}
