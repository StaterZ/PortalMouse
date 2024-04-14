namespace PortalMouse.Utils.Misc;

public struct AtomicBool {
	private int m_value = 0;

	public AtomicBool(bool value) => Value = value;

	public bool Value {
		get => Interlocked.CompareExchange(ref m_value, 1, 1) == 1;
		set {
			if (value) {
				Interlocked.CompareExchange(ref m_value, 1, 0);
			} else {
				Interlocked.CompareExchange(ref m_value, 0, 1);
			}
		}
	}
}