using System.Runtime.InteropServices;

namespace PortalMouse.Engine.Native;

internal static class Shcore {
	private const string dllName = "Shcore.dll";

	/// <summary>
	/// <see href="">https://learn.microsoft.com/en-us/windows/win32/api/shellscalingapi/nf-shellscalingapi-setprocessdpiawareness</see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	public static extern IntPtr SetProcessDpiAwareness(ProcessDpiAwareness value);

}