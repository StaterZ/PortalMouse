namespace PortalMouse.Utils;

public class FgScope : IDisposable {
	private readonly ConsoleColor m_prev;

	public FgScope(ConsoleColor color) {
		m_prev = Console.ForegroundColor;
		Console.ForegroundColor = color;
	}

	public void Dispose() {
		Console.ForegroundColor = m_prev;
	}
}
