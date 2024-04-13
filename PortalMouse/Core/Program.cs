using PortalMouse.Observers;
using PortalMouse.Utils;
using PortalMouse.Utils.Math;
using System.IO;
using System.Text.Json;

namespace PortalMouse.Core;

public static class Program {
	private static RunningState s_runningState = RunningState.Halted;

	[STAThread]
	public static void Main(string[] args) {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		ContextMenuStrip strip = new();
		strip.Items.Add("Show", null, (sender, eventArgs) => NativeWrapper.ShowConsole(true));
		strip.Items.Add("Hide", null, (sender, eventArgs) => NativeWrapper.ShowConsole(false));
		strip.Items.Add("Reload", null, (sender, eventArgs) => UpdateState(RunningState.Restart));
		strip.Items.Add("Exit", null, (sender, eventArgs) => UpdateState(RunningState.Exit));
		using TrayIcon tray = new(strip);

		Runtime();
	}

	private static void UpdateState(RunningState state) {
		s_runningState = state;
		Application.Exit();
	}

	private static void Runtime() {
		s_runningState = RunningState.Running;

		while (true) {
			switch (s_runningState) {
				case RunningState.Running:
					try {
						Run();
					} catch (Exception ex) {
						s_runningState = RunningState.Halted;
						using (new FgScope(ConsoleColor.Red)) {
							NativeWrapper.ShowConsole(true);
							Console.WriteLine(ex);
						}
					}
					break;
				case RunningState.Restart:
					s_runningState = RunningState.Running;
					break;
				case RunningState.Halted:
					NativeWrapper.ShowConsole(true);
					Console.WriteLine("Program has halted! (restart? [y/n])");
					s_runningState = ReadYN() ? RunningState.Restart : RunningState.Exit;
					break;
				case RunningState.Exit:
					return;
			}
		}
	}

	private static bool ReadYN() {
		while (true) {
			switch (Console.ReadKey().Key) {
				case ConsoleKey.Y:
					return true;
				case ConsoleKey.N:
					return false;
			}
		}
	}

	private static void Run() {
		NativeWrapper.ShowConsole(true);
		Console.Clear();

		Setup? setup = LoadSetup();
		if (setup == null) {
			Console.WriteLine("Failed to load setup!"); //TODO: not the best error handling ever... really should fix this...
			return;
		}

		using (new FgScope(ConsoleColor.Green)) {
			Console.WriteLine("Entering Runtime...");
		}
#if !DEBUG
		Thread.Sleep(500);
		NativeWrapper.ShowConsole(false);
#endif
		V2I? MoveHandler(V2I pos) {
#if DEBUG
			Console.WriteLine($"Pos: {pos}");
#endif

			V2I? movedPos = setup!.Handle(pos);
#if DEBUG
			if (movedPos.HasValue) {
				Console.WriteLine($"Moved: {pos} -> {movedPos.Value}");
			}
#endif

			return movedPos;
		}

		MouseObserver mouseObserver = CreateObserver(MoveHandler);

		Application.Run();

		mouseObserver.Dispose();
	}

	private static MouseObserver CreateObserver(Func<V2I, V2I?> MoveHandler) {
		return new LLMHookObserver(MoveHandler);
		//return MouseObserver mouseObserver = new PollObserver(MoveHandler);
		//return MouseObserver mouseObserver = new DummyObserver(MoveHandler, new V2I[] {
		//	new V2I(25, 25),
		//	new V2I(-75, -75),

		//	//-X
		//	new V2I(25, 100),
		//	new V2I(-75, 100),

		//	//+X
		//	new V2I(2560 - 25, 100),
		//	new V2I(2560 + 75, 100),

		//	//-Y
		//	new V2I(100, 25),
		//	new V2I(100, -75),

		//	//+Y
		//	new V2I(100, 1440 - 25),
		//	new V2I(100, 1440 + 75),
		//});
	}

	private static Setup? LoadSetup() {
		Setup setup = new();
		Console.WriteLine("Screens:");
		foreach (ScreenInfo monitor in NativeWrapper.GetDisplays()) {
			Console.Write($"    {monitor.Id}: {monitor.PhysicalRect}");
			if (monitor.Scale != Frac.One) {
				Console.Write($" @ {(float)monitor.Scale * 100}%");
#if DEBUG
				Console.Write($" -> Logical:{monitor.LogicalRect}");
#endif
			}
			Console.WriteLine();

			Screen screen = new(monitor);
			setup.Screens.Add(screen);
		}
		Console.WriteLine();

		Console.WriteLine("Loading Config...");
		Config? config = LoadConfig("config.json");
		if (config == null) return null;
		if (!LoadPortals(config, setup)) return null;
		Console.WriteLine("Config Loaded!");
		Console.WriteLine();

		return setup;
	}

