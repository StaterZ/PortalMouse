using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public struct ScreenInfo {
	public readonly Native.MonitorInfo MonitorInfo;
	public readonly float Scale;

	public Native.Rect LogicalRect => MonitorInfo.Monitor;
	public Native.Rect PhysicalRect => new(
		LogicalRect.Left,
		LogicalRect.Top,
		LogicalRect.Left + (int)MathF.Round((LogicalRect.Right - LogicalRect.Left) * Scale),
		LogicalRect.Top + (int)MathF.Round((LogicalRect.Bottom - LogicalRect.Top) * Scale)
	);

	public ScreenInfo(Native.MonitorInfo monitorInfo, float scale) {
		MonitorInfo = monitorInfo;
		Scale = scale;
	}
}
