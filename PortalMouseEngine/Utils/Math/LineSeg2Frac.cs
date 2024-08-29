namespace PortalMouse.Engine.Utils.Math;

public record struct LineSeg2Frac(V2Frac Begin, V2Frac End) {
	public readonly V2Frac Delta => End - Begin;
	public readonly LineSeg1Frac X => new(Begin.x, End.x);
	public readonly LineSeg1Frac Y => new(Begin.y, End.y);

	public readonly LineSeg2Frac RelativeTo(V2Frac pos) => new(
		Begin - pos,
		End - pos
	);

	public readonly LineSeg2Frac Transpose() => new(Begin.Transpose(), End.Transpose());

	public override readonly string ToString() => $"{Begin}->{End}";

	public readonly LineSeg2Frac ToUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new ArgumentOutOfRangeException(nameof(axis)),
	};

	public readonly LineSeg2Frac FromUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new ArgumentOutOfRangeException(nameof(axis)),
	};

	public readonly LineSeg1Frac this[Axis axis] => axis switch {
		Axis.Horizontal => X,
		Axis.Vertical => Y,
		_ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
	};

	public static LineSeg2Frac InitBeginDelta(V2Frac Begin, V2Frac Delta) => new(Begin, Begin + Delta);
}
