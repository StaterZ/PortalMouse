using PortalMouse.Utils.Math;
using PortalMouse.Utils.Misc;

namespace PortalMouse.Observers;

public class DummyObserver : MouseObserver {
	private readonly V2I[] m_moves;

	private readonly Thread m_thread;
	private AtomicBool m_isRunning = new(true);
	private int m_index = 0;

	public DummyObserver(Func<V2I, V2I?> callback, V2I[] moves) : base(callback) {
		m_moves = moves;

		m_thread = new Thread(PollLoop) {
			Name = nameof(DummyObserver)
		};
		m_thread.Start();
	}

	private void PollLoop() {
		while (m_isRunning.Value) {
			if (m_index >= m_moves.Length) {
				Terminal.Imp("Dummy routine success!");

				m_isRunning.Value = false;
				break;
			}

			m_callback(m_moves[m_index]);
			m_index++;
		}
	}

	protected override void ReleaseUnmanagedResources() {
		m_isRunning.Value = false;
		m_thread.Join();

		base.ReleaseUnmanagedResources();
	}
}
