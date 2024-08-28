using PortalMouse.Utils.Math;

namespace PortalMouse.Core;

public readonly record struct ScreenLineSeg(LineSeg2I Line, Screen Screen) {
	public ScreenPos Begin => new(Line.Begin, Screen);
	public ScreenPos End => new(Line.End, Screen);

	public readonly override string ToString() => $"{Begin}->{End}";
}
