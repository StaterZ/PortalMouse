using PortalMouse.Engine.Core;
using PortalMouse.Engine.Native;
using PortalMouse.Engine.Utils.Math;
using Point = PortalMouse.Engine.Native.Point;

namespace PortalMouse.Engine.Utils.Misc;

public static class NativeHelper {
	public static V2I CursorPos {
		get {
			AssertSuccess(User32.GetCursorPos(out Point point), nameof(User32.GetCursorPos));
			return (V2I)point;
		}
		set {
			AssertSuccess(User32.SetCursorPos(value.x, value.y), nameof(User32.SetCursorPos));
		}
	}

	public static void ShowConsole(bool shouldShow) {
		User32.ShowWindow(Kernel32.GetConsoleWindow(), shouldShow ? User32.SW_SHOW : User32.SW_HIDE);
	}

	//future improvement ideas: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-display_devicew
	//future improvement ideas: https://stackoverflow.com/questions/4958683/how-do-i-get-the-actual-monitor-name-as-seen-in-the-resolution-dialog
	internal static List<ScreenInfo> EnumDisplays() {
		static Frac GetScalingFactor(IntPtr hdc) {
			int logicalScreenHeight = Gdi32.GetDeviceCaps(hdc, (int)Gdi32.DeviceCap.VERTRES);
			int physicalScreenHeight = Gdi32.GetDeviceCaps(hdc, (int)Gdi32.DeviceCap.DESKTOPVERTRES);

			return new Frac(physicalScreenHeight, logicalScreenHeight);
		}

		List<ScreenInfo> result = new();
		bool Proc(IntPtr hMonitor, IntPtr hdcMonitor, ref User32.Rect lprcMonitor, IntPtr dwData) {
			User32.MonitorInfoEx mi = new();
			if (!User32.GetMonitorInfo(hMonitor, ref mi)) {
				Terminal.Err($"Failed to get monitor info. {nameof(hMonitor)}={hMonitor}");
				return true;
			}

			Frac scale = GetScalingFactor(hdcMonitor);
			result.Add(new ScreenInfo(mi, scale));
			return true;
		}

		IntPtr desktopHdc = User32.GetDC(IntPtr.Zero);
		AssertSuccess(User32.EnumDisplayMonitors(desktopHdc, IntPtr.Zero, Proc, IntPtr.Zero), nameof(User32.EnumDisplayMonitors));
		
		return result;
	}

	public static void AssertSuccess(bool ok, string funcName) {
		if (!ok) throw new NativeErrorException($"'{funcName}' Failed!");
	}
}
