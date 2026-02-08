namespace PortalMouse.Engine.Utils.Math;
using Math = System.Math;

public static class MathX {
	public static int Map(int value, R1I from, R1I to) => (value - from.Begin) * to.Size / from.Size + to.Begin;
	public static Frac Map(Frac value, R1Frac from, R1Frac to) => (value - from.Begin) * to.Size / from.Size + to.Begin;
	public static float Map(float value, R1I from, R1I to) => (value - from.Begin) * to.Size / from.Size + to.Begin;

	public static int Sign(int value) => value.CompareTo(0);
	public static int Sign(Frac value) => Sign(value.Numerator) * Sign(value.Denominator);
	public static Frac Min(Frac a, Frac b) => a < b ? a : b;
	public static Frac Max(Frac a, Frac b) => a > b ? a : b;
	public static V2Frac Min(V2Frac a, V2Frac b) => new(Min(a.x, b.x), Min(a.y, b.y));
	public static V2Frac Max(V2Frac a, V2Frac b) => new(Max(a.x, b.x), Max(a.y, b.y));
	public static Frac Clamp(Frac value, Frac min, Frac max) => Min(Max(value, min), max);
	public static Frac Abs(Frac value) => new(Math.Abs(value.Numerator), Math.Abs(value.Denominator));
}
