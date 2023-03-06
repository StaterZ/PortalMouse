using MouseControllerThing.Utils;
using System.Drawing;
using System.Text.Json;

namespace MouseControllerThing;

public static class Program {
	private static void Main(string[] args) {
		Setup setup = new();
		Console.WriteLine("Screens:");
		foreach ((int i, Native.MonitorInfo display) in GetDisplays().ZipIndex()) {
			Console.WriteLine($"    {i}: {display.Monitor}");

			Screen screen = new(display);
			setup.screens.Add(screen);
		}
		Console.WriteLine();

		Console.WriteLine("Loading Config...");
		if (!LoadMappings(setup)) {
			Console.WriteLine("Program has halted! (Enter to exit)");
			Console.ReadLine();
			return;
		}
		Console.WriteLine("Config Loaded!");
		Console.WriteLine();

		Console.WriteLine("Entering Runtime...");
		V2I? prevP = null;
		while (true) {
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
			Console.WriteLine("'config.cfg' not found, aborting");
			return false;
		}

		string configText = File.ReadAllText(configPath);
		Config? config = JsonSerializer.Deserialize<Config>(configText);
		if (config == null) {
			Console.WriteLine("Failed to parse config, aborting");
			return false;
		}

		foreach (Config.Mapping mapping in config.mappings) {
			if (mapping.a.screen < 0 || mapping.a.screen >= setup.screens.Count) {
				Console.WriteLine($"Screen index out of range. {mapping.a.screen} supplied, but range is 0-{setup.screens.Count - 1}");
				return false;
			}
			Screen a = setup.screens[mapping.a.screen];
			Edge aEdge = a.GetEdge(mapping.a.side);

			if (mapping.b.screen < 0 || mapping.b.screen >= setup.screens.Count) {
				Console.WriteLine($"Screen index out of range. {mapping.b.screen} supplied, but range is 0-{setup.screens.Count - 1}");
				return false;
			}
			Screen b = setup.screens[mapping.b.screen];
			Edge bEdge = b.GetEdge(mapping.b.side);

			Connection.Bind(
				new EdgeSpan(aEdge, new Range(mapping.a.begin ?? 0, mapping.a.end ?? aEdge.Length)),
				new EdgeSpan(bEdge, new Range(mapping.b.begin ?? 0, mapping.b.end ?? bEdge.Length))
			);
		}
		return true;
	}

	//http://pinvoke.net/default.aspx/user32/EnumDisplayMonitors.html
	public static List<Native.MonitorInfo> GetDisplays() {
		List<Native.MonitorInfo> result = new();

		Native.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
			(IntPtr hMonitor, IntPtr hdcMonitor, ref Native.Rect lprcMonitor, IntPtr dwData) => {
				Native.MonitorInfo mi = new();
				if (Native.GetMonitorInfo(hMonitor, mi)) {
					result.Add(mi);
				}
				return true;
			}, IntPtr.Zero
		);

		return result;
	}

	public static IEnumerable<(int index, T item)> ZipIndex<T>(this IEnumerable<T> self) {
		int i = 0;
		foreach (T item in self) {
			yield return (i, item);
			i++;
		}
	}
}
