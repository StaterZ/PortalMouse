using PortalMouse.Engine.Native;
using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

internal readonly record struct ScreenDesc(User32.MonitorInfoEx MonitorInfo, Frac Scale);
