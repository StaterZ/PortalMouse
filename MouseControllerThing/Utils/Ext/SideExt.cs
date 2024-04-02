using MouseControllerThing.Core;
using MouseControllerThing.Utils.Maths;

namespace MouseControllerThing.Utils.Ext;

public static class SideExt {
	public static Direction ToDirection(this Side self) => self switch {
		Side.Left => Direction.Left,
		Side.Right => Direction.Right,
		Side.Top => Direction.Up,
		Side.Bottom => Direction.Down,
		_ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
	};
}
