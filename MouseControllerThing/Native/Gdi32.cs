﻿using System.Runtime.InteropServices;

namespace MouseControllerThing.Native;

public static class Gdi32 {
	[DllImport("gdi32.dll", SetLastError = true)]
	public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
}
