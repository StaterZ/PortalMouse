using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

public record EdgeRange {
	public Edge Edge;
	private R1I m_range;

	public R1I LocalRange => m_range;
	public R1I Range => m_range + Edge.Offset; //TODO: slightly wonky to offset here, but it's okay for now...

	public EdgeRange(Edge edge, R1I range) {
		Edge = edge;
		m_range = range;
	}

	public override string ToString() => $"screen{Edge.Screen.Id} {Edge.Side} [{LocalRange.Begin}-{LocalRange.End}]";
}
