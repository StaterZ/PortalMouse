namespace MouseControllerThing.Utils.Maths;

public static class MathX {
	public static int Map(int value, R1I from, R1I to) => (value - from.Begin) * to.Size / from.Size + to.Begin;
	public static V2I Map(V2I value, R2I from, R2I to) => (value - from.Pos) * to.Size / from.Size + to.Pos;

	public static V2F MapLocal(V2F value, R2I from, R2I to) => value * (V2F)from.Size / (V2F)to.Size;
}
