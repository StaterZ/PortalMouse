using PortalMouse.Utils.Math;
using PortalMouse.Utils.Misc;

namespace PortalMouse.Observers;

public class PollObserver : MouseObserver
{
	private AtomicBool m_isRunning = new(true);
	private Thread m_thread;

	public PollObserver(Func<V2I, V2I?> callback) : base(callback)
	{
		m_thread = new Thread(PollLoop)
		{
			Name = nameof(PollObserver)
		};
		m_thread.Start();
	}

	private void PollLoop()
	{
		while (m_isRunning.Value)
		{
			Thread.Sleep(1);

			V2I? movedPos = m_callback(NativeHelper.CursorPos);
			if (!movedPos.HasValue) continue;

			NativeHelper.CursorPos = movedPos.Value;
		}
	}

	protected override void ReleaseUnmanagedResources()
	{
		m_isRunning.Value = false;
		m_thread.Join();
	}
}