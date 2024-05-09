using PortalMouse.Utils.Math;
using PortalMouse.Utils.Misc;

namespace PortalMouse.Core;

public sealed class Setup {
	public readonly List<Screen> Screens = new();
	private ScreenPos? m_prevPos;

	public V2I? Handle(V2I pos) {
		if (m_prevPos == null) {
			Screen? screen = FindCursorScreen(pos);
			if (screen == null) {
				Terminal.Wrn("Failed to find screen for cursor. Unless the error repeats it can be safely ignored");
				return null;
			}
			m_prevPos = new ScreenPos(pos, screen);
			return null;
		}

		if (pos == m_prevPos.Value.Pos) return null;

		ScreenLineSeg move = new(new LineSeg2I(m_prevPos.Value.Pos, pos), m_prevPos.Value.Screen);
		while (!move.Screen.LogicalRect.Contains(move.Line.End)) {

			ScreenLineSeg? nextMove = move.Screen.Handle(move.Line);
			if (!nextMove.HasValue) throw new UnreachableException("If we're outside the screen bounds (checked by the while) we should get to a new screen");

			move = nextMove.Value;
		}
		m_prevPos = move.End;

		return move.Line.End != pos ?
			move.Line.End :
			null;
	}

	private Screen? FindCursorScreen(V2I pos) =>
		Screens.FirstOrDefault(screen =>
			screen.LogicalRect.Contains(pos));
}
