namespace MouseControllerThing.Utils.Maths;

public readonly record struct LineSeg2I(V2I Begin, V2I End)
{
	public V2I Delta => End - Begin;
	public LineSeg1I X => new(Begin.x, End.x);
	public LineSeg1I Y => new(Begin.y, End.y);

	public LineSeg2I RelativeTo(V2I pos) => new(
		Begin - pos,
		End - pos
	);

	public LineSeg2I Transpose() => new(Begin.Transpose(), End.Transpose());

	public LineSeg1I this[Axis axis] => axis switch {
		Axis.Horizontal => X,
		Axis.Vertical => Y,
		_ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
	};
}
