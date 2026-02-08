using PortalMouse.Engine.Core;
using PortalMouse.Engine.Utils.Math;

namespace UnitTests;

[TestClass]
public class AxisChange {
	[TestMethod]
	public void RightToBottom() {
		Setup setup = new();

		Screen aScreen = new(
			1,
			new R2I(new V2I(0, 0), new V2I(2560, 1440)),
			Frac.One,
			"A"
		);
		setup.Screens.Add(aScreen);
		Screen bScreen = new(
			3,
			new R2I(new V2I(+2560, -1080), new V2I(1920, 1080)),
			Frac.One,
			"B"
		);
		setup.Screens.Add(bScreen);

		Portal.Bind(TestHelper.AutoPortal(aScreen.Right), TestHelper.AutoPortal(bScreen.Bottom));

		TestHelper.RunTest(setup,
			(new V2I(+2560-1, 200), null),
			(new V2I(+2560-1 + 1, 200), new V2I((int)(2560 + 200 * new Frac(1920, 1440) + new Frac(1, 2)), -1))
		);
	}
	
	[TestMethod]
	public void SimpleTurn() {
		Setup setup = new();

		Screen aScreen = new(
			1,
			new R2I(new V2I(0, 0), new V2I(100, 100)),
			Frac.One,
			"A"
		);
		setup.Screens.Add(aScreen);
		Screen bScreen = new(
			3,
			new R2I(new V2I(+100, -100), new V2I(100, 100)),
			Frac.One,
			"B"
		);
		setup.Screens.Add(bScreen);

		Portal.Bind(TestHelper.AutoPortal(aScreen.Right), TestHelper.AutoPortal(bScreen.Bottom));

		TestHelper.RunTest(setup,
			(new V2I(+100-1, 50), null),
			(new V2I(+100-1 + 1, 50), new V2I((int)(100 + 50 * new Frac(100, 100) + new Frac(1, 2)), -1))
		);
	}
	
	[TestMethod]
	public void SimpleNegX() {
		TestHelper.RunTest(TestHelper.CreateTestSetup(true),
			(new V2I(+0, +100), null),
			(new V2I(-1, +100), new V2I(TestHelper.k_xMax + 0, +100))
		);
	}
	
	[TestMethod]
	public void SimplePosX() {
		TestHelper.RunTest(TestHelper.CreateTestSetup(true),
			(new V2I(TestHelper.k_xMax - 0, +100), null),
			(new V2I(TestHelper.k_xMax + 1, +100), new V2I(+0, +100))
		);
	}
	
	[TestMethod]
	public void SimpleNegY() {
		TestHelper.RunTest(TestHelper.CreateTestSetup(true),
			(new V2I(+100, +0), null),
			(new V2I(+100, -1), new V2I(+100, TestHelper.k_yMax + 0))
		);
	}
	
	[TestMethod]
	public void SimplePosY() {
		TestHelper.RunTest(TestHelper.CreateTestSetup(true),
			(new V2I(+100, TestHelper.k_yMax - 0), null),
			(new V2I(+100, TestHelper.k_yMax + 1), new V2I(+100, +0))
		);
	}
}
