using PortalMouse.Engine.Utils.Math;

namespace UnitTests;

[TestClass]
public class AlongEdge {

	[TestMethod]
	public void NegX() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+100, +0), null),
			(new V2I(+0, +0), null)
		);
	}

	[TestMethod]
	public void PosX() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+0, +0), null),
			(new V2I(+100, +0), null)
		);
	}

	[TestMethod]
	public void NegY() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+0, +100), null),
			(new V2I(+0, +0), null)
		);
	}

	[TestMethod]
	public void PosY() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+0, +0), null),
			(new V2I(+0, +100), null)
		);
	}
}
