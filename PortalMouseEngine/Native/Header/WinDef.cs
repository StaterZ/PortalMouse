using System.Runtime.InteropServices;
using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Native;

internal static class WinDef {
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

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-pointl"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct POINTL {
		private int x;
		private int y;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-rect"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
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

		public override readonly string ToString() => $"[X:{Left},Y:{Top},W:{Right - Left},H:{Bottom - Top}]";

		public static explicit operator Rect(R2I other) => new(
			other.Pos.x,
			other.Pos.y,
			other.Pos.x + other.Size.x,
			other.Pos.y + other.Size.y
		);

		public static explicit operator R2I(Rect rect) => new(
			new V2I(rect.Left, rect.Top),
			new V2I(rect.Right - rect.Left, rect.Bottom - rect.Top)
		);
	}
}
