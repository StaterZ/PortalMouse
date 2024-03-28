namespace MouseControllerThing.Utils;

public static class Geometry {
	public static V2I? Intersect(LineSeg2I a, AxisLineSeg2I b) {
		switch (b.Axis) {
			case Axis.Horizontal:
				V2I beginDelta = a.Begin - b.Pos;
				V2I endDelta = a.End - b.Pos;
				if ((beginDelta.y < 0) == (endDelta.y < 0))
					return null;

				Frac frac = new(
					Math.Abs(beginDelta.y),
					Math.Abs(a.End.y - a.Begin.y)
				);

				int xIntersect = frac.Lerp(new R1I(a.Begin.x, a.End.x));
				if (!b.Range.Contains(xIntersect))
					return null;

				return new V2I(xIntersect, b.Pos.y);
			case Axis.Vertical:

				return ret;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}
