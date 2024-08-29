namespace PortalMouse.Engine.Utils.Math;

using System;

public static class Geometry {
	public static (Frac lineFrac, Frac mouseFrac)? Intersect(LineSeg2I a, AxisLineSeg2I b, bool isBounded) {
		LineSeg2I aFromB = a.RelativeTo(b.Pos).ToUnitSpace(b.Axis);

		return IntersectLocal(aFromB, b.Size, isBounded);
	}

	private static (Frac lineFrac, Frac mouseFrac)? IntersectLocal(LineSeg2I aFromB, int axisLineSize, bool isBounded) {
		Frac? mouseFracOpt = ComputeMouseFrac(aFromB.Y);
		if (mouseFracOpt == null) return null;
		Frac mouseFrac = mouseFracOpt.Value;

		int lineIntersect = mouseFrac.Lerp(aFromB.X);
		if (isBounded && !new R1I(0, axisLineSize).Contains(lineIntersect))
			return null;

		Frac lineFrac = new(lineIntersect, axisLineSize);

		return (lineFrac, mouseFrac);
	}

	public static (int lineIntersect, Frac mouseFrac)? Intersect(LineSeg2I a, AxisLine2I b) {
		a = a.ToUnitSpace(b.Axis);

		LineSeg2I aFromB = a.RelativeTo(new V2I(0, b.Pos));
		return IntersectLocal(aFromB);
	}

	private static (int lineIntersect, Frac mouseFrac)? IntersectLocal(LineSeg2I aFromB) {
		Frac? mouseFracOpt = ComputeMouseFrac(aFromB.Y);
		if (mouseFracOpt == null) return null;
		Frac mouseFrac = mouseFracOpt.Value;

		int lineIntersect = mouseFrac.Lerp(aFromB.X);

		return (lineIntersect, mouseFrac);
	}

	private static Frac? ComputeMouseFrac(LineSeg1I aFromB) {
		if ((aFromB.Begin < 0) == (aFromB.End < 0)) //If both vertices are on same side of the divider line...
			return null; //...then there's no intersection

		return new Frac(
			Math.Abs(aFromB.Begin),
			Math.Abs(aFromB.Delta)
		);
	}


	//===== Frac versions below here =====//


	public static (Frac lineFrac, Frac mouseFrac)? Intersect(LineSeg2Frac a, AxisLineSeg2I b, bool isBounded) {
		LineSeg2Frac aFromB = a.RelativeTo(b.Pos).ToUnitSpace(b.Axis);

		return IntersectLocal(aFromB, b.Size, isBounded);
	}

	private static (Frac lineFrac, Frac mouseFrac)? IntersectLocal(LineSeg2Frac aFromB, int axisLineSize, bool isBounded) {
		Frac? mouseFracOpt = ComputeMouseFrac(aFromB.Y);
		if (mouseFracOpt == null) return null;
		Frac mouseFrac = mouseFracOpt.Value;

		Frac lineIntersect = mouseFrac.Lerp(aFromB.X);
		if (isBounded && !new R1Frac(0, axisLineSize).Contains(lineIntersect))
			return null;

		Frac lineFrac = lineIntersect / axisLineSize;

		return (lineFrac, mouseFrac);
	}

	public static (Frac lineIntersect, Frac mouseFrac)? Intersect(LineSeg2Frac a, AxisLine2I b) {
		a = a.ToUnitSpace(b.Axis);

		LineSeg2Frac aFromB = a.RelativeTo(new V2I(0, b.Pos));
		return IntersectLocal(aFromB);
	}

	private static (Frac lineIntersect, Frac mouseFrac)? IntersectLocal(LineSeg2Frac aFromB) {
		Frac? mouseFracOpt = ComputeMouseFrac(aFromB.Y);
		if (mouseFracOpt == null) return null;
		Frac mouseFrac = mouseFracOpt.Value;

		Frac lineIntersect = mouseFrac.Lerp(aFromB.X);

		return (lineIntersect, mouseFrac);
	}

	private static Frac? ComputeMouseFrac(LineSeg1Frac aFromB) {
		if ((aFromB.Begin < 0) == (aFromB.End < 0)) //If both vertices are on same side of the divider line...
			return null; //...then there's no intersection

		return MathX.Abs(aFromB.Begin) / MathX.Abs(aFromB.Delta);
	}
}
