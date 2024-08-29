using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Observers;

public abstract class MouseObserver : IDisposable {
	protected readonly Func<V2I, V2I?> m_callback;
	protected readonly Action<Exception> m_exceptionHandler;

	public MouseObserver(Func<V2I, V2I?> callback, Action<Exception> exceptionHandler) {
		m_callback = callback;
		m_exceptionHandler = exceptionHandler;
	}

	~MouseObserver() {
		ReleaseUnmanagedResources();
	}

	protected virtual void ReleaseUnmanagedResources() { }

	public void Dispose() {
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}
}
