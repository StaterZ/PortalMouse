using System.Runtime.InteropServices;

namespace MouseControllerThing.Native;

public static class Kernel32 {
	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern IntPtr GetConsoleWindow();

	[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	internal static extern IntPtr GetModuleHandle(string lpModuleName);
}
