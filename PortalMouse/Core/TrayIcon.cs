namespace PortalMouse.Core;

public class TrayIcon : IDisposable {
	private readonly NotifyIcon m_tray;

	public TrayIcon(ContextMenuStrip strip) {
		m_tray = new() {
			Icon = Resources.trayIcon,
			Visible = true,
			Text = Application.ProductName,
			ContextMenuStrip = strip,
		};
	}

	~TrayIcon() {
		ReleaseUnmanagedResources();
	}

	private void ReleaseUnmanagedResources() {
		m_tray.Visible = false;
		m_tray.Dispose();
	}

	public void Dispose() {
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}
}
