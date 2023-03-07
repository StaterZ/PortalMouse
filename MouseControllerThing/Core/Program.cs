﻿using MouseControllerThing.Utils;
using System.IO;
using System.Text.Json;

namespace MouseControllerThing.Core;

public static class Program {
	private static RunningState m_runningState = RunningState.Halted;

	[STAThread]
	public static void Main(string[] args) {
		NativeWrapper.ShowConsole(true);

		NotifyIcon tray = new();
		tray.Icon = Resources.trayIcon;
		tray.Visible = true;
		tray.Text = Application.ProductName;
		tray.ContextMenuStrip = new ContextMenuStrip();
		tray.ContextMenuStrip.Items.Add("Show", null, (sender, args) => NativeWrapper.ShowConsole(true));
		tray.ContextMenuStrip.Items.Add("Hide", null, (sender, args) => NativeWrapper.ShowConsole(false));
		tray.ContextMenuStrip.Items.Add("Reload", null, (sender, args) => m_runningState = RunningState.Restart);
		tray.ContextMenuStrip.Items.Add("Exit", null, (sender, args) => m_runningState = RunningState.Exit);

		Thread trayThread = new Thread(() => GuardedMain(args));
		trayThread.Start();

		Application.Run();
		trayThread.Join();
		tray.Dispose();
	}

	private static void GuardedMain(string[] args) {
		while (m_runningState != RunningState.Exit) {
			NativeWrapper.ShowConsole(true);
			m_runningState = RunningState.Running;
			try {
				Run(args);
			} catch (Exception ex) {
				m_runningState = RunningState.Halted;
				using (new FgScope(ConsoleColor.Red)) {
					Console.WriteLine(ex);
				}
			}

			if (m_runningState == RunningState.Halted) {
				NativeWrapper.ShowConsole(true);
				Console.WriteLine("Program has halted! (restart? [y/n])");
				m_runningState = ReadYN() ? RunningState.Restart : RunningState.Exit;
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

	private static void Run(string[] args){
		Console.Clear();

		Setup setup = new();
		Console.WriteLine("Screens:");
		foreach ((int i, ScreenInfo monitor) in NativeWrapper.GetDisplays().ZipIndex()) {
			Console.Write($"    {i}: {monitor.PhysicalRect}");
			if (monitor.Scale != 1) {
				Console.Write($" @ {monitor.Scale * 100}%");
#if DEBUG
				Console.Write($" -> Logical:{monitor.LogicalRect}");
#endif
			}
			Console.WriteLine();

			Screen screen = new(monitor);
			setup.screens.Add(screen);
		}
		Console.WriteLine();

		Console.WriteLine("Loading Config...");
		Config? config = LoadConfig();
		if (config == null) return;
		if (!LoadMappings(config, setup)) return;
		Console.WriteLine("Config Loaded!");
		Console.WriteLine();

		using (new FgScope(ConsoleColor.Green)) {
			Console.WriteLine("Entering Runtime...");
		}
#if !DEBUG
		Thread.Sleep(500);
		NativeWrapper.ShowConsole(false);
#endif

		V2I? prevP = null;
		while (m_runningState == RunningState.Running) {
			Native.GetCursorPos(out Point point);
			V2I p = new(point);
			if (p == prevP) continue;
			//Console.WriteLine($"{prevP} ===========> {p}");

			V2I? movedP = setup.Handle(p);
			if (movedP.HasValue) {
				Native.SetCursorPos(movedP.Value.x, movedP.Value.y);
				Console.WriteLine($"Moved: {p} -> {movedP.Value}");
				p = movedP.Value;
			}

			prevP = p;
		}
	}

	private static bool LoadMappings(Config config, Setup setup) {
		if (config.mappings.Length <= 0) {
			using (new FgScope(ConsoleColor.Yellow)) {
				Console.WriteLine("No mappings present in config!");
			}
			return true;
		}

		foreach (Config.Mapping mapping in config.mappings) {
			bool TryParseScreen(int screenIndex, out Screen screen) {
				if (screenIndex >= 0 && screenIndex < setup.screens.Count) {
					screen = setup.screens[screenIndex];
					return true;
				} else {
					using (new FgScope(ConsoleColor.Red)) {
						Console.WriteLine($"Screen index out of range. '{screenIndex}' supplied, but range is 0-{setup.screens.Count - 1}, aborting");
					}
					screen = default!;
					return false;
				}
			}

			bool TryParseRange(Config.EdgeRange edgeRange, Edge edge, out R1I range) {
				int begin = edgeRange.begin ?? 0;
				int end = edgeRange.end ?? edge.Length;
				if (begin >= 0 && end <= edge.Length) {
					range = new R1I(begin, end);
					return true;
				} else {
					using (new FgScope(ConsoleColor.Red)) {
						Console.WriteLine($"EdgeRange is out of range. '{edgeRange.begin.ToString() ?? $"({begin})"}-{edgeRange.end.ToString() ?? $"({end})"}' supplied, but range is 0-{edge.Length}, aborting");
					}
					range = default!;
					return false;
				}
			}

			if (!TryParseScreen(mapping.a.screen, out Screen aScreen)) return false;
			Edge aEdge = aScreen.GetEdge(mapping.a.side);
			if (!TryParseRange(mapping.a, aEdge, out R1I aRange)) return false;

			if (!TryParseScreen(mapping.b.screen, out Screen bScreen)) return false;
			Edge bEdge = bScreen.GetEdge(mapping.b.side);
			if (!TryParseRange(mapping.b, bEdge, out R1I bRange)) return false;

			Console.WriteLine($"Binding 'screen{mapping.a.screen}_{aEdge.Side}[{aRange.Begin}-{aRange.End}]' to 'screen{mapping.b.screen}_{bEdge.Side}[{bRange.Begin}-{bRange.End}]'");
			Connection.Bind(
				new EdgeSpan(aEdge, aRange),
				new EdgeSpan(bEdge, bRange)
			);
		}
		return true;
	}

	private static Config? LoadConfig() {
		const string configPath = "config.json";
		if (!File.Exists(configPath))
		{
			using (new FgScope(ConsoleColor.Red))
			{
				Console.WriteLine($"'{configPath}' not found, aborting");
			}
			return null;
		}

		string configText = File.ReadAllText(configPath);
		Config? config = JsonSerializer.Deserialize<Config>(configText);
		if (config == null)
		{
			using (new FgScope(ConsoleColor.Red))
			{
				Console.WriteLine("Failed to parse config, aborting");
			}
			return null;
		}

		return config;
	}
}
