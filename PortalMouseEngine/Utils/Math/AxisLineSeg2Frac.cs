using PortalMouse.Engine.Utils.Ext;

namespace PortalMouse.Engine.Utils.Math;

public record struct AxisLineSeg2Frac(V2Frac Pos, Frac Size, Axis Axis) {
	public readonly R1Frac Range => R1Frac.InitBeginSize(Pos[Axis], Size);
	
	public static implicit operator AxisLineSeg2Frac(AxisLineSeg2I self) => new(self.Pos, self.Size, self.Axis);
	public static explicit operator AxisLineSeg2I(AxisLineSeg2Frac self) => new((V2I)self.Pos, (int)self.Size, self.Axis);
	public static explicit operator AxisLine2Frac(AxisLineSeg2Frac self) => new(self.Pos[self.Axis.Opposite()], self.Axis);
}
