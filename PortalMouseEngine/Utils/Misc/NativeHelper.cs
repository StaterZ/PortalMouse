using PortalMouse.Engine.Core;
using PortalMouse.Engine.Native;
using PortalMouse.Engine.Utils.Math;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static PortalMouse.Engine.Native.WinError;

namespace PortalMouse.Engine.Utils.Misc;

public static class NativeHelper {
	public static V2I CursorPos {
		get {
			AssertSuccess(User32.GetCursorPos(out WinDef.Point point), nameof(User32.GetCursorPos));
			return (V2I)point;
		}
		set {
			AssertSuccess(User32.SetCursorPos(value.x, value.y), nameof(User32.SetCursorPos));
		}
	}

	public static bool IsKeyDown(int vKey) => (User32.GetAsyncKeyState(vKey) & 0x8000) != 0;

	public static void ShowConsole(bool shouldShow) {
		User32.ShowWindow(Kernel32.GetConsoleWindow(), shouldShow ? User32.SW_SHOW : User32.SW_HIDE);
	}

	//future improvement ideas: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-display_devicew
	//future improvement ideas: https://stackoverflow.com/questions/4958683/how-do-i-get-the-actual-monitor-name-as-seen-in-the-resolution-dialog
	internal static List<ScreenDesc> EnumScreenDescs() {
		static Frac GetScalingFactor(IntPtr hdc) {
			int logicalScreenHeight = Gdi32.GetDeviceCaps(hdc, (int)Gdi32.DeviceCap.VERTRES);
			int physicalScreenHeight = Gdi32.GetDeviceCaps(hdc, (int)Gdi32.DeviceCap.DESKTOPVERTRES);

			return new Frac(physicalScreenHeight, logicalScreenHeight);
		}

		IEnumerator<string> friendlyNames = EnumFriendlyMonitorNames();

		List<ScreenDesc> result = new();
		bool Proc(IntPtr hMonitor, IntPtr hdcMonitor, ref WinDef.Rect lprcMonitor, IntPtr dwData) {
			User32.MONITORINFOEXW monitorInfo = new();
			if (!User32.GetMonitorInfo(hMonitor, ref monitorInfo)) {
				Terminal.Err($"Failed to get monitor info. {nameof(hMonitor)}={hMonitor}");
				return true;
			}

			Frac scale = GetScalingFactor(hdcMonitor);
			friendlyNames.MoveNext();
			ScreenDesc desc = new() {
				MonitorInfo = monitorInfo,
				Scale = scale,
				FriendlyName = friendlyNames.Current,
			};

			Gdi32.DISPLAY_DEVICEW displayDevice = new();
			if (User32.EnumDisplayDevicesW(monitorInfo.szDevice, 0, ref displayDevice, 0)) {
				displayDevice.DeviceKey = displayDevice.DeviceKey.Trim().Replace(@"\Registry\Machine\", @"HKLM:\");
				desc.DisplayDevice = displayDevice;
			}

			result.Add(desc);
			return true;
		}

		IntPtr desktopHdc = AssertSuccess(User32.GetDC(IntPtr.Zero), nameof(User32.GetDC));
		AssertSuccess(User32.EnumDisplayMonitors(desktopHdc, IntPtr.Zero, Proc, IntPtr.Zero), nameof(User32.EnumDisplayMonitors));

		return result;
	}

	private static IEnumerator<string> EnumFriendlyMonitorNames() {
		AssertSuccess(User32.GetDisplayConfigBufferSizes(
			Gdi32.QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
			out uint pathCount,
			out uint modeCount
		), nameof(User32.GetDisplayConfigBufferSizes));

		Gdi32.DISPLAYCONFIG_PATH_INFO[] displayPaths = new Gdi32.DISPLAYCONFIG_PATH_INFO[pathCount];
		Gdi32.DISPLAYCONFIG_MODE_INFO[] displayModes = new Gdi32.DISPLAYCONFIG_MODE_INFO[modeCount];
		AssertSuccess(User32.QueryDisplayConfig(
			Gdi32.QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
			ref pathCount,
			displayPaths,
			ref modeCount,
			displayModes,
			IntPtr.Zero
		), nameof(User32.QueryDisplayConfig));

		for (int i = 0; i < modeCount; i++) {
			if (displayModes[i].infoType != Gdi32.DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET) continue;

			yield return GetMonitorFriendlyName(displayModes[i].adapterId, displayModes[i].id);
		}
	}

	private static string GetMonitorFriendlyName(WinNt.LUID adapterId, uint targetId) {
		Gdi32.DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName = new() {
			header = {
				size = (uint)Marshal.SizeOf<Gdi32.DISPLAYCONFIG_TARGET_DEVICE_NAME>(),
				adapterId = adapterId,
				id = targetId,
				type = Gdi32.DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
			}
		};
		AssertSuccess(User32.DisplayConfigGetDeviceInfo(ref deviceName), nameof(User32.DisplayConfigGetDeviceInfo));
		return deviceName.monitorFriendlyDeviceName;
	}

	public static void EnableDpiAwareness() {
		AssertSuccess(Shcore.SetProcessDpiAwareness(ShellScalingApi.ProcessDpiAwareness.ProcessPerMonitorDpiAware) == IntPtr.Zero, nameof(Shcore.SetProcessDpiAwareness));
	}

	internal static ExitCode AssertSuccess(ExitCode code, string funcName) {
		AssertSuccess(code == ExitCode.ERROR_SUCCESS, funcName);
		return code;
	}
	internal static IntPtr AssertSuccess(IntPtr ptr, string funcName) {
		AssertSuccess(ptr != IntPtr.Zero, funcName);
		return ptr;
	}
	internal static void AssertSuccess(bool ok, string funcName) {
		if (ok) return;

		int code = Marshal.GetLastWin32Error();
		string msg = new Win32Exception(code).Message;
		string fmt = $"'{funcName}' Failed! Code:{code} {msg}";
		Terminal.Wrn(fmt);
		//throw new NativeErrorException(fmt);
	}
}
