using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Observers;

public class PollObserver : MouseObserver {
	private AtomicBool m_isRunning = new(true);
	private readonly Thread m_thread;

	public PollObserver(Func<V2I, V2I?> callback) : base(callback) {
		m_thread = new Thread(PollLoop) {
			Name = nameof(PollObserver)
		};
		m_thread.Start();
	}

	private void PollLoop() {
		while (m_isRunning.Value) {
			Thread.Sleep(1);

			V2I? movedPos = m_callback(NativeHelper.CursorPos);
			if (!movedPos.HasValue) continue;

			NativeHelper.CursorPos = movedPos.Value;
		}
	}

	protected override void ReleaseUnmanagedResources() {
		m_isRunning.Value = false;
		m_thread.Join();

		base.ReleaseUnmanagedResources();
	}
}