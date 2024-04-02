using MouseControllerThing.Utils.Maths;

namespace MouseControllerThing.Utils.Ext;

public static class AxisExt {
	public static Axis Opposite(this Axis self) => self switch {
		Axis.Horizontal => Axis.Vertical,
		Axis.Vertical => Axis.Horizontal,
		_ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
	};
}
