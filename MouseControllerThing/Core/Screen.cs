using MouseControllerThing.Utils;
using MouseControllerThing.Utils.Maths;

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

	public ScreenPos Handle(LineSeg2I mouseMove) {
		return
			m_left.TryHandle(mouseMove) ??
			m_right.TryHandle(mouseMove) ??
			m_top.TryHandle(mouseMove) ??
			m_bottom.TryHandle(mouseMove) ??
			throw new UnreachableException();
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

	public V2I FromPhysicalToLogicalSpace(V2I p) => MathX.Map(p, PhysicalRect, LogicalRect);
	public V2I FromLogicalToPhysicalSpace(V2I p) => MathX.Map(p, LogicalRect, PhysicalRect);
	public V2F FromPhysicalToLogicalSpace_Local(V2F p) => MathX.MapLocal(p, PhysicalRect, LogicalRect);
	public V2F FromLogicalToPhysicalSpace_Local(V2F p) => MathX.MapLocal(p, LogicalRect, PhysicalRect);
}
