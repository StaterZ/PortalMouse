using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

public record EdgeRange {
	public Edge Edge;

	public R1I LocalRange { get; private set; }
	public R1I Range => LocalRange + Edge.ScreenRangeAlongEdgeAxis.Begin;

	public EdgeRange(Edge edge, R1I range) {
		Edge = edge;
		LocalRange = range;
	}

	public override string ToString() => $"screen{Edge.Screen.Id} {Edge.Side} [{LocalRange.Begin}-{LocalRange.End}]";
}
