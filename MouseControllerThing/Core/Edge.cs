using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public class Edge {
	public readonly List<Connection> Connections = new();
	public readonly Side Side;
	private readonly Screen m_screen;

	private V2I Pos => m_screen.PhysicalRect.Pos + (m_screen.PhysicalRect.Size - V2I.One) * Side switch {
		Side.Left => new V2I(0, 0),
		Side.Right => new V2I(1, 0),
		Side.Top => new V2I(0, 0),
		Side.Bottom => new V2I(0, 1),
		_ => throw new ArgumentOutOfRangeException()
	};

	public int Length => Side switch {
		Side.Left => m_screen.PhysicalRect.Size.y,
		Side.Right => m_screen.PhysicalRect.Size.y,
		Side.Top => m_screen.PhysicalRect.Size.x,
		Side.Bottom => m_screen.PhysicalRect.Size.x,
		_ => throw new ArgumentOutOfRangeException()
	};

	public Edge(Screen screen, Side side) {
		m_screen = screen;
		Side = side;
	}

	public V2I GetLandingSite(int p, int overStep) {
		overStep = Math.Max(overStep - 1, 1);
		return Pos + Side switch {
			Side.Left => new V2I(overStep, p),
			Side.Right => new V2I(-overStep, p),
			Side.Top => new V2I(p, overStep),
			Side.Bottom => new V2I(p, -overStep),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	public V2I? Handle(int p, int overStep) {
		foreach (Connection connection in Connections) {
			V2I? result = connection.TryRemap(this, p, overStep);
			if (result.HasValue) return result.Value;
		}
		return null;
	}
}
