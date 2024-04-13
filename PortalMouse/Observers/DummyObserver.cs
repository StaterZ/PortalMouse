using PortalMouse.Utils;
using PortalMouse.Utils.Math;

namespace PortalMouse.Observers;

public class DummyObserver : MouseObserver
{
	private readonly V2I[] m_dummyTable;

	private readonly Thread m_thread;
	private AtomicBool m_isRunning = new(true);
	private int m_dummyIndex = 0;

	public DummyObserver(Func<V2I, V2I?> callback, V2I[] dummyTable) : base(callback)
	{
		m_dummyTable = dummyTable;

		m_thread = new Thread(PollLoop);
		m_thread.Name = nameof(PollObserver);
		m_thread.Start();
	}

	private void PollLoop()
	{
		while (m_isRunning.Value)
		{
			if (m_dummyIndex >= m_dummyTable.Length)
			{
				using (new FgScope(ConsoleColor.Cyan))
				{
					Console.WriteLine("Dummy routine success!");
				}

				m_isRunning.Value = false;
				break;
			}

			m_callback(m_dummyTable[m_dummyIndex]);
			m_dummyIndex++;
		}
	}

	protected override void ReleaseUnmanagedResources()
	{
		m_isRunning.Value = false;
		m_thread.Join();
	}
}