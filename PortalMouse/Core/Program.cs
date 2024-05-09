using PortalMouse.Observers;
using PortalMouse.Utils.Math;
using PortalMouse.Utils.Misc;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PortalMouse.Core;

public static class Program {
	private static RunningState s_runningState = RunningState.Halted;

	[STAThread]
	private static void Main(string[] args) {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		using TrayIcon tray = new();

		Runtime();
	}

	public static void UpdateState(RunningState state) {
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
							Console.WriteLine(ex);
						}
#if DEBUG
						throw;
#endif
					}
					break;
				case RunningState.Restart:
					s_runningState = RunningState.Running;
					break;
				case RunningState.Halted:
					NativeHelper.ShowConsole(true);
					Terminal.Wrn("Program has halted! (restart? [y/n])");
					s_runningState = Terminal.ReadYN() ? RunningState.Restart : RunningState.Exit;
					break;
				case RunningState.Exit:
					return;
			}
		}
	}

	private static void Run() {
		NativeHelper.ShowConsole(true);
		Console.Clear();

		Setup? setup = LoadSetup();
		if (setup == null) {
			Terminal.Err("Failed to load setup!"); //TODO: not the best error handling ever... really should fix this...
			UpdateState(RunningState.Halted);
			return;
		}

		Terminal.Imp("Entering Runtime...");

#if !DEBUG
		Thread.Sleep(500);
		NativeHelper.ShowConsole(false);
#endif
		V2I? MoveHandler(V2I pos) {
#if DEBUG
			Terminal.Dbg($"Pos: {pos}");
#endif

			V2I? movedPos = setup!.Handle(pos);
#if DEBUG
			if (movedPos.HasValue) {
				Terminal.Dbg($"Moved: {pos} -> {movedPos.Value}");
			}
#endif

			return movedPos;
		}

		MouseObserver mouseObserver = CreateObserver(MoveHandler);

		Application.Run();

		mouseObserver.Dispose();
	}

	private static MouseObserver CreateObserver(Func<V2I, V2I?> MoveHandler) {
		//return new LLMHookObserver(MoveHandler);
		//return new PollObserver(MoveHandler);
		return new DummyObserver(MoveHandler, new V2I[] {
			//test scaled wrap
			new V2I(3000, 0),
			new V2I(3000, -1),

			//Diagonal
			new V2I(25, 25),
			new V2I(-75, -75),

			//-X
			new V2I(25, 100),
			new V2I(-75, 100),

			//+X
			new V2I(2560 - 25, 100),
			new V2I(2560 + 75, 100),

			//-Y
			new V2I(100, 25),
			new V2I(100, -75),

			//+Y
			new V2I(100, 1440 - 25),
			new V2I(100, 1440 + 75),
		});
	}

	private static Setup? LoadSetup() {
		Setup setup = new();
		Terminal.Inf("Screens:");
		StringBuilder builder = new();
		foreach (ScreenInfo monitor in NativeHelper.EnumDisplays()) {
			builder.Append($"    {monitor.Id}: {monitor.PhysicalRect}");
			if (monitor.Scale != Frac.One) {
				builder.Append($" @ {(float)monitor.Scale * 100}%");
#if DEBUG
				builder.Append($" -> Logical:{monitor.LogicalRect}");
#endif
			}
			Terminal.Inf(builder.ToString());
			builder.Clear();

			Screen screen = new(monitor);
			setup.Screens.Add(screen);
		}
		Terminal.BlankLine();

		if (setup.Screens.Count <= 0) {
			Terminal.Err("No screens found in setup? Something has gone terribly wrong!");
			return null;
		}

		Terminal.Inf("Loading Config...");
		Config? config = LoadConfig("config.json");
		if (config == null) return null;
		if (!LoadPortals(config, setup)) return null;
		Terminal.Inf("Config Loaded!");
		Terminal.BlankLine();

		return setup;
	}

	private static bool LoadPortals(Config config, Setup setup) {
		if (config.Mappings.Length <= 0) {
			Terminal.Wrn("No mappings present in config!");
			return true;
		}

		foreach (Config.Mapping mapping in config.Mappings) {
			bool TryParseScreen(int screenId, out Screen screen) {
				Screen? foundScreen = setup.Screens.FirstOrDefault(screen => screen.Id == screenId);
				if (foundScreen == null) {
					Terminal.Err($@"Screen id out of range. '{screenId}' supplied, but valid ids are: {setup.Screens.Aggregate(new StringBuilder(), (builder, screen) => {
						if (builder.Length > 0) builder.Append(", ");
						builder.Append(screen.Id);
						return builder;
					})}, aborting");
					screen = default!;
					return false;
				}

				screen = foundScreen;
				return true;
			}

			bool TryParseRange(Config.EdgeRange edgeRange, Edge edge, out R1I range) {
				bool TryParseAnchor(string anchorStr, Edge edge, out int anchor) {
					R1I pixelRange = new(0, edge.Length);
					R1I percentRange = new(0, 100);

					if (anchorStr.EndsWith("px")) {
						if (!int.TryParse(anchorStr[..^2], out int value)) {
							Terminal.Err($"Failed to parse anchor. '{anchorStr}' supplied, but int is malformed");
							anchor = default;
							return false;
						}

						if (
							value < pixelRange.Begin ||
							value > pixelRange.End
						) {
							Terminal.Err($"Anchor is out of range. '{anchorStr}' supplied, but valid range is {pixelRange.Begin}px-{pixelRange.End}px");
							anchor = default;
							return false;
						}

						anchor = value;
						return true;
					}
					if (anchorStr.EndsWith("%")) {
						if (!int.TryParse(anchorStr[..^1], out int value)) {
							Terminal.Err($"Failed to parse anchor. '{anchorStr}' supplied, but int is malformed");
							anchor = default;
							return false;
						}

						if (
							value < percentRange.Begin ||
							value > percentRange.End
						) {
							Terminal.Err($"Anchor is out of range. '{anchorStr}' supplied, but valid range is {percentRange.Begin}%-{percentRange.End}%");
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
				if (!TryParseAnchor(edgeRange.Begin ?? beginDefault, edge, out int begin)) {
					range = default;
					return false;
				}

				const string endDefault = "100%";
				if (!TryParseAnchor(edgeRange.End ?? endDefault, edge, out int end)) {
					range = default;
					return false;
				}

				range = new R1I(begin, end);
				return true;
			}

			if (!TryParseScreen(mapping.A.Screen, out Screen aScreen)) return false;
			Edge aEdge = aScreen.GetEdge(mapping.A.Side);
			if (!TryParseRange(mapping.A, aEdge, out R1I aRange)) return false;

			if (!TryParseScreen(mapping.B.Screen, out Screen bScreen)) return false;
			Edge bEdge = bScreen.GetEdge(mapping.B.Side);
			if (!TryParseRange(mapping.B, bEdge, out R1I bRange)) return false;

			Terminal.Inf($"Mapping 'screen{mapping.A.Screen} {aEdge.Side} [{aRange.Begin}-{aRange.End}]' to 'screen{mapping.B.Screen} {bEdge.Side} [{bRange.Begin}-{bRange.End}]'");
			Portal.Bind(
				new EdgeSpan(aEdge, aRange),
				new EdgeSpan(bEdge, bRange)
			);
		}

		return true;
	}

	private static Config? LoadConfig(string path) {
		if (!File.Exists(path)) {
			Terminal.Err($"'{path}' not found, aborting");
			return null;
		}

		string configText = File.ReadAllText(path);
		Config? config = JsonSerializer.Deserialize<Config>(configText);
		if (config == null) {
			Terminal.Err("Failed to parse config, aborting");
			return null;
		}

		return config;
	}
}
