using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

public class Portal {
	public readonly EdgeSpan EdgeSpan;
	public Portal Exit;
	public readonly int EdgeBarrier;

	private Portal(EdgeSpan edgeSpan, Portal exit, int edgeBarrier) {
		EdgeSpan = edgeSpan;
		Exit = exit;
		EdgeBarrier = edgeBarrier;
	}

	public Frac Map(Frac value) =>
		MathX.Map(value, EdgeSpan.Range, Exit.EdgeSpan.Range);

	public static void Bind(EdgeSpan a, EdgeSpan b, int edgeBarrier = 0) {
		Portal aPortal = new(a, null!, edgeBarrier);
		Portal bPortal = new(b, null!, edgeBarrier);

		aPortal.Exit = bPortal;
		bPortal.Exit = aPortal;

		if (
			!a.Edge.Add(aPortal) ||
			!b.Edge.Add(bPortal)
		) throw new OverlappingPortalsException(aPortal, bPortal);
	}
}
