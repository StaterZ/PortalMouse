namespace PortalMouse.Engine.Native;

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/shellscalingapi/ne-shellscalingapi-process_dpi_awareness"></see>
/// </summary>
public enum ProcessDpiAwareness {
	ProcessDpiUnaware = 0,
	ProcessSystemDpiAware = 1,
	ProcessPerMonitorDpiAware = 2,
}
