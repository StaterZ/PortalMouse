using PortalMouse.Utils.Math;
using System.Runtime.InteropServices;

namespace PortalMouse.Native;

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-point"></see>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Point {
	public int x;
	public int y;

	public Point(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static explicit operator Point(V2I point) => new(point.x, point.y);
	public static explicit operator V2I(Point point) => new(point.x, point.y);
}
