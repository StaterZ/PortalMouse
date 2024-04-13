using MouseControllerThing.Utils.Maths;

namespace MouseControllerThing.Core;

public readonly record struct ScreenLineSeg(LineSeg2I Line, Screen Screen) {
	public ScreenPos Begin => new(Line.Begin, Screen);
	public ScreenPos End => new(Line.End, Screen);
}
