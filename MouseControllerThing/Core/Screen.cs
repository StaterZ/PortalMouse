using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public sealed class Screen {
	public readonly V2I pos;
	public readonly V2I size;
	public readonly Edge left;
	public readonly Edge right;
	public readonly Edge top;
	public readonly Edge bottom;
	private bool wasOnScreen;

	public Screen(Native.MonitorInfo monitorInfo) {
		pos = new V2I(monitorInfo.Monitor.Left, monitorInfo.Monitor.Top);
		size = new V2I(
			monitorInfo.Monitor.Right - monitorInfo.Monitor.Left,
			monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top
		);

		left = new Edge(this, Side.Left);
		right = new Edge(this, Side.Right);
		top = new Edge(this, Side.Top);
		bottom = new Edge(this, Side.Bottom);
	}

	public V2I? Handle(V2I p) {
		p -= pos;

		bool isOnScreen =
			p.x >= 0 && p.x < size.x &&
			p.y >= 0 && p.y < size.y;

		V2I? result = null;
		if (isOnScreen || wasOnScreen) {
			if (p.x <= 0) result ??= left.Handle(p.y);
			if (p.x >= size.x - 1) result ??= right.Handle(p.y);
			if (p.y <= 0) result ??= top.Handle(p.x);
			if (p.y >= size.y - 1) result ??= bottom.Handle(p.x);
		}

		wasOnScreen = isOnScreen && result == null;
		return result;
	}

	public Edge GetEdge(Side side) {
		return side switch {
			Side.Left => left,
			Side.Right => right,
			Side.Top => top,
			Side.Bottom => bottom,
			_ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
		};
	}
}
