using PortalMouse.Engine.Core;
using PortalMouse.Engine.Utils.Math;

namespace UnitTests;

[TestClass]
public class RealSetups {
	[TestMethod]
	public void CommonSetup() {
		Setup setup = new();

		Screen leftScreen = new(
			setup,
			2,
			new R2I(new V2I(-1920, 0), new V2I(1920, 1080)),
			Frac.One
		);
		Screen mainScreen = new(
			setup,
			1,
			new R2I(new V2I(0, 0), new V2I(2560, 1440)),
			Frac.One
		);
		Screen rightScreen = new(
			setup,
			3,
			new R2I(new V2I(+2560, 0), new V2I(1920, 1080)),
			Frac.One
		);

		Portal.Bind(TestHelper.AutoPortal(leftScreen.Right), TestHelper.AutoPortal(mainScreen.Left));
		Portal.Bind(TestHelper.AutoPortal(mainScreen.Right), TestHelper.AutoPortal(rightScreen.Left));

		TestHelper.RunTest(setup,
			(new V2I(+0000 - 1000, 200), null),
			(new V2I(+2560 + 1000, 200), null) //we expect null here since even though it crosses screen borders, no offset is actually needed in the end
		);
	}

	[TestMethod]
	public void TvSetup() {
		Setup setup = new();

		Screen mainScreen = new(
			setup,
			1,
			new R2I(new V2I(0, 0), new V2I(1920, 1080)),
			Frac.One
		);
		Screen tvScreen = new(
			setup,
			2,
			new R2I(new V2I(1920, 104), new V2I(3840, 2160)),
			Frac.One
		);

		Portal.Bind(TestHelper.AutoPortal(tvScreen.Left), TestHelper.AutoPortal(mainScreen.Right));

		TestHelper.RunTest(setup,
			(new V2I(1900, 10), null),
			(new V2I(2000, 10), new V2I(2000, 124))
		);
	}
}
