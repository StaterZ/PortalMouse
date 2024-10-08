﻿using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

public class Portal {
	public readonly EdgeSpan EdgeSpan;
	public Portal Exit;

	private Portal(EdgeSpan edgeSpan, Portal exit) {
		EdgeSpan = edgeSpan;
		Exit = exit;
	}

	public Frac Map(Frac value) =>
		MathX.Map(value, EdgeSpan.Range, Exit.EdgeSpan.Range);

	public static void Bind(EdgeSpan a, EdgeSpan b) {
		Portal aPortal = new(a, null!);
		Portal bPortal = new(b, null!);

		aPortal.Exit = bPortal;
		bPortal.Exit = aPortal;

		if (
			!a.Edge.Add(aPortal) ||
			!b.Edge.Add(bPortal)
		) throw new OverlappingPortalsException(aPortal, bPortal);
	}
}
