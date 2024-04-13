using MouseControllerThing.Native;
using MouseControllerThing.Utils.Maths;

namespace MouseControllerThing.Core;

public readonly struct ScreenInfo {
	public readonly User32.MonitorInfoEx MonitorInfo;
	public readonly Frac Scale;
	public readonly int Id;

	public User32.Rect LogicalRect => MonitorInfo.rsMonitor;
	public User32.Rect PhysicalRect => new(
		LogicalRect.Left,
		LogicalRect.Top,
		LogicalRect.Left + (LogicalRect.Right - LogicalRect.Left) * Scale,
		LogicalRect.Top + (LogicalRect.Bottom - LogicalRect.Top) * Scale
	);

	public ScreenInfo(User32.MonitorInfoEx monitorInfo, Frac scale) {
		MonitorInfo = monitorInfo;
		Scale = scale;

		const string IdPrefix = @"\\.\DISPLAY";
		if (!MonitorInfo.szDevice.StartsWith(IdPrefix)) throw new Exception();

		string idStr = MonitorInfo.szDevice.Substring(IdPrefix.Length);
		if (!int.TryParse(idStr, out Id)) throw new Exception();
	}
}
