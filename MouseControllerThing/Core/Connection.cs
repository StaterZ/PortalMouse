using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public class Connection {
	private EdgeSpan m_a;
	private EdgeSpan m_b;

	public Connection(EdgeSpan a, EdgeSpan b) {
		m_a = a;
		m_b = b;
	}

	private (EdgeSpan self, EdgeSpan other) GetSelfOther(Edge edge) {
		if (edge == m_a.Edge) return (m_a, m_b);
		if (edge == m_b.Edge) return (m_b, m_a);
		throw new ArgumentOutOfRangeException();
	}

	public V2I? TryRemap(Edge edge, int p, int overStep) {
		(EdgeSpan self, EdgeSpan other) = GetSelfOther(edge);
		if (p < self.Range.Begin || p >= self.Range.End) return null;

		int result = MyMath.Map(p, self.Range, other.Range);
		return other.Edge.GetLandingSite(result, overStep);
	}

	public static Connection Bind(EdgeSpan a, EdgeSpan b) {
		Connection connection = new(a, b);
		a.Edge.Connections.Add(connection);
		b.Edge.Connections.Add(connection);
		return connection;
	}
}
