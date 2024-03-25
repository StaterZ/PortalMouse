using System.Runtime.InteropServices;

namespace MouseControllerThing.Native;

public static class Kernel32 {
	[DllImport("kernel32.dll")]
	public static extern IntPtr GetConsoleWindow();

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern IntPtr GetModuleHandle(string lpModuleName);
}
