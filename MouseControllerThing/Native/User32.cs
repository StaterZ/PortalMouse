using MouseControllerThing.Utils;
using System.Runtime.InteropServices;

namespace MouseControllerThing.Native;

public static class User32
{
	public const int MONITOR_DEFAULTTOPRIMERTY = 0x00000001;
	public const int MONITOR_DEFAULTTONEAREST = 0x00000002;
	public const int SW_HIDE = 0;
	public const int SW_SHOW = 5;
	public const int WhMouseLl = 14;
	public const int WmMouseMove = 0x0200;

	public delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);
	public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

	[DllImport("user32.dll")]
	public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

	[DllImport("user32.dll")]
	public static extern bool GetMonitorInfo(IntPtr hMonitor, MonitorInfo lpmi);

	[DllImport("user32.dll")]
	public static extern bool SetCursorPos(int X, int Y);

	[DllImport("user32.dll")]
	public static extern bool GetCursorPos(out Point pos);

	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool UnhookWindowsHookEx(IntPtr hhk);

	[DllImport("user32.dll")]
	public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern bool ClipCursor(ref Rect lpRect);

	[DllImport("user32.dll")]
	public static extern bool GetClipCursor(out Rect lpRect);

	[Serializable, StructLayout(LayoutKind.Sequential)]
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

		public Rect(R2I other) : this(
			other.Pos.x,
			other.Pos.y,
			other.Pos.x + other.Size.x,
			other.Pos.y + other.Size.y
		) { }

		public override string ToString() => $"[X:{Left},Y:{Top},W:{Right - Left},H:{Bottom - Top}]";
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public sealed class MonitorInfo {
		public int Size = Marshal.SizeOf(typeof(MonitorInfo));
		public Rect Monitor;
		public Rect Work;
		public int Flags;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct Msllhookstruct
	{
		public Point pt;
		public uint mouseData;
		public uint flags;
		public uint time;
		public IntPtr dwExtraInfo;
	}
}
