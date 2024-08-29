using CommandLine;
using CommandLine.Text;
using PortalMouse.Engine.Core;
using PortalMouse.Engine.Observers;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Frontend;

public static class Program {
	private static RunningState s_runningState = RunningState.Halted;

	[STAThread]
	private static void Main(string[] args) {
		Options? options = Options.Parse(args);
		if (options == null) return;

		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		using TrayIcon tray = new();

		Runtime(options);
	}

	public static void UpdateState(RunningState state) {
		s_runningState = state;
		Application.Exit();
	}

	private static void Runtime(Options options) {
		s_runningState = RunningState.Running;

		while (true) {
			switch (s_runningState) {
				case RunningState.Running:
					try {
						Run(options);
					} catch (Exception ex) {
						HandleException(ex);
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
					NativeHelper.ShowConsole(true);
					Terminal.Imp("Program has exit!");
					return;
			}
		}
	}

	private static void Run(Options options) {
		NativeHelper.ShowConsole(true);
		Console.Clear();

		Terminal.Imp($"[{Application.ProductName}] V{Application.ProductVersion} by StaterZ");
		Terminal.BlankLine();

		Setup setup = Setup.ConstructLocalSetup();
		FrontendUtils.PrintSetupStats(setup);

		if (setup.Screens.Count <= 0) {
			Terminal.Err("No screens found in setup? Something has gone terribly wrong! Halting...");
			UpdateState(RunningState.Halted);
			return;
		}

		Terminal.Inf("Loading Config...");
		Config? config = FrontendUtils.LoadConfig(options.ConfigPath);
		if (config == null) {
			Terminal.Err("Failed to load the config. Did you move it without setting a custom config path argument? Halting...");
			UpdateState(RunningState.Halted);
			return;
		}
		if (!FrontendUtils.ApplyConfig(config, setup)) {
			Terminal.Err("Failed to apply some or all config mappings. Halting...");
			UpdateState(RunningState.Halted);
			return;
		}

		Terminal.Inf("Config Successfully Loaded!");
		Terminal.BlankLine();

		V2I? MoveHandler(V2I pos) {
			V2I? movedPos = setup!.Handle(pos);
#if DEBUG
			if (movedPos.HasValue) {
				Terminal.Dbg($"Moved: {pos} -> {movedPos.Value}");
			}
#endif

			return movedPos;
		}

		Terminal.Imp("Starting Observer!");
#if !DEBUG
		if (options.HideDelay > 0) Thread.Sleep(options.HideDelay);
		NativeHelper.ShowConsole(false);
#endif

		MouseObserver mouseObserver = CreateObserver(MoveHandler, options.Observer);

		Application.Run();

		mouseObserver.Dispose();
	}

	private static MouseObserver CreateObserver(Func<V2I, V2I?> callback, Options.ObserverKind observerKind) => observerKind switch {
		Options.ObserverKind.Poll => new PollObserver(callback, HandleException),
		Options.ObserverKind.LLMH => new LLMHookObserver(callback, HandleException),
		_ => throw new UnreachableException(),
	};

	private static void HandleException(Exception ex) {
		UpdateState(RunningState.Halted);
		using (new FgScope(ConsoleColor.Red)) {
			Console.WriteLine(ex);
		}
	}

	private class Options {
		[Option('d', "delay", Default = 500, HelpText = "How many milis to wait before hiding the window as it starts.")]
		public int HideDelay { get; set; }
		[Option("cfg", Default = "config.json", HelpText = "The config path.")]
		public string ConfigPath { get; set; } = null!;
		[Option('o', "observer", Default = ObserverKind.LLMH, HelpText = "What backend to use for reading the mouse position.")]
		public ObserverKind Observer { get; set; }

		public static Options? Parse(string[] args) {
			Parser parser = new(cfg => {
				cfg.CaseInsensitiveEnumValues = true;
				cfg.HelpWriter = null;
			});

			ParserResult<Options> parserResult = parser.ParseArguments<Options>(args);
			parserResult.WithNotParsed(_ => {
				Terminal.Inf(HelpText.AutoBuild(parserResult, h => {
					h.Heading = string.Empty;
					h.Copyright = string.Empty;
					h.AddEnumValuesToHelpText = true;
					h.AdditionalNewLineAfterOption = true;
					h.MaximumDisplayWidth = int.MaxValue;
					return h;
				}));
			});

			return parserResult.Value;
		}

		public enum ObserverKind {
			Poll,
			LLMH,
		}
	}
}
