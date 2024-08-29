namespace PortalMouse.Engine.Utils.Math;

public record struct LineSeg1Frac(Frac Begin, Frac End) {
	public readonly Frac Delta => End - Begin;

	public readonly R1Frac Range => new(MathX.Min(Begin, End), MathX.Max(Begin, End));

	public readonly LineSeg1Frac RelativeTo(Frac pos) => new(
		Begin - pos,
		End - pos
	);

	public static LineSeg1Frac InitBeginDelta(Frac Begin, Frac Delta) => new(Begin, Begin + Delta);

	public readonly LineSeg1Frac Clamp(R1I range) => new(
		MathX.Clamp(Begin, range.Begin, range.End - 1),
		MathX.Clamp(End, range.Begin, range.End - 1)
	);
}
