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
		if (edge == m_a.edge) return (m_a, m_b);
		if (edge == m_b.edge) return (m_b, m_a);
		throw new ArgumentOutOfRangeException();
	}

	public V2I? TryRemap(Edge edge, int p) {
		(EdgeSpan self, EdgeSpan other) = GetSelfOther(edge);
		if (p < self.range.begin || p >= self.range.end) return null;

		float result = p;
		result -= self.range.begin;
		result /= self.range.end - self.range.begin;
		result *= other.range.end - other.range.begin;
		result += other.range.begin;
		return other.edge.GetOutPoint((int)result);
	}

	public static Connection Bind(EdgeSpan a, EdgeSpan b) {
		Connection connection = new(a, b);
		a.edge.Connections.Add(connection);
		b.edge.Connections.Add(connection);
		return connection;
	}
}
