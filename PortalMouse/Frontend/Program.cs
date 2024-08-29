using PortalMouse.Engine.Core;
using PortalMouse.Engine.Observers;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Frontend;

public static class Program {
	private static RunningState s_runningState = RunningState.Halted;

	[STAThread]
	private static void Main(string[] args) {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		using TrayIcon tray = new();

		Runtime(args);
	}

	public static void UpdateState(RunningState state) {
		s_runningState = state;
		Application.Exit();
	}

	private static void Runtime(string[] args) {
		s_runningState = RunningState.Running;

		while (true) {
			switch (s_runningState) {
				case RunningState.Running:
					try {
						Run(args);
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
					return;
			}
		}
	}

	private static void Run(string[] args) {
		string configPath = args.Length > 0 ? args[0] : "config.json";

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
		Config? config = FrontendUtils.LoadConfig(configPath);
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
		//return new PollObserver(callback, HandleException);
		return new LLMHookObserver(callback, HandleException);
	}

	private static void HandleException(Exception ex) {
		UpdateState(RunningState.Halted);
		using (new FgScope(ConsoleColor.Red)) {
			Console.WriteLine(ex);
		}
	}
}
