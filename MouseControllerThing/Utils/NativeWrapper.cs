using MouseControllerThing.Core;
using MouseControllerThing.Native;
using MouseControllerThing.Utils.Maths;

namespace MouseControllerThing.Utils;

public static class NativeWrapper {
	public static V2I CursorPos {
		get {
			User32.GetCursorPos(out Point point);
			return (V2I)point;
		}
		set => User32.SetCursorPos(value.x, value.y);
	}

	public static void ShowConsole(bool shouldShow) {
		User32.ShowWindow(Kernel32.GetConsoleWindow(), shouldShow ? User32.SW_SHOW : User32.SW_HIDE);
	}

	//http://pinvoke.net/default.aspx/user32/EnumDisplayMonitors.html
	//https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-monitorinfoexa
	//future ideas: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-display_devicea
	//future ideas: https://stackoverflow.com/questions/4958683/how-do-i-get-the-actual-monitor-name-as-seen-in-the-resolution-dialog
	public static List<ScreenInfo> GetDisplays() {
		List<ScreenInfo> result = new();
		Graphics g = Graphics.FromHwnd(IntPtr.Zero);
		IntPtr desktop = g.GetHdc();
		User32.EnumDisplayMonitors(desktop, IntPtr.Zero,
			(IntPtr hMonitor, IntPtr hdcMonitor, ref User32.Rect lprcMonitor, IntPtr dwData) => {
				User32.MonitorInfoEx mi = new();
				if (User32.GetMonitorInfo(hMonitor, mi)) {
					Frac scale = GetScalingFactor(hdcMonitor);
					result.Add(new ScreenInfo(mi, scale));
				}
				return true;
			}, IntPtr.Zero
		);

		return result;
	}

	public static Frac GetScalingFactor(IntPtr hdc) {
		int logicalScreenHeight = Gdi32.GetDeviceCaps(hdc, (int)DeviceCap.VERTRES);
		int physicalScreenHeight = Gdi32.GetDeviceCaps(hdc, (int)DeviceCap.DESKTOPVERTRES);

		return new Frac(physicalScreenHeight, logicalScreenHeight);
	}
}
