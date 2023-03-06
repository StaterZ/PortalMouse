using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public class Edge {
	public readonly List<Connection> Connections = new();
	public readonly Side Side;
	private readonly Screen m_screen;

	public V2I Pos => m_screen.pos + m_screen.size * Side switch {
		Side.Left => new V2I(0, 0),
		Side.Right => new V2I(1, 0),
		Side.Top => new V2I(0, 0),
		Side.Bottom => new V2I(0, 1),
		_ => throw new ArgumentOutOfRangeException()
	};

	public int Length => Side switch {
		Side.Left => m_screen.size.y,
		Side.Right => m_screen.size.y,
		Side.Top => m_screen.size.x,
		Side.Bottom => m_screen.size.x,
		_ => throw new ArgumentOutOfRangeException()
	};

	public Edge(Screen screen, Side side) {
		m_screen = screen;
		Side = side;
	}

	public V2I GetOutPoint(int p) {
		return Pos + Side switch {
			Side.Left => new V2I(1, p),
			Side.Right => new V2I(-2, p),
			Side.Top => new V2I(p, 1),
			Side.Bottom => new V2I(p, -2),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	public V2I? Handle(int p) {
		foreach (Connection connection in Connections) {
			V2I? result = connection.TryRemap(this, p);
			if (result.HasValue) return result.Value;
		}
		return null;
	}
}
