using MouseControllerThing.Utils;

namespace MouseControllerThing;

public sealed class Setup {
	public List<Screen> screens = new();

	public V2I? Handle(V2I p) {
		foreach (Screen screen in screens) {
			V2I? result = screen.Handle(p);
			if (result.HasValue) {
				return result.Value;
			}
		}
		return null;
	}
}
