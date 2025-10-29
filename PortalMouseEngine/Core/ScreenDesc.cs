using PortalMouse.Engine.Native;
using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

internal record struct ScreenDesc(
	User32.MONITORINFOEXW MonitorInfo,
	Frac Scale,
	string FriendlyName,
	Gdi32.DISPLAY_DEVICEW? DisplayDevice
);
