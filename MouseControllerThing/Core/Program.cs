﻿using MouseControllerThing.Hooking;
using MouseControllerThing.Utils;
using System.IO;
using System.Text.Json;
using MouseControllerThing.Utils.Ext;
using MouseControllerThing.Utils.Maths;

namespace MouseControllerThing.Core;

public static class Program {
	private static RunningState s_runningState = RunningState.Halted;

	[STAThread]
	public static void Main(string[] args) {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		NativeWrapper.ShowConsole(true);

		// ContextMenuStrip strip = new();
		// strip.Items.Add("Show", null, (sender, eventArgs) => NativeWrapper.ShowConsole(true));
		// strip.Items.Add("Hide", null, (sender, eventArgs) => NativeWrapper.ShowConsole(false));
		// strip.Items.Add("Reload", null, (sender, eventArgs) => s_runningState = RunningState.Restart);
		// strip.Items.Add("Exit", null, (sender, eventArgs) => s_runningState = RunningState.Exit);
		// NotifyIcon tray = new() {
		// 	Icon = Resources.trayIcon,
		// 	Visible = true,
		// 	Text = Application.ProductName,
		// 	ContextMenuStrip = strip,
		// };

		Setup? setup = Run(args);
		if (setup == null) {
			Console.WriteLine("Setup Failed!"); //TODO: not the best error handling ever... really should fix this...
			return;
		}

		V2I? prevPos = null;
		LLMouseHook llMouseHook = new(pos => {
			if (pos == prevPos) return;

			V2I? movedPos = setup.Handle(pos);
			if (movedPos.HasValue) {
				NativeWrapper.CursorPos = movedPos.Value;
				Console.WriteLine($"Moved: {pos} -> {movedPos.Value}");
				pos = movedPos.Value;
			}

			prevPos = pos;
		});

		Application.Run();
		// tray.Visible = false;
		// tray.Dispose();
		llMouseHook.Dispose();
	}

	private static void GuardedMain(string[] args) {
		while (s_runningState != RunningState.Exit) {
			NativeWrapper.ShowConsole(true);
			s_runningState = RunningState.Running;
			try {
				Run(args);
			} catch (Exception ex) {
				s_runningState = RunningState.Halted;
				using (new FgScope(ConsoleColor.Red)) {
					Console.WriteLine(ex);
				}
			}

			if (s_runningState == RunningState.Halted) {
				NativeWrapper.ShowConsole(true);
				Console.WriteLine("Program has halted! (restart? [y/n])");
				s_runningState = ReadYN() ? RunningState.Restart : RunningState.Exit;
			}
		}

		Application.Exit();
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

	private static Setup? Run(string[] args) {
		Console.Clear();

		Setup setup = new();
		Console.WriteLine("Screens:");
		foreach ((int i, ScreenInfo monitor) in NativeWrapper.GetDisplays().Enumerate()) {
			Console.Write($"    {i}: {monitor.PhysicalRect}");
			if (monitor.Scale != 1) {
				Console.Write($" @ {monitor.Scale * 100}%");
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
		Config? config = LoadConfig();
		if (config == null) return null;
		if (!LoadMappings(config, setup)) return null;
		Console.WriteLine("Config Loaded!");
		Console.WriteLine();

		using (new FgScope(ConsoleColor.Green)) {
			Console.WriteLine("Entering Runtime...");
		}
#if !DEBUG
		Thread.Sleep(500);
		NativeWrapper.ShowConsole(false);
#endif

		return setup;

		// V2I? prevCursorPos = null;
		// while (s_runningState == RunningState.Running) {
		// 	Thread.Sleep(1);
		// 	V2I cursorPos = NativeWrapper.CursorPos;
		// 	if (cursorPos == prevCursorPos) continue;
		//
		// 	V2I? movedP = setup.Handle(cursorPos);
		// 	if (movedP.HasValue) {
		// 		NativeWrapper.CursorPos = movedP.Value;
		// 		Console.WriteLine($"Moved: {cursorPos} -> {movedP.Value}");
		// 		cursorPos = movedP.Value;
		// 	}
		//
		// 	prevCursorPos = cursorPos;
		// }
	}

	private static bool LoadMappings(Config config, Setup setup) {
		if (config.Portals.Length <= 0) {
			using (new FgScope(ConsoleColor.Yellow)) {
				Console.WriteLine("No mappings present in config!");
			}
			return true;
		}

		foreach (Config.Portal mapping in config.Portals) {
			bool TryParseScreen(int screenIndex, out Screen screen) {
				if (screenIndex < 0 && screenIndex >= setup.Screens.Count) {
					using (new FgScope(ConsoleColor.Red)) {
						Console.WriteLine($"Screen index out of range. '{screenIndex}' supplied, but range is 0-{setup.Screens.Count - 1}, aborting");
					}
					screen = default!;
					return false;
				}
				screen = setup.Screens[screenIndex];
				return true;
			}

			bool TryParseRange(Config.EdgeRange edgeRange, Edge edge, out R1I range) {
				int begin = edgeRange.Begin ?? 0;
				int end = edgeRange.End ?? edge.Length;
				if (begin < 0 && end > edge.Length) {
					using (new FgScope(ConsoleColor.Red)) {
						Console.WriteLine($"EdgeRange is out of range. '{edgeRange.Begin.ToString() ?? $"({begin})"}-{edgeRange.End.ToString() ?? $"({end})"}' supplied, but range is 0-{edge.Length}, aborting");
					}
					range = default!;
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

			Console.WriteLine($"Mapping 'screen{mapping.A.Screen}_{aEdge.Side}[{aRange.Begin}-{aRange.End}]' to 'screen{mapping.B.Screen}_{bEdge.Side}[{bRange.Begin}-{bRange.End}]'");
			Portal.Bind(
				new EdgeSpan(aEdge, aRange),
				new EdgeSpan(bEdge, bRange)
			);
		}
		return true;
	}

	private static Config? LoadConfig() {
		const string configPath = "config.json";
		if (!File.Exists(configPath)) {
			using (new FgScope(ConsoleColor.Red)) {
				Console.WriteLine($"'{configPath}' not found, aborting");
			}
			return null;
		}

		string configText = File.ReadAllText(configPath);
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
