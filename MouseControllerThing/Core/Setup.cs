using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public sealed class Setup {
	public List<Screen> screens = new();
	private Screen? m_prevScreen;

	public V2I? Handle(V2I p) {
		Screen? screen = FindCursorScreen(p);
		if (screen == null) return null;

		p = screen.FromLogicalToPhysicalSpace(p);
		V2I? result = (m_prevScreen ?? screen).Handle(p);
		m_prevScreen = screen;
		if (!result.HasValue) return null;

		m_prevScreen = null;
		return screen.FromPhysicalToLogicalSpace(result.Value);
	}

	private Screen? FindCursorScreen(V2I cur) {
		foreach (Screen screen in screens) {
			if (!screen.LogicalRect.Contains(cur)) continue;
			return screen;
		}
		return null;
	}
}
