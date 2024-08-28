using PortalMouse.Engine.Utils.Ext;
using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

public class Edge {
	public readonly Side Side;
	public readonly Screen Screen;
	private readonly List<Portal> m_portals = new();

	private V2I Pos => Screen.LogicalRect.Pos + Screen.LogicalRect.Size * Side.ToVec();

	private V2I InnerPos => Screen.LogicalRect.Pos + (Screen.LogicalRect.Size - 1) * Side.ToVec();

	private Axis Axis => Side.ToDirection().ToAxis().Opposite();

	public int Offset => Screen.LogicalRect.Pos[Axis];
	public int Length => Screen.LogicalRect.Size[Axis];

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
		AxisLineSeg2I axisLine = new() {
			Pos = Pos,
			Size = Length,
			Axis = Axis,
		};

		(Frac lineFrac, Frac mouseFrac)? intersection = Geometry.Intersect(mouseMove, axisLine, false);
		if (!intersection.HasValue)
			return null;

		int inPos = Pos[Axis] + Length * intersection.Value.lineFrac;
		V2I outMove = mouseMove.Delta / intersection.Value.mouseFrac;
		LineSeg1I inLine = LineSeg1I.InitBeginDelta(inPos, outMove[Axis]);
		(int pos, Portal? portal) entry = SlideAlongEdgeIntoPortal(inLine.Clamp(axisLine.Range));

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
			Edge exitEdge = entry.portal.Exit.EdgeSpan.Edge;

			V2I exitEdgePos = new(
				entry.portal.Map(entry.pos),
				exitEdge.Pos[Axis.Opposite()]
			);

			V2I entryPos = new(
				exitEdgePos.x,
				exitEdge.InnerPos[Axis.Opposite()]
			);

			V2I exitPos = exitEdgePos + new V2I(inLine.End - entry.pos, outMove[Axis.Opposite()]);

			LineSeg2I line = new(entryPos, exitPos);
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
			if (m_portals.IsInRange(beginIndex) && line.Begin < m_portals[beginIndex].EdgeSpan.Range.End) {
				return (line.Begin, m_portals[beginIndex]);
			}

			return line.Delta switch {
				< 0 when m_portals.IsInRange(beginIndex) && line.End < m_portals[beginIndex].EdgeSpan.Range.End =>
					(m_portals[beginIndex].EdgeSpan.Range.End, m_portals[beginIndex]),
				> 0 when m_portals.IsInRange(beginIndex + 1) && line.End >= m_portals[beginIndex + 1].EdgeSpan.Range.Begin =>
					(m_portals[beginIndex + 1].EdgeSpan.Range.Begin, m_portals[beginIndex + 1]),
				_ => (line.End, null),
			};
		}
	}
}
