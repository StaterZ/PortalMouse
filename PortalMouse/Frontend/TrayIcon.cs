using PortalMouse.Engine.Utils.Misc;
using PortalMouse.Love;
using System;
using System.Windows.Forms;

namespace PortalMouse.Frontend;

public class TrayIcon : IDisposable {
	private readonly NotifyIcon m_tray;

	public TrayIcon() {
		ContextMenuStrip strip = new();
		strip.Items.Add("Reload", null, static (sender, eventArgs) => Program.SwitchState(RunningState.Running));
		strip.Items.Add("Open Editor", null, static (sender, eventArgs) => {
			if (Program.Setup == null) {
				Terminal.Err("Can't open editor, daemon is not running!"); //TODO: this is a stupid limitation, let's remove it later
				return;
			}
			Editor.Open(Program.Setup);
		});
		strip.Items.Add("Show", null, static (sender, eventArgs) => NativeHelper.ShowConsole(true));
		strip.Items.Add("Hide", null, static (sender, eventArgs) => NativeHelper.ShowConsole(false));
		strip.Items.Add("Exit", null, static (sender, eventArgs) => Program.SwitchState(RunningState.Exit));

		m_tray = new() {
			Icon = Resources.Icon,
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
