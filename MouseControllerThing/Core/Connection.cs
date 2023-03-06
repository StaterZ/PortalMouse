using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public class Connection {
	private EdgeSpan a;
	private EdgeSpan b;

	public Connection(EdgeSpan a, EdgeSpan b) {
		this.a = a;
		this.b = b;
	}

	private (EdgeSpan self, EdgeSpan other) GetSelfOther(Edge edge) {
		if (edge == a.edge) return (a, b);
		if (edge == b.edge) return (b, a);
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
