using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseControllerThing.Utils;

public static class NativeWrapper {
	public static void ShowConsole(bool shouldShow) {
		Native.ShowWindow(Native.GetConsoleWindow(), shouldShow ? Native.SW_SHOW : Native.SW_HIDE);
	}

	//http://pinvoke.net/default.aspx/user32/EnumDisplayMonitors.html
	public static List<Native.MonitorInfo> GetDisplays() {
		List<Native.MonitorInfo> result = new();

		Native.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
			(IntPtr hMonitor, IntPtr hdcMonitor, ref Native.Rect lprcMonitor, IntPtr dwData) => {
				Native.MonitorInfo mi = new();
				if (Native.GetMonitorInfo(hMonitor, mi)) {
					result.Add(mi);
				}
				return true;
			}, IntPtr.Zero
		);

		return result;
	}
}
