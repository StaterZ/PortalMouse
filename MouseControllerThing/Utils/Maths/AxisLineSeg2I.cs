namespace MouseControllerThing.Utils.Maths;

public struct AxisLineSeg2I {
	public V2I Pos;
	public int Size;
	public Axis Axis;

	public R1I Range => new(Pos[Axis], Size);
}
