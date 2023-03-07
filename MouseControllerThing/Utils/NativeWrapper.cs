using MouseControllerThing.Core;
using System;
using System.Collections.Generic;
namespace MouseControllerThing.Utils;

public static class NativeWrapper {
	public static void ShowConsole(bool shouldShow) {
		Native.ShowWindow(Native.GetConsoleWindow(), shouldShow ? Native.SW_SHOW : Native.SW_HIDE);
	}
	public static void ShowConsole(IntPtr hWnd, bool shouldShow) {
		Native.ShowWindow(hWnd, shouldShow ? Native.SW_SHOW : Native.SW_HIDE);
	}

	//http://pinvoke.net/default.aspx/user32/EnumDisplayMonitors.html
	public static List<ScreenInfo> GetDisplays() {
		List<ScreenInfo> result = new();

		Graphics g = Graphics.FromHwnd(IntPtr.Zero);
		IntPtr desktop = g.GetHdc();
		Native.EnumDisplayMonitors(desktop, IntPtr.Zero,
			(IntPtr hMonitor, IntPtr hdcMonitor, ref Native.Rect lprcMonitor, IntPtr dwData) => {
				Native.MonitorInfo mi = new();
				if (Native.GetMonitorInfo(hMonitor, mi)) {
					float scale = GetScalingFactor(hdcMonitor);
					result.Add(new ScreenInfo(mi, scale));
				}
				return true;
			}, IntPtr.Zero
		);

		return result;
	}

	public static float GetScalingFactor(IntPtr hdc) {
		int LogicalScreenHeight = Native.GetDeviceCaps(hdc, (int)Native.DeviceCap.VERTRES);
		int PhysicalScreenHeight = Native.GetDeviceCaps(hdc, (int)Native.DeviceCap.DESKTOPVERTRES);

		float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

		return ScreenScalingFactor;
	}
}
