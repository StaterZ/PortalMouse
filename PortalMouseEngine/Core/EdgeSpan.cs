using PortalMouse.Utils.Math;

namespace PortalMouse.Core;

public record EdgeSpan {
	public Edge Edge;
	public R1I Range;

	public EdgeSpan(Edge edge, R1I localRange) {
		Edge = edge;
		Range = localRange + edge.Offset; //TODO: slightly wonky to offset here, but it's okay for now...
	}
}
