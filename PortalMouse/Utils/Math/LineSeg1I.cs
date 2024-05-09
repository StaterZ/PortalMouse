namespace PortalMouse.Utils.Math;

using System;

public record struct LineSeg1I(int Begin, int End) {
	public readonly int Delta => End - Begin;

	public readonly R1I Range => new(Math.Min(Begin, End), Math.Max(Begin, End));

	public readonly LineSeg1I RelativeTo(int pos) => new(
		Begin - pos,
		End - pos
	);

	public static LineSeg1I InitBeginDelta(int Begin, int Delta) => new(Begin, Begin + Delta);
}
