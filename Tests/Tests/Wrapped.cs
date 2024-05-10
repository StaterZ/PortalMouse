using PortalMouse.Utils.Math;

namespace UnitTests;

[TestClass]
public class Wrapped {
	[TestMethod]
	public void NegX() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+25, +100), null),
			(new V2I(-75, +100), new V2I(TestHelper.k_xMax - 74, +100))
		);
	}

	[TestMethod]
	public void PosX() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(TestHelper.k_xMax - 25, +100), null),
			(new V2I(TestHelper.k_xMax + 75, +100), new V2I(+74, +100))
		);
	}

	[TestMethod]
	public void NegY() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+100, +25), null),
			(new V2I(+100, -75), new V2I(+100, TestHelper.k_yMax - 74))
		);
	}

	[TestMethod]
	public void PosY() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+100, TestHelper.k_yMax - 25), null),
			(new V2I(+100, TestHelper.k_yMax + 75), new V2I(+100, +74))
		);
	}


	[TestMethod]
	public void Diagonal_NegNeg() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+25, +25), null),
			(new V2I(-75, -75), new V2I(TestHelper.k_xMax - 74, TestHelper.k_yMax - 74))
		);
	}

	[TestMethod]
	public void Diagonal_PosNeg() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(TestHelper.k_xMax - 25, +25), null),
			(new V2I(TestHelper.k_xMax + 75, -75), new V2I(+74, TestHelper.k_yMax - 74))
		);
	}

	[TestMethod]
	public void Diagonal_NegPos() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(+25, TestHelper.k_yMax - 25), null),
			(new V2I(-75, TestHelper.k_yMax + 75), new V2I(TestHelper.k_xSize - 75, +74))
		);
	}

	[TestMethod]
	public void Diagonal_PosPos() {
		TestHelper.RunTest(TestHelper.GetSetup(true),
			(new V2I(TestHelper.k_xMax - 25, TestHelper.k_yMax - 25), null),
			(new V2I(TestHelper.k_xMax + 75, TestHelper.k_yMax + 75), new V2I(+74, +74))
		);
	}
}
