using PortalMouse.Engine.Utils.Math;
using System.Runtime.InteropServices;

namespace PortalMouse.Engine.Native;

internal static class User32 {
	private const string dllName = "user32.dll";

	public const int MONITOR_DEFAULTTOPRIMERTY = 0x00000001;
	public const int MONITOR_DEFAULTTONEAREST = 0x00000002;

	public const int SW_HIDE = 0;
	public const int SW_SHOW = 5;

	public const int WhMouseLl = 14;
	public const int WmMouseMove = 0x0200;

	public const int CCHDEVICENAME = 32;

	public const int MONITORINFOF_PRIMARY = 1;

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-monitorenumproc"></see>
	/// </summary>
	public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/winmsg/lowlevelmouseproc"></see>
	/// </summary>
	public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaymonitors"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmonitorinfow"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true, CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

	/*
	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfromwindow"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	public static extern IntPtr MonitorFromWindow(IntPtr hWnd, int dwFlags);
	*/

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcursorpos"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetCursorPos(int X, int Y);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorpos"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetCursorPos(out Point lpPoint);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	public static extern IntPtr SetWindowsHookEx(HookType idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-unhookwindowshookex"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool UnhookWindowsHookEx(IntPtr hhk);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-callnexthookex"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdc"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	public static extern IntPtr GetDC(IntPtr zero);

	/*
	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-clipcursor"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ClipCursor(ref Rect lpRect);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-clipcursor"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ClipCursor(IntPtr lpRect);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclipcursor"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetClipCursor(out Rect lpRect);
	*/

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-rect"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Rect {
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public Rect(int left, int top, int right, int bottom) {
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public override readonly string ToString() => $"[X:{Left},Y:{Top},W:{Right - Left},H:{Bottom - Top}]";

		public static explicit operator Rect(R2I other) => new(
			other.Pos.x,
			other.Pos.y,
			other.Pos.x + other.Size.x,
			other.Pos.y + other.Size.y
		);

		public static explicit operator R2I(Rect rect) => new(
			new V2I(rect.Left, rect.Top),
			new V2I(rect.Right - rect.Left, rect.Bottom - rect.Top)
		);
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-monitorinfoexw"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct MonitorInfoEx {
		public int cbSize = Marshal.SizeOf(typeof(MonitorInfoEx));
		public Rect rcMonitor;
		public Rect rcWork;
		public int dwFlags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
		public string szDevice;

		public MonitorInfoEx() {
			//Assign everything so the compiler stops complaining
			rcMonitor = default;
			rcWork = default;
			dwFlags = default;
			szDevice = default!;
		}
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msllhookstruct"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct MSLLHOOKSTRUCT {
		public Point pt;
		public uint mouseData;
		public uint flags;
		public uint time;
		public IntPtr dwExtraInfo;
	}
}
