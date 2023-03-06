using System.Drawing;
using System.Runtime.InteropServices;

namespace MouseControllerThing.Utils;

public static class Native {
    public const int MONITOR_DEFAULTTOPRIMERTY = 0x00000001;
    public const int MONITOR_DEFAULTTONEAREST = 0x00000002;

    public delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

    [DllImport("user32.dll")]
    public static extern bool GetMonitorInfo(IntPtr hMonitor, MonitorInfo lpmi);

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point pos);

    [Serializable, StructLayout(LayoutKind.Sequential)]
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

        public override string ToString() {
            return $"[X:{Left},Y:{Top},W:{Right - Left},H:{Bottom - Top}]";
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class MonitorInfo {
        public int Size = Marshal.SizeOf(typeof(MonitorInfo));
        public Rect Monitor;
        public Rect Work;
        public int Flags;
    }
}
