namespace PortalMouse.Engine.Utils.Math;

public record struct LineSeg1Frac(Frac Begin, Frac End) {
	public readonly Frac Delta => End - Begin;
	public readonly R1Frac Range => new(MathX.Min(Begin, End), MathX.Max(Begin, End));

	public readonly LineSeg1Frac RelativeTo(Frac pos) => new(
		Begin - pos,
		End - pos
	);

	public readonly LineSeg1Frac Clamp(R1Frac range) => new(
		range.Clamp(Begin),
		range.Clamp(End)
	);
	
	public static LineSeg1Frac InitBeginDelta(Frac Begin, Frac Delta) => new(Begin, Begin + Delta);
}
