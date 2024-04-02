namespace MouseControllerThing.Utils.Maths;

public readonly record struct Frac(int Numerator, int Denominator) {
	public int Lerp(R1I range) =>
		range.Begin + range.Size * Numerator / Denominator;

	public static explicit operator int(Frac self) =>
		self.Numerator / self.Denominator;

	public static explicit operator float(Frac self) =>
		(float)self.Numerator / self.Denominator;
}
