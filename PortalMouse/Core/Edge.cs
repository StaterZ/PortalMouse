using PortalMouse.Utils.Ext;
using PortalMouse.Utils.Math;

namespace PortalMouse.Core;

public class Edge {
	public readonly Side Side;
	public readonly Screen Screen;
	private readonly List<Portal> m_portals = new();

	private V2I Pos => Screen.PhysicalRect.Pos + Screen.PhysicalRect.Size * Side.ToVec();

	private V2I InnerPos => Screen.PhysicalRect.Pos + (Screen.PhysicalRect.Size - 1) * Side.ToVec();

	private Axis Axis => Side.ToDirection().ToAxis().Opposite();

	public int Length => Screen.PhysicalRect.Size[Axis];

	public AxisLineSeg2I AxisLine => new() {
		Pos = Pos,
		Size = Length,
		Axis = Axis,
	};

	public Edge(Screen screen, Side side) {
		Screen = screen;
		Side = side;
	}

	public void Add(Portal portal) {
		(bool success, int index) = m_portals.BetterBinarySearch(
			portal.EdgeSpan.Range.Begin,
			portal => portal.EdgeSpan.Range.Begin
		);

		//check for overlapping portals!
		if (success) throw new ArgumentOutOfRangeException("Overlapping portals!");
		if (index < m_portals.Count) {
			Portal nextPortal = m_portals[index];
			if (portal.EdgeSpan.Range.End < nextPortal.EdgeSpan.Range.Begin) throw new ArgumentOutOfRangeException("Overlapping portals!");
		}

		m_portals.Insert(index, portal);
	}

	public ScreenLineSeg? TryHandle(LineSeg2I mouseMove) {
		(Frac lineFrac, Frac mouseFrac)? intersection = Geometry.Intersect(mouseMove, AxisLine);
		if (!intersection.HasValue)
			return null;

		int inPos = Pos[Axis] + Length * intersection.Value.lineFrac;
		V2I inMove = mouseMove.Delta * intersection.Value.mouseFrac;
		LineSeg1I inLine = LineSeg1I.InitBeginDelta(inPos, inMove[Axis]);
		(int pos, Portal? portal) entry = SlideAlongEdgeIntoPortal(inLine);

		ScreenLineSeg result;
		if (entry.portal == null) {
			V2I exitPos = new(
				InnerPos[Axis.Opposite()],
				entry.pos
			);

			result = new ScreenLineSeg(
				new LineSeg2I(mouseMove.Begin, exitPos),
				Screen
			);
		} else {
			int outMove = mouseMove.Delta[Axis.Opposite()] - inMove[Axis.Opposite()];
			Edge exitEdge = entry.portal.Exit.EdgeSpan.Edge;

			V2I exitEdgePos = new(
				exitEdge.Pos[Axis.Opposite()],
				entry.portal.Map(entry.pos)
			);

			V2I exitPos = exitEdgePos + new V2I(outMove, inLine.End - entry.pos);

			result = new ScreenLineSeg(
				new LineSeg2I(exitEdgePos, exitPos),
				exitEdge.Screen
			);
		}

		if (Axis == Axis.Horizontal) { //Convert out of unit space
			result = new ScreenLineSeg(result.Line.Transpose(), result.Screen);
		}

		return result;
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
