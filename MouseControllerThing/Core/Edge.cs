﻿using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public class Edge {
	public readonly List<Portal> Portals = new();
	public readonly Side Side;
	private readonly Screen m_screen;

	private V2I Pos => m_screen.PhysicalRect.Pos + (m_screen.PhysicalRect.Size - V2I.One) * Side switch {
		Side.Left => new V2I(0, 0),
		Side.Right => new V2I(1, 0),
		Side.Top => new V2I(0, 0),
		Side.Bottom => new V2I(0, 1),
		_ => throw new ArgumentOutOfRangeException(nameof(Side))
	};

	public int Length => Side switch {
		Side.Left => m_screen.PhysicalRect.Size.y,
		Side.Right => m_screen.PhysicalRect.Size.y,
		Side.Top => m_screen.PhysicalRect.Size.x,
		Side.Bottom => m_screen.PhysicalRect.Size.x,
		_ => throw new ArgumentOutOfRangeException(nameof(Side))
	};

	public Edge(Screen screen, Side side) {
		m_screen = screen;
		Side = side;
	}

	public V2I GetLandingSite(int p, int overStep) {
		V2I mappedPos = m_screen.FromPhysicalToLogicalSpace_Pos(Pos + Side switch {
			Side.Left => new V2I(0, p),
			Side.Right => new V2I(0, p),
			Side.Top => new V2I(p, 0),
			Side.Bottom => new V2I(p, 0),
			_ => throw new ArgumentOutOfRangeException(nameof(Side))
		});
		int inset = Math.Max(overStep, 2) - 1;
		V2I insetVec = V2I.Round(m_screen.FromPhysicalToLogicalSpace_Vec(Side switch {
			Side.Left => new V2F(inset, 0),
			Side.Right => new V2F(-inset, 0),
			Side.Top => new V2F(0, inset),
			Side.Bottom => new V2F(0, -inset),
			_ => throw new ArgumentOutOfRangeException(nameof(Side))
		}).EnsureMag(1));
		return mappedPos + insetVec;
	}

	public V2I? Handle(int p, int overStep) =>
		Portals
			.Select(c => c.TryRemap(this, p, overStep))
			.FirstOrDefault(result => result.HasValue);
}