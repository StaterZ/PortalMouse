using PortalMouse.Engine.Native;
using PortalMouse.Engine.Utils.Ext;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Core;

public class Edge {
	public readonly Side Side;
	public readonly Screen Screen;
	private readonly List<Portal> m_portals = new();

	public IReadOnlyList<Portal> Portals => m_portals;

	private Axis Axis => Side.ToAxis().Opposite();
	public R1I ScreenRangeAlongEdgeAxis => Screen.LogicalRect[Axis];

	public V2I LocalPos => Screen.LogicalRect.Size * Side.ToVec();
	private V2I Pos => Screen.LogicalRect.Pos + LocalPos;

	private V2I LocalPosInclusive => (Screen.LogicalRect.Size - 1) * Side.ToVec();
	private V2I PosInclusive => Screen.LogicalRect.Pos + LocalPosInclusive;
	
	public Edge(Screen screen, Side side) {
		Screen = screen;
		Side = side;
	}

	public bool Add(Portal portal) {
		(bool success, int index) = m_portals.BetterBinarySearch(
			portal.Desc.EdgeRange.Range.Begin,
			portal => portal.Desc.EdgeRange.Range.Begin
		);

		//check for overlapping portals!
		if (success) return false;
		if (index < m_portals.Count) {
			Portal nextPortal = m_portals[index];
			if (portal.Desc.EdgeRange.Range.End < nextPortal.Desc.EdgeRange.Range.Begin) return false;
		}

		m_portals.Insert(index, portal);
		return true;
	}

	public ScreenLineSeg? TryHandle(LineSeg2Frac mouseMove) {
		AxisLineSeg2I entryEdgeCollider = new() {
			Pos = Pos,
			Size = ScreenRangeAlongEdgeAxis.Size,
			Axis = Axis,
		};

		(Frac entryEdgeFrac, Frac mouseFrac)? intersection = Geometry.Intersect(mouseMove, entryEdgeCollider, false);
		if (!intersection.HasValue) return null; //if mouse didn't cross the entry edge, then return

		Frac inPos = intersection.Value.entryEdgeFrac.Lerp(entryEdgeCollider.Range);
		V2Frac outMove = mouseMove.Delta * (1 - intersection.Value.mouseFrac);
		LineSeg1Frac inLine = LineSeg1Frac.InitBeginDelta(inPos, outMove[Axis]);
		LineSeg1Frac slideRange = inLine.Clamp(entryEdgeCollider.Range);
		(Frac pos, Portal? portal) entry = SlideAlongEdgeIntoPortal(slideRange);
		if (outMove[Axis] < entry.portal?.Desc.EdgeBarrier && NativeHelper.IsKeyDown(User32.VK_LBUTTON)) {
			entry = (slideRange.End, null);
		}

		if (entry.portal == null) {
			V2Frac exitPos = new(
				entry.pos,
				PosInclusive[Axis.Opposite()]
			);

			exitPos = exitPos.FromUnitSpace(Axis);

			return new ScreenLineSeg(
				new LineSeg2Frac(mouseMove.Begin, exitPos),
				Screen
			);
		} else {
			Edge exitEdge = entry.portal.Exit.Desc.EdgeRange.Edge;

			V2Frac exitEdgeEntryPos = new(
				entry.portal.Map(entry.pos),
				exitEdge.PosInclusive[Axis.Opposite()]
			);

			V2Frac exitEdgeEndPos = new(
				exitEdgeEntryPos.x,
				exitEdge.Pos[Axis.Opposite()]
			);
			V2Frac exitMove = new(inLine.End - entry.pos, outMove[Axis.Opposite()]);
			exitEdgeEndPos += exitMove;

			LineSeg2Frac line = new(exitEdgeEntryPos, exitEdgeEndPos);
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
			portal => (Frac)portal.Desc.EdgeRange.Range.Begin
		);

		if (success) {
			return (line.Begin, m_portals[beginIndex]);
		} else {
			beginIndex--;
			if (m_portals.IsInRange(beginIndex) && line.Begin < m_portals[beginIndex].Desc.EdgeRange.Range.End) {
				return (line.Begin, m_portals[beginIndex]);
			}

			//UGH!!! stupid C# not allowing struct constants in switch patterns >:(
			if (line.Delta < 0 && m_portals.IsInRange(beginIndex) && line.End < m_portals[beginIndex].Desc.EdgeRange.Range.End)
				return (m_portals[beginIndex].Desc.EdgeRange.Range.End, m_portals[beginIndex]);
			if (line.Delta > 0 && m_portals.IsInRange(beginIndex + 1) && line.End >= m_portals[beginIndex + 1].Desc.EdgeRange.Range.Begin)
				return (m_portals[beginIndex + 1].Desc.EdgeRange.Range.Begin, m_portals[beginIndex + 1]);
			return (line.End, null);
		}
	}
}
