using MouseControllerThing.Utils.Maths;

namespace MouseControllerThing.Hooking {
	public abstract class MouseObserver : IDisposable {
		protected readonly Func<V2I, V2I?> m_callback;

		public MouseObserver(Func<V2I, V2I?> callback) {
			m_callback = callback;
		}

		~MouseObserver() {
			ReleaseUnmanagedResources();
		}

		protected abstract void ReleaseUnmanagedResources();

		public void Dispose() {
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}
	}
}