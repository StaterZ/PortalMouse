using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

public sealed class Screen {
	public readonly int Id;
	public readonly R2I LogicalRect;
	public readonly Frac Scale;

	public readonly Edge Left;
	public readonly Edge Right;
	public readonly Edge Top;
	public readonly Edge Bottom;

	public R2I PhysicalRect => new(LogicalRect.Pos, LogicalRect.Size * Scale);

	private Screen() {
		Left = new Edge(this, Side.Left);
		Right = new Edge(this, Side.Right);
		Top = new Edge(this, Side.Top);
		Bottom = new Edge(this, Side.Bottom);
	}

	public Screen(int id, R2I physicalRect, Frac scale) : this() {
		Id = id;
		LogicalRect = physicalRect;
		Scale = scale;
	}

	internal Screen(ScreenInfo screenInfo) : this() {
		{ //Parse out id
			const string IdPrefix = @"\\.\DISPLAY";
			if (!screenInfo.MonitorInfo.szDevice.StartsWith(IdPrefix)) throw new Exception("Bad monitor id, bad prefix");

			string idStr = screenInfo.MonitorInfo.szDevice[IdPrefix.Length..];
			if (!int.TryParse(idStr, out Id)) throw new Exception("Bad monitor id, failed to parse");
		}

		LogicalRect = (R2I)screenInfo.MonitorInfo.rcMonitor;
		Scale = screenInfo.Scale;
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
}
