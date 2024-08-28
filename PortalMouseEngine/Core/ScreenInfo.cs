using PortalMouse.Engine.Native;
using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

internal readonly record struct ScreenInfo(User32.MonitorInfoEx MonitorInfo, Frac Scale);
