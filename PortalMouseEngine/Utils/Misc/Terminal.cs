namespace PortalMouse.Utils.Misc;

public static class Terminal {
	public static void Imp(string msg) => WriteLineColor(msg, ConsoleColor.Cyan);
	public static void Dbg(string msg) => WriteLineColor(msg, ConsoleColor.Magenta);
	public static void Inf(string msg) => Console.WriteLine(msg);
	public static void Wrn(string msg) => WriteLineColor(msg, ConsoleColor.Yellow);
	public static void Err(string msg) => WriteLineColor(msg, ConsoleColor.Red);

	public static void BlankLine() => Console.WriteLine();

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
