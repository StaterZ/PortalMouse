namespace PortalMouse.Utils.Math;

public struct AxisLineSeg2I {
	public V2I Pos;
	public int Size;
	public Axis Axis;

	public R1I Range => R1I.InitBeginSize(Pos[Axis], Size);
}
