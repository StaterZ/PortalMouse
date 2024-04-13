namespace MouseControllerThing.Utils.Maths;

public static class Geometry {
	public static (Frac lineFrac, Frac mouseFrac)? Intersect(LineSeg2I a, AxisLineSeg2I b) {
		LineSeg2I aFromB = a.RelativeTo(b.Pos);
		aFromB = b.Axis switch {
			Axis.Horizontal => aFromB,
			Axis.Vertical => aFromB.Transpose(),
			_ => throw new ArgumentOutOfRangeException(nameof(b.Axis)),
		};
		return IntersectLocal(aFromB, b.Size);
	}

	private static (Frac lineFrac, Frac mouseFrac)? IntersectLocal(LineSeg2I aFromB, int axisLineSize) {
		if ((aFromB.Begin.y < 0) == (aFromB.End.y < 0))
			return null;

		int numerator = aFromB.Begin.y;
		//numerator = numerator < 0 ? -numerator : (numerator + 1);
		numerator = Math.Abs(numerator);
		Frac yFrac = new(
			numerator,
			Math.Abs(aFromB.Delta.y)
		);

		int xIntersect = yFrac.Lerp(new R1I(aFromB.Begin.x, aFromB.End.x));
		if (!new R1I(0, axisLineSize).Contains(xIntersect))
			return null;

		Frac xFrac = new(xIntersect, axisLineSize);

		return (xFrac, yFrac);
	}
}
