﻿using PortalMouse.Core;
using PortalMouse.Native;
using PortalMouse.Utils.Math;
using Point = PortalMouse.Native.Point;

namespace PortalMouse.Utils.Misc;

internal static class NativeHelper {
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
		AssertSuccess(User32.ShowWindow(Kernel32.GetConsoleWindow(), shouldShow ? User32.SW_SHOW : User32.SW_HIDE), nameof(User32.ShowWindow));
	}

	//future improvement ideas: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-display_devicew
	//future improvement ideas: https://stackoverflow.com/questions/4958683/how-do-i-get-the-actual-monitor-name-as-seen-in-the-resolution-dialog
	public static List<ScreenInfo> EnumDisplays() {
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

		Graphics g = Graphics.FromHwnd(IntPtr.Zero);
		IntPtr desktopHdc = g.GetHdc();
		AssertSuccess(User32.EnumDisplayMonitors(desktopHdc, IntPtr.Zero, Proc, IntPtr.Zero), nameof(User32.EnumDisplayMonitors));
		g.Dispose();

		return result;
	}

	public static void AssertSuccess(bool ok, string funcName) {
		//if (!ok) throw new NativeErrorException($"'{funcName}' Failed!");
	}
}
