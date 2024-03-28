using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public sealed class Screen {
	public readonly R2I PhysicalRect;
	public readonly R2I LogicalRect;
	private readonly Edge m_left;
	private readonly Edge m_right;
	private readonly Edge m_top;
	private readonly Edge m_bottom;

	public Screen(ScreenInfo monitorInfo) {
		PhysicalRect = new R2I(monitorInfo.PhysicalRect);
		LogicalRect = new R2I(monitorInfo.LogicalRect);

		m_left = new Edge(this, Side.Left);
		m_right = new Edge(this, Side.Right);
		m_top = new Edge(this, Side.Top);
		m_bottom = new Edge(this, Side.Bottom);
	}

	public V2I? Handle(V2I p) {
		p -= PhysicalRect.Pos;

		V2I? result = null;
		if (p.x <= 0) result ??= m_left.Handle(p.y, -p.x);
		if (p.x >= PhysicalRect.Size.x - 1) result ??= m_right.Handle(p.y, p.x - (PhysicalRect.Size.x - 1));
		if (p.y <= 0) result ??= m_top.Handle(p.x, -p.y);
		if (p.y >= PhysicalRect.Size.y - 1) result ??= m_bottom.Handle(p.x, p.y - (PhysicalRect.Size.y - 1));

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

	public V2I FromPhysicalToLogicalSpace_Pos(V2I p) => MathX.Map(p, PhysicalRect, LogicalRect);
	public V2I FromLogicalToPhysicalSpace_Pos(V2I p) => MathX.Map(p, LogicalRect, PhysicalRect);
	public V2F FromPhysicalToLogicalSpace_Vec(V2F p) => MathX.MapVec(p, PhysicalRect, LogicalRect);
	public V2F FromLogicalToPhysicalSpace_Vec(V2F p) => MathX.MapVec(p, LogicalRect, PhysicalRect);
}
