using MouseControllerThing.Utils;
using MouseControllerThing.Utils.Maths;

namespace MouseControllerThing.Core;

public sealed class Setup {
	public readonly List<Screen> Screens = new();
	private ScreenPos? m_prevMousePos;

	public V2I? Handle(V2I pos) {
		m_prevMousePos ??= new ScreenPos(pos, FindCursorScreen(pos) ?? throw new UnreachableException());

		while (!m_prevMousePos.Value.Screen.LogicalRect.Contains(pos)) {
			LineSeg2I line = new(m_prevMousePos.Value.Pos, pos);
			m_prevMousePos = m_prevMousePos.Value.Screen.Handle(line);
		}

		return pos;
	}

	private Screen? FindCursorScreen(V2I pos) =>
		Screens.FirstOrDefault(screen =>
			screen.LogicalRect.Contains(pos));
}
