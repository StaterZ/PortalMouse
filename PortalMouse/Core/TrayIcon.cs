using PortalMouse.Utils.Misc;

namespace PortalMouse.Core;

public class TrayIcon : IDisposable {
	private readonly NotifyIcon m_tray;

	public TrayIcon() {
		ContextMenuStrip strip = new();
		strip.Items.Add("Show", null, (sender, eventArgs) => NativeHelper.ShowConsole(true));
		strip.Items.Add("Hide", null, (sender, eventArgs) => NativeHelper.ShowConsole(false));
		strip.Items.Add("Reload", null, (sender, eventArgs) => Program.UpdateState(RunningState.Restart));
		strip.Items.Add("Exit", null, (sender, eventArgs) => Program.UpdateState(RunningState.Exit));

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
