using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public class Edge {
	public readonly List<Connection> Connections = new();
	private readonly Screen screen;
	private readonly Side side;

	public V2I Pos => screen.pos + screen.size * side switch {
		Side.Left => new V2I(0, 0),
		Side.Right => new V2I(1, 0),
		Side.Top => new V2I(0, 0),
		Side.Bottom => new V2I(0, 1),
		_ => throw new ArgumentOutOfRangeException()
	};

	public int Length => side switch {
		Side.Left => screen.size.x,
		Side.Right => screen.size.x,
		Side.Top => screen.size.y,
		Side.Bottom => screen.size.y,
		_ => throw new ArgumentOutOfRangeException()
	};

	public Edge(Screen screen, Side side) {
		this.screen = screen;
		this.side = side;
	}

	public V2I GetOutPoint(int p) {
		return Pos + side switch {
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
