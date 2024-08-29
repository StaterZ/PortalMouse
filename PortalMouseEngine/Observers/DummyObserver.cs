using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Observers;

public class DummyObserver : MouseObserver {
	private readonly IList<V2I> m_moves;

	private readonly Thread m_thread;
	private AtomicBool m_isRunning = new(true);
	private int m_index = 0;

	public DummyObserver(Func<V2I, V2I?> callback, IList<V2I> moves) : base(callback, (_) => { }) {
		m_moves = moves;

		m_thread = new Thread(PollLoop) {
			Name = nameof(DummyObserver)
		};
		m_thread.Start();
	}

	private void PollLoop() {
		while (m_isRunning.Value) {
			if (m_index >= m_moves.Count) {
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
