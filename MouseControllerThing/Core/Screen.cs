using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public sealed class Screen {
	public readonly V2I pos;
	public readonly V2I size;
	private readonly Edge m_left;
	private readonly Edge m_right;
	private readonly Edge m_top;
	private readonly Edge m_bottom;
	private bool m_wasOnScreen;

	public Screen(Native.MonitorInfo monitorInfo) {
		pos = new V2I(monitorInfo.Monitor.Left, monitorInfo.Monitor.Top);
		size = new V2I(
			monitorInfo.Monitor.Right - monitorInfo.Monitor.Left,
			monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top
		);

		m_left = new Edge(this, Side.Left);
		m_right = new Edge(this, Side.Right);
		m_top = new Edge(this, Side.Top);
		m_bottom = new Edge(this, Side.Bottom);
	}

	public V2I? Handle(V2I p) {
		p -= pos;

		bool isOnScreen =
			p.x >= 0 && p.x < size.x &&
			p.y >= 0 && p.y < size.y;

		V2I? result = null;
		if (isOnScreen || m_wasOnScreen) {
			if (p.x <= 0) result ??= m_left.Handle(p.y);
			if (p.x >= size.x - 1) result ??= m_right.Handle(p.y);
			if (p.y <= 0) result ??= m_top.Handle(p.x);
			if (p.y >= size.y - 1) result ??= m_bottom.Handle(p.x);
		}

		m_wasOnScreen = isOnScreen && result == null;
		return result;
	}

	public Edge GetEdge(Side side) {
		return side switch {
			Side.Left => m_left,
			Side.Right => m_right,
			Side.Top => m_top,
			Side.Bottom => m_bottom,
			_ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
		};
	}
}
