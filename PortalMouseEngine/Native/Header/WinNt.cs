using System.Runtime.InteropServices;

namespace PortalMouse.Engine.Native;

internal static class WinNt {
	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-luid"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct LUID {
		public uint LowPart;
		public int HighPart;
	}
}
