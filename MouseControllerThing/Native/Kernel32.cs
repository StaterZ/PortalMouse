using System.Runtime.InteropServices;

namespace MouseControllerThing.Native;

public static class Kernel32 {
	[DllImport("kernel32.dll")]
	public static extern IntPtr GetConsoleWindow();
}
