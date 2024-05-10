using PortalMouse.Utils.Math;

namespace UnitTests;

[TestClass]
public class Normal {
	[TestMethod]
	public void NegX() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+200, +100), null),
			(new V2I(+100, +100), null)
		);
	}

	[TestMethod]
	public void PosX() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+100, +100), null),
			(new V2I(+200, +100), null)
		);
	}

	[TestMethod]
	public void NegY() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+100, +200), null),
			(new V2I(+200, +100), null)
		);
	}

	[TestMethod]
	public void PosY() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+100, +100), null),
			(new V2I(+100, +200), null)
		);
	}
}
