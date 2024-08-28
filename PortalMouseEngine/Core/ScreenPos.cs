using PortalMouse.Utils.Math;

namespace PortalMouse.Core;

public readonly record struct ScreenPos(V2I Pos, Screen Screen) {
	public readonly override string? ToString() => $"S{Screen.Id}:{Pos}";
}
