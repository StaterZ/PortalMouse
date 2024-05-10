using PortalMouse.Utils.Math;

namespace PortalMouse.Core;

public sealed class Screen {
	public readonly int Id;
	public readonly R2I PhysicalRect;
	public readonly R2I LogicalRect;
	public readonly Edge Left;
	public readonly Edge Right;
	public readonly Edge Top;
	public readonly Edge Bottom;

	public Screen(ScreenInfo monitorInfo) {
		Id = monitorInfo.Id;
		PhysicalRect = (R2I)monitorInfo.PhysicalRect;
		LogicalRect = (R2I)monitorInfo.LogicalRect;

		Left = new Edge(this, Side.Left);
		Right = new Edge(this, Side.Right);
		Top = new Edge(this, Side.Top);
		Bottom = new Edge(this, Side.Bottom);
	}

	public ScreenLineSeg? Handle(LineSeg2I mouseMove) {
		return
			Left.TryHandle(mouseMove) ??
			Right.TryHandle(mouseMove) ??
			Top.TryHandle(mouseMove) ??
			Bottom.TryHandle(mouseMove);
	}

	public Edge GetEdge(Side side) {
		return side switch {
			Side.Left => Left,
			Side.Right => Right,
			Side.Top => Top,
			Side.Bottom => Bottom,
			_ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
		};
	}

	public V2I PhysicalToLogicalSpace(V2I p) => MathX.Map(p, PhysicalRect, LogicalRect);
	public V2I LogicalToPhysicalSpace(V2I p) => MathX.Map(p, LogicalRect, PhysicalRect);
}
