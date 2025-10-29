using System.Runtime.InteropServices;
using static PortalMouse.Engine.Native.WinDef;
using static PortalMouse.Engine.Native.WinError;

namespace PortalMouse.Engine.Native;

internal static class User32 {
	private const string dllName = "user32.dll";

	public const int MONITOR_DEFAULTTOPRIMARY = 0x00000001;
	public const int MONITOR_DEFAULTTONEAREST = 0x00000002;

	public const int SW_HIDE = 0;
	public const int SW_SHOW = 5;

	public const int WhMouseLl = 14;
	public const int WmMouseMove = 0x0200;

	public const int CCHDEVICENAME = 32;

	public const int MONITORINFOF_PRIMARY = 1;

	public const int VK_LBUTTON = 1;
	public const int VK_RBUTTON = 2;
	public const int VK_MBUTTON = 4;

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
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaydevicesw"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnumDisplayDevicesW(
		string lpDevice,
		uint iDevNum,
		ref Gdi32.DISPLAY_DEVICEW lpDisplayDevice,
		uint dwFlags
	);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmonitorinfow"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEXW lpmi);

	/*
	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfromwindow"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr MonitorFromWindow(IntPtr hWnd, int dwFlags);
	*/

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcursorpos"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetCursorPos(int X, int Y);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorpos"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetCursorPos(out Point lpPoint);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr SetWindowsHookEx(HookType idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-unhookwindowshookex"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool UnhookWindowsHookEx(IntPtr hhk);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-callnexthookex"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdc"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr GetDC(IntPtr zero);

	/*
	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-clipcursor"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ClipCursor(ref Rect lpRect);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-clipcursor"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ClipCursor(IntPtr lpRect);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclipcursor"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetClipCursor(out Rect lpRect);
	*/

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getasynckeystate"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern short GetAsyncKeyState(int vKey);


	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdisplayconfigbuffersizes"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern ExitCode GetDisplayConfigBufferSizes(
		Gdi32.QUERY_DEVICE_CONFIG_FLAGS flags,
		out uint numPathArrayElements,
		out uint numModeInfoArrayElements
	);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-querydisplayconfig"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern ExitCode QueryDisplayConfig(
		Gdi32.QUERY_DEVICE_CONFIG_FLAGS flags,
		ref uint numPathArrayElements,
		[Out] Gdi32.DISPLAYCONFIG_PATH_INFO[] PathInfoArray,
		ref uint numModeInfoArrayElements,
		[Out] Gdi32.DISPLAYCONFIG_MODE_INFO[] ModeInfoArray,
		IntPtr currentTopologyId
	);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern ExitCode DisplayConfigGetDeviceInfo(ref Gdi32.DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName);

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-monitorinfoexw"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct MONITORINFOEXW {
		public int cbSize = Marshal.SizeOf<MONITORINFOEXW>();
		public Rect rcMonitor;
		public Rect rcWork;
		public int dwFlags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
		public string szDevice;

		public MONITORINFOEXW() {
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
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct MSLLHOOKSTRUCT {
		public Point pt;
		public uint mouseData;
		public uint flags;
		public uint time;
		public IntPtr dwExtraInfo;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexw"></see>
	/// </summary>
	internal enum HookType : int {
		/// <summary>
		/// Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box,
		/// message box, menu, or scroll bar. For more information, see the MessageProc hook procedure.
		/// </summary>
		WH_MSGFILTER = -1,
		/// <summary>
		/// Installs a hook procedure that records input messages posted to the system message queue. This hook is
		/// useful for recording macros. For more information, see the JournalRecordProc hook procedure.
		/// </summary>
		WH_JOURNALRECORD = 0,
		/// <summary>
		/// Installs a hook procedure that posts messages previously recorded by a WH_JOURNALRECORD hook procedure.
		/// For more information, see the JournalPlaybackProc hook procedure.
		/// </summary>
		WH_JOURNALPLAYBACK = 1,
		/// <summary>
		/// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc
		/// hook procedure.
		/// </summary>
		WH_KEYBOARD = 2,
		/// <summary>
		/// Installs a hook procedure that monitors messages posted to a message queue. For more information, see the
		/// GetMsgProc hook procedure.
		/// </summary>
		WH_GETMESSAGE = 3,
		/// <summary>
		/// Installs a hook procedure that monitors messages before the system sends them to the destination window
		/// procedure. For more information, see the CallWndProc hook procedure.
		/// </summary>
		WH_CALLWNDPROC = 4,
		/// <summary>
		/// Installs a hook procedure that receives notifications useful to a CBT application. For more information,
		/// see the CBTProc hook procedure.
		/// </summary>
		WH_CBT = 5,
		/// <summary>
		/// Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box,
		/// message box, menu, or scroll bar. The hook procedure monitors these messages for all applications in the
		/// same desktop as the calling thread. For more information, see the SysMsgProc hook procedure.
		/// </summary>
		WH_SYSMSGFILTER = 6,
		/// <summary>
		/// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook
		/// procedure.
		/// </summary>
		WH_MOUSE = 7,
		/// <summary>
		///
		/// </summary>
		WH_HARDWARE = 8,
		/// <summary>
		/// Installs a hook procedure useful for debugging other hook procedures. For more information, see the
		/// DebugProc hook procedure.
		/// </summary>
		WH_DEBUG = 9,
		/// <summary>
		/// Installs a hook procedure that receives notifications useful to shell applications. For more information,
		/// see the ShellProc hook procedure.
		/// </summary>
		WH_SHELL = 10,
		/// <summary>
		/// Installs a hook procedure that will be called when the application's foreground thread is about to become
		/// idle. This hook is useful for performing low priority tasks during idle time. For more information, see the
		/// ForegroundIdleProc hook procedure.
		/// </summary>
		WH_FOREGROUNDIDLE = 11,
		/// <summary>
		/// Installs a hook procedure that monitors messages after they have been processed by the destination window
		/// procedure. For more information, see the CallWndRetProc hook procedure.
		/// </summary>
		WH_CALLWNDPROCRET = 12,
		/// <summary>
		/// Installs a hook procedure that monitors low-level keyboard input events. For more information, see the
		/// LowLevelKeyboardProc hook procedure.
		/// </summary>
		WH_KEYBOARD_LL = 13,
		/// <summary>
		/// Installs a hook procedure that monitors low-level mouse input events. For more information, see the
		/// LowLevelMouseProc hook procedure.
		/// </summary>
		WH_MOUSE_LL = 14
	}
}
