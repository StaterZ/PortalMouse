using MouseControllerThing.Utils;
using System.IO;
using System.Text.Json;

namespace MouseControllerThing.Core;

public static class Program {
	private static bool m_isRunning;

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
		tray.ContextMenuStrip.Items.Add("Halt", null, (sender, args) => m_isRunning = false);

		Thread trayThread = new Thread(() => GuardedMain(args));
		trayThread.Start();

		Application.Run();
		trayThread.Join();
		tray.Dispose();
	}

	private static void GuardedMain(string[] args) {
		NativeWrapper.ShowConsole(true);

		while (true) {
			try {
				Run(args);
			} catch (Exception ex) {
				m_isRunning = false;
				using (new FgScope(ConsoleColor.Red)) {
					Console.WriteLine(ex);
				}
			}

			NativeWrapper.ShowConsole(true);
			Console.WriteLine("Program has halted! (reboot? [y/n])");
			if (!ReadYN()) {
				break;
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
		m_isRunning = true;
		Console.Clear();

		Setup setup = new();
		Console.WriteLine("Screens:");
		foreach ((int i, Native.MonitorInfo display) in NativeWrapper.GetDisplays().ZipIndex())
		{
			Console.WriteLine($"    {i}: {display.Monitor}");

			Screen screen = new(display);
			setup.screens.Add(screen);
		}
		Console.WriteLine();

		Console.WriteLine("Loading Config...");
		if (!LoadMappings(setup)) return;
		Console.WriteLine("Config Loaded!");
		Console.WriteLine();

		using (new FgScope(ConsoleColor.Green)) {
			Console.WriteLine("Entering Runtime...");
		}
		Thread.Sleep(500);
		NativeWrapper.ShowConsole(false);

		V2I? prevP = null;
		while (m_isRunning)
		{
			Native.GetCursorPos(out Point point);
			V2I p = new(point);
			if (p == prevP) continue;
			prevP = p;
			V2I? result = setup.Handle(p);
			if (!result.HasValue) continue;
			Console.WriteLine($"Moved: {p} -> {result.Value}");
			Native.SetCursorPos(result.Value.x, result.Value.y);
		}
	}

	private static bool LoadMappings(Setup setup) {
		const string configPath = "config.json";
		if (!File.Exists(configPath)) {
			using (new FgScope(ConsoleColor.Red)) {
				Console.WriteLine($"'{configPath}' not found, aborting");
			}
			return false;
		}

		string configText = File.ReadAllText(configPath);
		Config? config = JsonSerializer.Deserialize<Config>(configText);
		if (config == null) {
			using (new FgScope(ConsoleColor.Red)) {
				Console.WriteLine("Failed to parse config, aborting");
			}
			return false;
		}

		if (config.mappings.Length <= 0) {
			using (new FgScope(ConsoleColor.Yellow)) {
				Console.WriteLine("No mappings present in config!");
			}
			return true;
		}

		foreach (Config.Mapping mapping in config.mappings) {
			bool TryGetScreen(int screenIndex, out Screen screen) {
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

			bool TryParseRange(Config.EdgeRange edgeRange, Edge edge, out Range range) {
				int begin = edgeRange.begin ?? 0;
				int end = edgeRange.end ?? edge.Length;
				if (begin >= 0 && end <= edge.Length) {
					range = new Range(begin, end);
					return true;
				} else {
					using (new FgScope(ConsoleColor.Red)) {
						Console.WriteLine($"EdgeRange is out of range. '{edgeRange.begin.ToString() ?? $"({begin})"}-{edgeRange.end.ToString() ?? $"({end})"}' supplied, but range is 0-{edge.Length}, aborting");
					}
					range = default!;
					return false;
				}
			}

			if (!TryGetScreen(mapping.a.screen, out Screen aScreen)) return false;
			Edge aEdge = aScreen.GetEdge(mapping.a.side);
			if (!TryParseRange(mapping.a, aEdge, out Range aRange)) return false;

			if (!TryGetScreen(mapping.b.screen, out Screen bScreen)) return false;
			Edge bEdge = bScreen.GetEdge(mapping.b.side);
			if (!TryParseRange(mapping.b, bEdge, out Range bRange)) return false;

			Console.WriteLine($"Binding 'screen{mapping.a.screen}_{aEdge.Side}[{aRange.begin}-{aRange.end}]' to 'screen{mapping.b.screen}_{bEdge.Side}[{bRange.begin}-{bRange.end}]'");
			Connection.Bind(
				new EdgeSpan(aEdge, aRange),
				new EdgeSpan(bEdge, bRange)
			);
		}
		return true;
	}
}
