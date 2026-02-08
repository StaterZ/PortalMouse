namespace PortalMouse.Engine.Utils.Math;

public static class Geometry {
	public static (Frac lineFrac, Frac mouseFrac)? Intersect(LineSeg2Frac a, AxisLineSeg2Frac b, bool isBounded, bool isInsidePositive) {
		 LineSeg2Frac aFromB = a.RelativeTo(b.Pos).ToUnitSpace(b.Axis);

		return IntersectLocal(aFromB, b.Size, isBounded, isInsidePositive);
	}

	private static (Frac lineFrac, Frac mouseFrac)? IntersectLocal(LineSeg2Frac aFromB, Frac axisLineSize, bool isBounded, bool isInsidePositive) {
		Frac? mouseFracOpt = ComputeMouseFrac(aFromB.Y, isInsidePositive);
		if (mouseFracOpt == null) return null;
		Frac mouseFrac = mouseFracOpt.Value;

		Frac lineIntersect = mouseFrac.Lerp(aFromB.X);
		if (isBounded && !new R1Frac(0, axisLineSize).Contains(lineIntersect)) return null;

		Frac lineFrac = lineIntersect / axisLineSize;

		return (lineFrac, mouseFrac);
	}

	public static (Frac lineIntersect, Frac mouseFrac)? Intersect(LineSeg2Frac a, AxisLine2I b, bool isInsidePositive) {
		a = a.ToUnitSpace(b.Axis);

		LineSeg2Frac aFromB = a.RelativeTo(new V2I(0, b.Pos));
		return IntersectLocal(aFromB, isInsidePositive);
	}

	private static (Frac lineIntersect, Frac mouseFrac)? IntersectLocal(LineSeg2Frac aFromB, bool isInsidePositive) {
		Frac? mouseFracOpt = ComputeMouseFrac(aFromB.Y, isInsidePositive);
		if (mouseFracOpt == null) return null;
		Frac mouseFrac = mouseFracOpt.Value;

		Frac lineIntersect = mouseFrac.Lerp(aFromB.X);

		return (lineIntersect, mouseFrac);
	}

	private static Frac? ComputeMouseFrac(LineSeg1Frac aFromB, bool isInsidePositive) {
		if (aFromB.Begin == 0 && aFromB.End == 0) //If both vertices are on the divider line...
			return null; //...then they never crossed it

		//bool steppingOffDividerLine = aFromB.Begin == 0 && aFromB.End != 0;
		bool steppingOffDividerLine = aFromB.Begin == 0 && (
			isInsidePositive && aFromB.End < 0 ||
			!isInsidePositive && aFromB.End > 0
		);
		bool differentSidesOfDividerLine =
			(aFromB.Begin != 0 && aFromB.End != 0) &&
			(aFromB.Begin < 0) != (aFromB.End < 0);
		if (!steppingOffDividerLine && !differentSidesOfDividerLine) //If both vertices are on same side of the divider line...
			return null; //...then there's no intersection

		return MathX.Abs(aFromB.Begin) / MathX.Abs(aFromB.Delta);
	}
}
