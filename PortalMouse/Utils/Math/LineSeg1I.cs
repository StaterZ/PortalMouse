namespace PortalMouse.Utils.Math;

public readonly record struct LineSeg1I(int Begin, int End) {
	public int Delta => End - Begin;

	public LineSeg1I RelativeTo(int pos) => new(
		Begin - pos,
		End - pos
	);

	public static LineSeg1I InitBeginDelta(int Begin, int Delta) => new(Begin, Begin + Delta);
}
