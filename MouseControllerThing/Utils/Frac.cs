namespace MouseControllerThing.Utils;

public readonly record struct Frac(int Numerator, int Denominator) {
	public int Lerp(R1I range) => range.Begin + range.Size * Numerator / Denominator;
}
