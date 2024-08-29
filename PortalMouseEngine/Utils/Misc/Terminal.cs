namespace PortalMouse.Engine.Utils.Misc;

public static class Terminal {
	public static void Imp(string msg) => Log("IMP", msg, ConsoleColor.Cyan);
	public static void Dbg(string msg) => Log("DBG", msg, ConsoleColor.Magenta);
	public static void Inf(string msg) => Log("INF", msg, ConsoleColor.White);
	public static void Wrn(string msg) => Log("WRN", msg, ConsoleColor.Yellow);
	public static void Err(string msg) => Log("ERR", msg, ConsoleColor.Red);

	public static void BlankLine() => Console.WriteLine();

	private static void Log(string prefix, string msg, ConsoleColor color) {
#if DEBUG
		msg = $"{prefix}:{msg}";
#endif
		WriteLineColor(msg, color);
	}

	private static void WriteLineColor(string msg, ConsoleColor color) {
		using (new FgScope(color)) {
			Console.WriteLine(msg);
		}
	}

	public static bool ReadYN() {
		while (true) {
			switch (Console.ReadKey().Key) {
				case ConsoleKey.Y:
					return true;
				case ConsoleKey.N:
					return false;
			}
		}
	}
}
