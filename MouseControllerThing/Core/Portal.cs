﻿using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public class Portal {
	private readonly EdgeSpan m_a;
	private readonly EdgeSpan m_b;

	private Portal(EdgeSpan a, EdgeSpan b) {
		m_a = a;
		m_b = b;
	}

	private (EdgeSpan self, EdgeSpan other) GetSelfOther(Edge edge) {
		if (edge == m_a.Edge) return (m_a, m_b);
		if (edge == m_b.Edge) return (m_b, m_a);
		throw new ArgumentOutOfRangeException(nameof(edge));
	}

	public V2I? TryRemap(Edge edge, int p, int overStep) {
		(EdgeSpan self, EdgeSpan other) = GetSelfOther(edge);
		if (p < self.Range.Begin || p >= self.Range.End) return null;

		int result = MyMath.Map(p, self.Range, other.Range);
		return other.Edge.GetLandingSite(result, overStep);
	}

	public static Portal Bind(EdgeSpan a, EdgeSpan b) {
		Portal portal = new(a, b);
		a.Edge.Portals.Add(portal);
		b.Edge.Portals.Add(portal);
		return portal;
	}
}