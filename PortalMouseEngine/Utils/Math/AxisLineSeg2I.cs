namespace PortalMouse.Engine.Utils.Math;

public record struct AxisLineSeg2I(V2I Pos, int Size, Axis Axis) {
	public readonly R1I Range => R1I.InitBeginSize(Pos[Axis], Size);
}
