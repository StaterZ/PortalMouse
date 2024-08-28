using PortalMouse.Engine.Core;
using PortalMouse.Engine.Observers;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;
using System.IO;
using System.Text;
using System.Text.Json;
using Screen = PortalMouse.Engine.Core.Screen;

namespace PortalMouse.Frontend;

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

		Terminal.Imp("[PortalMouse] by StaterZ");
		Terminal.BlankLine();

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

	private static MouseObserver CreateObserver(Func<V2I, V2I?> callback) {
		//return new PollObserver(callback);
		return new LLMHookObserver(callback);
	}

	private static Setup? LoadSetup() {
		Setup setup = Setup.ConstructLocalSetup();
		Terminal.Inf("Screens:");
		StringBuilder builder = new();
		foreach (Screen screen in setup.Screens) {
			builder.Append($"    {screen.Id} : {screen.PhysicalRect} @ {(float)(screen.Scale * 100)}%");
			if (screen.Scale != Frac.One) {
				builder.Append($" -> {screen.LogicalRect}");
			}
			Terminal.Inf(builder.ToString());
			builder.Clear();
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
					R1I validPixelRange = new(0, edge.Length); //begin is 0 since this is in local space
					R1I validPercentRange = new(0, 100);

					if (anchorStr.EndsWith("px")) {
						if (!int.TryParse(anchorStr[..^2], out int value)) {
							Terminal.Err($"Failed to parse anchor. '{anchorStr}' supplied, but int is malformed");
							anchor = default;
							return false;
						}

						if (
							value < validPixelRange.Begin ||
							value > validPixelRange.End
						) {
							Terminal.Err($"Anchor is out of range. '{anchorStr}' supplied, but valid range is {validPixelRange.Begin}px-{validPixelRange.End}px");
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
							value < validPercentRange.Begin ||
							value > validPercentRange.End
						) {
							Terminal.Err($"Anchor is out of range. '{anchorStr}' supplied, but valid range is {validPercentRange.Begin}%-{validPercentRange.End}%");
							anchor = default;
							return false;
						}

						anchor = MathX.Map(value, validPercentRange, validPixelRange);
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
