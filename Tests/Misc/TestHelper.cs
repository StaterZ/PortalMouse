using PortalMouse.Engine.Core;
using PortalMouse.Engine.Utils.Math;

namespace UnitTests;

public static class TestHelper {
	public const int k_xSize = 2560;
	public const int k_ySize = 1440;
	public const int k_xMax = k_xSize - 1;
	public const int k_yMax = k_ySize - 1;

	public static PortalDesc AutoPortal(Edge edge) =>
		new(new EdgeRange(edge, new R1I(0, edge.ScreenRangeAlongEdgeAxis.Size)), 0);

	public static Setup GetSetup(bool shouldWrap) {
		Setup setup = new();
		Screen mainScreen = new(
			setup,
			1,
			new R2I(new V2I(0, 0), new V2I(k_xSize, k_ySize)),
			Frac.One,
			"Main"
		);

		if (shouldWrap) {
			Portal.Bind(AutoPortal(mainScreen.Left), AutoPortal(mainScreen.Right));
			Portal.Bind(AutoPortal(mainScreen.Top), AutoPortal(mainScreen.Bottom));
		}

		return setup;
	}

	public static void RunTest(Setup setup, params (V2I, V2I?)[] test) {
		foreach ((V2I inPos, V2I? expectedOutPos) in test) {
			V2I? movedPos = setup!.Handle(inPos);
			Assert.AreEqual(expectedOutPos, movedPos);
		}
	}
}
