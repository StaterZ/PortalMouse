using PortalMouse.Utils.Ext;
using PortalMouse.Utils.Math;

namespace PortalMouse.Core;

public class Edge {
	public readonly Side Side;
	public readonly Screen Screen;
	private readonly List<Portal> m_portals = new();

	private V2I Pos => Screen.LogicalRect.Pos + Screen.LogicalRect.Size * Side.ToVec();

	private V2I InnerPos => Screen.LogicalRect.Pos + (Screen.LogicalRect.Size - 1) * Side.ToVec();

	private Axis Axis => Side.ToDirection().ToAxis().Opposite();

	public int Length => Screen.LogicalRect.Size[Axis];

	private AxisLineSeg2I AxisLine => new() {
		Pos = Pos,
		Size = Length,
		Axis = Axis,
	};

	public Edge(Screen screen, Side side) {
		Screen = screen;
		Side = side;
	}

	public bool Add(Portal portal) {
		(bool success, int index) = m_portals.BetterBinarySearch(
			portal.EdgeSpan.Range.Begin,
			portal => portal.EdgeSpan.Range.Begin
		);

		//check for overlapping portals!
		if (success) return false;
		if (index < m_portals.Count) {
			Portal nextPortal = m_portals[index];
			if (portal.EdgeSpan.Range.End < nextPortal.EdgeSpan.Range.Begin) return false;
		}

		m_portals.Insert(index, portal);
		return true;
	}

	public ScreenLineSeg? TryHandle(LineSeg2I mouseMove) {
		(Frac lineFrac, Frac mouseFrac)? intersection = Geometry.Intersect(mouseMove, AxisLine, false);
		if (!intersection.HasValue)
			return null;

		int inPos = Pos[Axis] + Length * intersection.Value.lineFrac;
		V2I inMove = mouseMove.Delta * intersection.Value.mouseFrac;
		LineSeg1I inLine = LineSeg1I.InitBeginDelta(inPos, inMove[Axis]);
		(int pos, Portal? portal) entry = SlideAlongEdgeIntoPortal(inLine);

		if (entry.portal == null) {
			V2I exitPos = new(
				entry.pos,
				InnerPos[Axis.Opposite()]
			);

			exitPos = exitPos.FromUnitSpace(Axis);

			return new ScreenLineSeg(
				new LineSeg2I(mouseMove.Begin, exitPos),
				Screen
			);
		} else {
			int outMove = mouseMove.Delta[Axis.Opposite()] - inMove[Axis.Opposite()];
			Edge exitEdge = entry.portal.Exit.EdgeSpan.Edge;

			V2I exitEdgePos = new(
				entry.portal.Map(entry.pos),
				exitEdge.Pos[Axis.Opposite()]
			);

			V2I exitPos = exitEdgePos + new V2I(inLine.End - entry.pos, outMove);

			LineSeg2I line = new(exitEdgePos, exitPos);
			line = line.FromUnitSpace(Axis);

			return new ScreenLineSeg(
				line,
				exitEdge.Screen
			);
		}
	}

	private (int pos, Portal? portal) SlideAlongEdgeIntoPortal(LineSeg1I line) {
		(bool success, int beginIndex) = m_portals.BetterBinarySearch(
			line.Begin,
			portal => portal.EdgeSpan.Range.Begin
		);

		if (success) {
			return (line.Begin, m_portals[beginIndex]);
		} else {
			beginIndex--;
			return line.Delta switch {
				< 0 when m_portals.IsInRange(beginIndex) && line.End < m_portals[beginIndex].EdgeSpan.Range.End =>
					(m_portals[beginIndex].EdgeSpan.Range.End, m_portals[beginIndex]),
				0 when m_portals.IsInRange(beginIndex) && line.End < m_portals[beginIndex].EdgeSpan.Range.End =>
					(line.Begin, m_portals[beginIndex]),
				> 0 when m_portals.IsInRange(beginIndex + 1) && line.End >= m_portals[beginIndex + 1].EdgeSpan.Range.Begin =>
					(m_portals[beginIndex + 1].EdgeSpan.Range.Begin, m_portals[beginIndex + 1]),
				_ => (line.End, null),
			};
		}
	}
}