	private static bool LoadPortals(Config config, Setup setup) {
		if (config.Mappings.Length <= 0) {
			using (new FgScope(ConsoleColor.Yellow)) {
				Console.WriteLine("No mappings present in config!");
			}
			return true;
		}

		foreach (Config.Mapping mapping in config.Mappings) {
			bool TryParseScreen(int screenId, out Screen screen) {
				if (screenId < 0 && screenId >= setup.Screens.Count) {
					using (new FgScope(ConsoleColor.Red)) {
						Console.WriteLine($"Screen id out of range. '{screenId}' supplied, but valid range is 0-{setup.Screens.Count - 1}, aborting");
					}
					screen = default!;
					return false;
				}
				screen = setup.Screens.Single(screen => screen.Id == screenId);
				return true;
			}

			bool TryParseRange(Config.EdgeRange edgeRange, Edge edge, out R1I range) {
				bool TryParseAnchor(string anchorStr, Edge edge, out int anchor) {
					R1I pixelRange = new(0, edge.Length);
					R1I percentRange = new(0, 100);

					if (anchorStr.EndsWith("px")) {
						if (!int.TryParse(anchorStr[..^2], out int value)) {
							using (new FgScope(ConsoleColor.Red)) {
								Console.WriteLine($"Failed to parse anchor. '{anchorStr}' supplied, but int is malformed");
							}
							anchor = default;
							return false;
						}

						if (
							value < pixelRange.Begin ||
							value > pixelRange.End
						) {
							using (new FgScope(ConsoleColor.Red)) {
								Console.WriteLine($"Anchor is out of range. '{anchorStr}' supplied, but valid range is {pixelRange.Begin}px-{pixelRange.End}px");
							}

							anchor = default;
							return false;
						}

						anchor = value;
						return true;
					}
					if (anchorStr.EndsWith("%")) {
						if (!int.TryParse(anchorStr[..^1], out int value)) {
							using (new FgScope(ConsoleColor.Red)) {
								Console.WriteLine($"Failed to parse anchor. '{anchorStr}' supplied, but int is malformed");
							}

							anchor = default;
							return false;
						}

						if (
							value < percentRange.Begin || 
							value > percentRange.End
						) {
							using (new FgScope(ConsoleColor.Red)) {
								Console.WriteLine($"Anchor is out of range. '{anchorStr}' supplied, but valid range is {percentRange.Begin}%-{percentRange.End}%");
							}

							anchor = default;
							return false;
						}

						anchor = MathX.Map(value, percentRange, pixelRange);
						return true;
					}

					anchor = default;
					return false;
				}

				const string beginDefault = "0%";
				const string endDefault = "100%";
				if (!TryParseAnchor(edgeRange.Begin ?? beginDefault, edge, out int begin)) return false;
				if (!TryParseAnchor(edgeRange.End ?? endDefault, edge, out int end)) return false;
				
				range = new R1I(begin, end);
				return true;
			}

			if (!TryParseScreen(mapping.A.Screen, out Screen aScreen)) return false;
			Edge aEdge = aScreen.GetEdge(mapping.A.Side);
			if (!TryParseRange(mapping.A, aEdge, out R1I aRange)) return false;

			if (!TryParseScreen(mapping.B.Screen, out Screen bScreen)) return false;
			Edge bEdge = bScreen.GetEdge(mapping.B.Side);
			if (!TryParseRange(mapping.B, bEdge, out R1I bRange)) return false;

			Console.WriteLine($"Mapping 'screen{mapping.A.Screen} {aEdge.Side} [{aRange.Begin}-{aRange.End}]' to 'screen{mapping.B.Screen} {bEdge.Side} [{bRange.Begin}-{bRange.End}]'");
			Portal.Bind(
				new EdgeSpan(aEdge, aRange),
				new EdgeSpan(bEdge, bRange)
			);
		}

		return true;
	}

	private static Config? LoadConfig(string path) {
		if (!File.Exists(path)) {
			using (new FgScope(ConsoleColor.Red)) {
				Console.WriteLine($"'{path}' not found, aborting");
			}
			return null;
		}

		string configText = File.ReadAllText(path);
		Config? config = JsonSerializer.Deserialize<Config>(configText);
		if (config == null) {
			using (new FgScope(ConsoleColor.Red)) {
				Console.WriteLine("Failed to parse config, aborting");
			}
			return null;
		}

		return config;
	}
}
