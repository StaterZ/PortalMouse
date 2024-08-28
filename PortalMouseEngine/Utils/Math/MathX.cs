namespace PortalMouse.Engine.Utils.Math;

public static class MathX {
	public static int Map(int value, R1I from, R1I to) => (value - from.Begin) * to.Size / from.Size + to.Begin;
	public static V2I Map(V2I value, R2I from, R2I to) => (value - from.Pos) * to.Size / from.Size + to.Pos;
}
