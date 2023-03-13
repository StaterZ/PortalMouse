using MouseControllerThing.Native;

namespace MouseControllerThing.Core;

public struct ScreenInfo {
	public readonly User32.MonitorInfo MonitorInfo;
	public readonly float Scale;

	public User32.Rect LogicalRect => MonitorInfo.Monitor;
	public User32.Rect PhysicalRect => new(
		LogicalRect.Left,
		LogicalRect.Top,
		LogicalRect.Left + (int)MathF.Round((LogicalRect.Right - LogicalRect.Left) * Scale),
		LogicalRect.Top + (int)MathF.Round((LogicalRect.Bottom - LogicalRect.Top) * Scale)
	);

	public ScreenInfo(User32.MonitorInfo monitorInfo, float scale) {
		MonitorInfo = monitorInfo;
		Scale = scale;
	}
}
