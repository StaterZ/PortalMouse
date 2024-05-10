using PortalMouse.Core;
using PortalMouse.Native;
using PortalMouse.Utils.Math;

namespace UnitTests;

public static class TestHelper {
	public const int k_xSize = 2560;
	public const int k_ySize = 1440;
	public const int k_xMax = k_xSize - 1;
	public const int k_yMax = k_ySize - 1;

	public static Setup GetSetup(bool shouldWrap) {
		Screen mainScreen = new(new ScreenInfo(
			new User32.MonitorInfoEx() {
				rcMonitor = new User32.Rect(0, 0, k_xSize, k_ySize),
				rcWork = new User32.Rect(0, 0, k_xSize, k_ySize),
				dwFlags = User32.MONITORINFOF_PRIMARY,
				szDevice = @"\\.\DISPLAY1",
			},
			Frac.One
		));


		Setup setup = new();
		setup.Screens.Add(mainScreen);

		if (shouldWrap) {
			static EdgeSpan AutoEdge(Edge edge) => new(edge, new R1I(0, edge.Length + 1));

			Portal.Bind(AutoEdge(mainScreen.Left), AutoEdge(mainScreen.Right));
			Portal.Bind(AutoEdge(mainScreen.Top), AutoEdge(mainScreen.Bottom));
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
