using PortalMouse.Engine.Core;
using PortalMouse.Engine.Utils.Math;

namespace UnitTests;

[TestClass]
public class RealSetups {
	[TestMethod]
	public void CommonSetup() {
		Screen leftScreen = new(
			2,
			new R2I(new V2I(-1920, 0), new V2I(1920, 1080)),
			Frac.One
		);
		Screen mainScreen = new(
			1,
			new R2I(new V2I(0, 0), new V2I(2560, 1440)),
			Frac.One
		);
		Screen rightScreen = new(
			3,
			new R2I(new V2I(+2560, 0), new V2I(1920, 1080)),
			Frac.One
		);


		Setup setup = new();
		setup.Screens.Add(leftScreen);
		setup.Screens.Add(mainScreen);
		setup.Screens.Add(rightScreen);

		static EdgeSpan AutoEdge(Edge edge) => new(edge, new R1I(0, edge.Length));
		Portal.Bind(AutoEdge(leftScreen.Right), AutoEdge(mainScreen.Left));
		Portal.Bind(AutoEdge(mainScreen.Right), AutoEdge(rightScreen.Left));

		TestHelper.RunTest(setup,
			(new V2I(+0000 - 1000, 200), null),
			(new V2I(+2560 + 1000, 200), null) //we expect null here since even though it crosses screen borders, no offset is actually needed in the end
		);
	}

	[TestMethod]
	public void TvSetup() {
		Screen mainScreen = new(
			1,
			new R2I(new V2I(0, 0), new V2I(1920, 1080)),
			Frac.One
		);
		Screen tvScreen = new(
			2,
			new R2I(new V2I(1920, 104), new V2I(3840, 2160)),
			Frac.One
		);


		Setup setup = new();
		setup.Screens.Add(mainScreen);
		setup.Screens.Add(tvScreen);

		static EdgeSpan AutoEdge(Edge edge) => new(edge, new R1I(0, edge.Length));
		Portal.Bind(AutoEdge(tvScreen.Left), AutoEdge(mainScreen.Right));

		TestHelper.RunTest(setup,
			(new V2I(1900, 10), null),
			(new V2I(2000, 10), new V2I(2000, 124))
		);
	}
}
