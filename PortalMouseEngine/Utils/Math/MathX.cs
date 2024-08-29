namespace PortalMouse.Engine.Utils.Math;
using Math = System.Math;

public static class MathX {
	public static int Map(int value, R1I from, R1I to) => (value - from.Begin) * to.Size / from.Size + to.Begin;
	public static Frac Map(Frac value, R1I from, R1I to) => (value - from.Begin) * to.Size / from.Size + to.Begin;

	public static Frac Min(Frac a, Frac b) => a < b ? a : b;
	public static Frac Max(Frac a, Frac b) => a > b ? a : b;
	public static Frac Clamp(Frac value, Frac min, Frac max) => Min(Max(value, min), max);
	public static Frac Abs(Frac value) => new(Math.Abs(value.Numerator), Math.Abs(value.Denominator));
}
