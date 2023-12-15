using MouseControllerThing.Native;
using MouseControllerThing.Utils;

namespace MouseControllerThing.Core;

public sealed class Setup {
	public readonly List<Screen> m_screens = new();
	private Screen? m_prevScreen;

	public V2I? Handle(V2I p) {
		Screen? screen = FindCursorScreen(p);
		if (screen == null) return null;

		p = screen.FromLogicalToPhysicalSpace_Pos(p);
		V2I? movedP = (m_prevScreen ?? screen).Handle(p);
		m_prevScreen = screen;
		if (!movedP.HasValue) return null;

		Screen? outScreen = FindCursorScreen(movedP.Value);
		User32.Rect rect = new(outScreen!.LogicalRect);
		User32.ClipCursor(ref rect);

		m_prevScreen = null;
		return movedP.Value;
	}

	private Screen? FindCursorScreen(V2I p) => m_screens.FirstOrDefault(screen => screen.LogicalRect.Contains(p));
}
