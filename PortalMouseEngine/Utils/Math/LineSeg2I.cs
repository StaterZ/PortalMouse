namespace PortalMouse.Engine.Utils.Math;

public record struct LineSeg2I(V2I Begin, V2I End) {
	public readonly V2I Delta => End - Begin;
	public readonly LineSeg1I X => new(Begin.x, End.x);
	public readonly LineSeg1I Y => new(Begin.y, End.y);

	public readonly LineSeg2I RelativeTo(V2I pos) => new(
		Begin - pos,
		End - pos
	);

	public readonly LineSeg2I Transpose() => new(Begin.Transpose(), End.Transpose());

	public override readonly string ToString() => $"{Begin}->{End}";

	public readonly LineSeg2I ToUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new ArgumentOutOfRangeException(nameof(axis)),
	};

	public readonly LineSeg2I FromUnitSpace(Axis axis) => axis switch {
		Axis.Horizontal => this,
		Axis.Vertical => Transpose(),
		_ => throw new ArgumentOutOfRangeException(nameof(axis)),
	};

	public readonly LineSeg1I this[Axis axis] => axis switch {
		Axis.Horizontal => X,
		Axis.Vertical => Y,
		_ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
	};

	public static LineSeg2I InitBeginDelta(V2I Begin, V2I Delta) => new(Begin, Begin + Delta);
}
