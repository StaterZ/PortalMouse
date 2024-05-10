using System.Runtime.InteropServices;

namespace PortalMouse.Native;

internal static class Kernel32 {
	private const string dllName = "kernel32.dll";

	/// <summary>
	/// <see href="">https://learn.microsoft.com/en-us/windows/console/getconsolewindow</see>
	/// </summary>
	[DllImport(dllName, SetLastError = true)]
	public static extern IntPtr GetConsoleWindow();

	/*
	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-getmodulehandlew"></see>
	/// </summary>
	[DllImport(dllName, SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern IntPtr GetModuleHandle(string lpModuleName);
	*/
}
