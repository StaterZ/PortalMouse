using MouseControllerThing.Utils.Ext;
using MouseControllerThing.Utils.Maths;

namespace MouseControllerThing.Core;

public class Edge {
	private readonly List<Portal> m_portals = new();
	public readonly Side Side;
	private readonly Screen m_screen;

	private V2I Pos => m_screen.PhysicalRect.Pos + (m_screen.PhysicalRect.Size - 1) * Side switch {
		Side.Left => new V2I(0, 0),
		Side.Right => new V2I(1, 0),
		Side.Top => new V2I(0, 0),
		Side.Bottom => new V2I(0, 1),
		_ => throw new ArgumentOutOfRangeException(nameof(Side))
	};

	public int Length => m_screen.PhysicalRect.Size[Side.ToDirection().ToAxis().Opposite()];

	public AxisLineSeg2I AxisLine => new() {
		Pos = Pos,
		Size = Length,
		Axis = Side.ToDirection().ToAxis(),
	};

	public Edge(Screen screen, Side side) {
		m_screen = screen;
		Side = side;
	}

	public V2I? TryHandle(LineSeg2I mouseMove) {
		(Frac lineFrac, Frac mouseFrac)? intersection = Geometry.Intersect(mouseMove, AxisLine);
		if (!intersection.HasValue)
			return null;

		LineSeg1I mouseMoveAxis = mouseMove[Side.ToDirection().ToAxis()];
		(bool success, int index) = m_portals.BetterBinarySearch(
			mouseMoveAxis.Begin,
			portal => portal.GetSelfOther(this).self.Range.Begin
		);
		if (success) index++;

		switch (mouseMoveAxis.Delta) {
			case <0:
				for (; index >= 0; index--) {
					if (!m_portals[index].GetSelfOther(this).self.Range.Contains((int)intersection.Value.mouseFrac)) continue;
					break;
				}
				break;
			case 0:
				break;
			case >0:
				for (; index < m_portals.Count; index++) {
					if (!m_portals[index].GetSelfOther(this).self.Range.Contains((int)intersection.Value.mouseFrac)) continue;
					break;
				}
				break;
		}
	}
}
