using MouseControllerThing.Utils;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace MouseControllerThing.Core;

public static class Program {
	public static void Main(string[] args) {
		while (true) {
			try {
				GuradedMain(args);
			} catch(Exception ex) {
				ConsoleColor prevColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex);
				Console.ForegroundColor = prevColor;
			}
			NativeWrapper.ShowConsole(true);
			Console.WriteLine("Program has halted! (Enter to reboot)");
			Console.ReadLine();
			Console.WriteLine();
		}
	}

	private static void GuradedMain(string[] args) {
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

		Console.WriteLine("Entering Runtime...");
		Thread.Sleep(1000);
		NativeWrapper.ShowConsole(false);

		V2I? prevP = null;
		while (true)
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
			Console.WriteLine($"'{configPath}' not found, aborting");
			return false;
		}

		string configText = File.ReadAllText(configPath);
		Config? config = JsonSerializer.Deserialize<Config>(configText);
		if (config == null) {
			Console.WriteLine("Failed to parse config, aborting");
			return false;
		}

		foreach (Config.Mapping mapping in config.mappings) {
			bool TryGetScreen(int screenIndex, out Screen screen) {
				if (screenIndex >= 0 && screenIndex < setup.screens.Count) {
					screen = setup.screens[screenIndex];
					return true;
				} else {
					Console.WriteLine($"Screen index out of range. '{screenIndex}' supplied, but range is 0-{setup.screens.Count - 1}, aborting");
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
					Console.WriteLine($"edgeRange is out of range. '{edgeRange.begin.ToString() ?? $"({begin})"}-{edgeRange.end.ToString() ?? $"({end})"}' supplied, but range is 0-{edge.Length}, aborting");
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

			Connection.Bind(
				new EdgeSpan(aEdge, aRange),
				new EdgeSpan(bEdge, bRange)
			);
		}
		return true;
	}
}
