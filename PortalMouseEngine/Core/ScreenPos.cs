using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

public readonly record struct ScreenPos(V2I Pos, Screen Screen) {
	public readonly override string? ToString() => $"S{Screen.Id}:{Pos}";
}
