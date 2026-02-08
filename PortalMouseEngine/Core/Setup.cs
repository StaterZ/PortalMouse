using System.Collections.Generic;
using System.Linq;
using PortalMouse.Engine.Utils.Ext;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Core;

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

		ScreenLineSeg move = new(new LineSeg2Frac(m_prevPos.Value.Pos, pos), m_prevPos.Value.Screen);
		List<ScreenLineSeg> moves = [move];
		while (!((R2Frac)move.Screen.LogicalRect - V2Frac.Half).Contains(move.Line.End)) {
			ScreenLineSeg? nextMove = move.Screen.TryHandle(move.Line);
			if (!nextMove.HasValue) throw new UnreachableException($"If we're outside the screen bounds (checked by the while) we should get a move adjustment. move was:\n{moves.Delimit(",\n")}");

			move = nextMove.Value;
			moves.Add(move);
		}
		m_prevPos = move.End;

		V2I end = (V2I)move.Line.End.Clamp(move.Screen.LogicalRect);
		
		return end != pos ? end : null;
	}

	private Screen? FindCursorScreen(V2I pos) =>
		Screens.FirstOrDefault(screen =>
			screen.LogicalRect.Contains(pos));

	public static Setup ConstructLocalSetup() {
		Setup setup = new();
		foreach (ScreenDesc screenDesc in NativeHelper.EnumScreenDescs()) {
			setup.Screens.Add(new Screen(screenDesc));
		}
		return setup;
	}
}
