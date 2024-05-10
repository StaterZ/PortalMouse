using PortalMouse.Native;
using PortalMouse.Utils.Math;

namespace PortalMouse.Core;

internal readonly record struct ScreenInfo(User32.MonitorInfoEx MonitorInfo, Frac Scale);
