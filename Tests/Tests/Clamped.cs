using PortalMouse.Engine.Utils.Math;

namespace UnitTests;

[TestClass]
public class Clamped {
	[TestMethod]
	public void NegX() {
		TestHelper.RunTest(TestHelper.GetSetup(false),
			(new V2I(+25, +100), null),
			(new V2I(-75, +100), new V2I(+0, +100))
		);
	}

	[TestMethod]
	public void PosX() {
		TestHelper.RunTest(TestHelper.GetSetup(false),
			(new V2I(TestHelper.k_xMax - 25, +100), null),
			(new V2I(TestHelper.k_xMax + 75, +100), new V2I(TestHelper.k_xMax, +100))
		);
	}

	[TestMethod]
	public void NegY() {
		TestHelper.RunTest(TestHelper.GetSetup(false),
			(new V2I(+100, +25), null),
			(new V2I(+100, -75), new V2I(+100, +0))
		);
	}

	[TestMethod]
	public void PosY() {
		TestHelper.RunTest(TestHelper.GetSetup(false),
			(new V2I(+100, TestHelper.k_yMax - 25), null),
			(new V2I(+100, TestHelper.k_yMax + 75), new V2I(+100, TestHelper.k_yMax))
		);
	}

	[TestMethod]
	public void Diagonal_NegNeg() {
		TestHelper.RunTest(TestHelper.GetSetup(false),
			(new V2I(+25, +25), null),
			(new V2I(-75, -75), new V2I(+0, +0))
		);
	}

	[TestMethod]
	public void Diagonal_PosNeg() {
		TestHelper.RunTest(TestHelper.GetSetup(false),
			(new V2I(TestHelper.k_xMax - 25, +25), null),
			(new V2I(TestHelper.k_xMax + 75, -75), new V2I(TestHelper.k_xMax, +0))
		);
	}

	[TestMethod]
	public void Diagonal_NegPos() {
		TestHelper.RunTest(TestHelper.GetSetup(false),
			(new V2I(+25, TestHelper.k_yMax - 25), null),
			(new V2I(-75, TestHelper.k_yMax + 75), new V2I(0, TestHelper.k_yMax))
		);
	}

	[TestMethod]
	public void Diagonal_PosPos() {
		TestHelper.RunTest(TestHelper.GetSetup(false),
			(new V2I(TestHelper.k_xMax - 25, TestHelper.k_yMax - 25), null),
			(new V2I(TestHelper.k_xMax + 75, TestHelper.k_yMax + 75), new V2I(TestHelper.k_xMax, TestHelper.k_yMax))
		);
	}
}
