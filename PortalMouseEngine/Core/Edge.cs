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

	public ScreenLineSeg? TryHandle(LineSeg2Frac mouseMove) {
		AxisLineSeg2I axisLine = new() {
			Pos = Pos,
			Size = Length,
			Axis = Axis,
		};

		(Frac lineFrac, Frac mouseFrac)? intersection = Geometry.Intersect(mouseMove, axisLine, false);
		if (!intersection.HasValue)
			return null;

		Frac inPos = Pos[Axis] + Length * intersection.Value.lineFrac;
		V2Frac outMove = mouseMove.Delta * (1 - intersection.Value.mouseFrac);
		LineSeg1Frac inLine = LineSeg1Frac.InitBeginDelta(inPos, outMove[Axis]);
		(Frac pos, Portal? portal) entry = SlideAlongEdgeIntoPortal(inLine.Clamp(axisLine.Range));

		if (entry.portal == null) {
			V2Frac exitPos = new(
				entry.pos,
				InnerPos[Axis.Opposite()]
			);

			exitPos = exitPos.FromUnitSpace(Axis);

			return new ScreenLineSeg(
				new LineSeg2Frac(mouseMove.Begin, exitPos),
				Screen
			);
		} else {
			Edge exitEdge = entry.portal.Exit.EdgeSpan.Edge;

			V2Frac exitEdgePos = new(
				entry.portal.Map(entry.pos),
				exitEdge.Pos[Axis.Opposite()]
			);

			V2Frac entryPos = new(
				exitEdgePos.x,
				exitEdge.InnerPos[Axis.Opposite()]
			);

			V2Frac exitPos = exitEdgePos + new V2Frac(inLine.End - entry.pos, outMove[Axis.Opposite()]);

			LineSeg2Frac line = new(entryPos, exitPos);
			line = line.FromUnitSpace(Axis);

			return new ScreenLineSeg(
				line,
				exitEdge.Screen
			);
		}
	}

	private (Frac pos, Portal? portal) SlideAlongEdgeIntoPortal(LineSeg1Frac line) {
		(bool success, int beginIndex) = m_portals.BetterBinarySearch(
			line.Begin,
			portal => (Frac)portal.EdgeSpan.Range.Begin
		);

		if (success) {
			return (line.Begin, m_portals[beginIndex]);
		} else {
			beginIndex--;
			if (m_portals.IsInRange(beginIndex) && line.Begin < m_portals[beginIndex].EdgeSpan.Range.End) {
				return (line.Begin, m_portals[beginIndex]);
			}

			//UGH!!! stupid C# not allowing struct constants in switch patterns >:(
			if (line.Delta < 0 && m_portals.IsInRange(beginIndex) && line.End < m_portals[beginIndex].EdgeSpan.Range.End)
				return (m_portals[beginIndex].EdgeSpan.Range.End, m_portals[beginIndex]);
			if (line.Delta > 0 && m_portals.IsInRange(beginIndex + 1) && line.End >= m_portals[beginIndex + 1].EdgeSpan.Range.Begin)
				return (m_portals[beginIndex + 1].EdgeSpan.Range.Begin, m_portals[beginIndex + 1]);
			return (line.End, null);
		}
	}
}
