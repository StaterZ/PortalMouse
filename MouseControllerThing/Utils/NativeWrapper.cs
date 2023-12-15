using MouseControllerThing.Core;
using MouseControllerThing.Native;
using System.Windows.Forms;

namespace MouseControllerThing.Utils;

public static class NativeWrapper {
	public static V2I CursorPos {
		get {
			User32.GetCursorPos(out Point point);
			return new V2I(point);
		}
		set => User32.SetCursorPos(value.x, value.y);
	}

	public static void ShowConsole(bool shouldShow) {
		User32.ShowWindow(Kernel32.GetConsoleWindow(), shouldShow ? User32.SW_SHOW : User32.SW_HIDE);
	}

	//http://pinvoke.net/default.aspx/user32/EnumDisplayMonitors.html
	public static List<ScreenInfo> GetDisplays() {
		List<ScreenInfo> result = new();

		Graphics g = Graphics.FromHwnd(IntPtr.Zero);
		IntPtr desktop = g.GetHdc();
		User32.EnumDisplayMonitors(desktop, IntPtr.Zero,
			(IntPtr hMonitor, IntPtr hdcMonitor, ref User32.Rect lprcMonitor, IntPtr dwData) => {
				User32.MonitorInfo mi = new();
				if (User32.GetMonitorInfo(hMonitor, mi)) {
					float scale = GetScalingFactor(hdcMonitor);
					result.Add(new ScreenInfo(mi, scale));
				}
				return true;
			}, IntPtr.Zero
		);

		return result;
	}

	public static float GetScalingFactor(IntPtr hdc) {
		int logicalScreenHeight = Gdi32.GetDeviceCaps(hdc, (int)DeviceCap.VERTRES);
		int physicalScreenHeight = Gdi32.GetDeviceCaps(hdc, (int)DeviceCap.DESKTOPVERTRES);

		return (float)physicalScreenHeight / (float)logicalScreenHeight;
	}
}
