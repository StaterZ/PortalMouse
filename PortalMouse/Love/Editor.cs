using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Love;
using PortalMouse.Engine.Core;

namespace PortalMouse.Love;

public class Editor : Scene {
	private Thread m_commandPollThread = null!;
	private readonly CancellationTokenSource m_cts = new();
	private readonly ConcurrentQueue<string> m_commandQueue = new();
	private readonly Graph Graph = new();
	private Font m_font = null!;

	public override void Load() {
		m_commandPollThread = new Thread(CommandPollLoop) {
			Name = "CommandPollThread",
			IsBackground = true,
		};
		m_commandPollThread.Start();

		m_font = Graphics.NewFont(64);
		Graphics.SetFont(m_font);
	}
	
	private void CommandPollLoop() {
		while (!m_cts.IsCancellationRequested) {
			if (Console.KeyAvailable) {
				string? line = Console.ReadLine();
				if (line == null) continue;

				m_commandQueue.Enqueue(line);
			}
		}
	}

	public override void Update(float dt) {
		dt = Mathf.Min(dt, 0.1f);

		ProcessCommandQueue();

		Selection.Reset();
		Graph.Update(dt);
	}

	private void ProcessCommandQueue() {
		while (m_commandQueue.Count > 0) {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
			if (!m_commandQueue.TryDequeue(out string line)) continue;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

			string[] args = ParseCommandArgs(line);
			switch (args[0]) {
				case "help":
					Console.WriteLine("help                  #show this list");
					Console.WriteLine("quit                  #close application");
					break;
				case "quit":
					if (args.Length > 1) {
						Console.WriteLine("Too many arguments!");
						break;
					}
					Event.Quit();
					break;
				default:
					Console.WriteLine("I don't know that command.");
					break;
			}

			Console.WriteLine();
		}
	}

	private static string[] ParseCommandArgs(string line) {
		List<string> parts = new();

		bool isInLiteral = false;
		bool isEscaped = false;
		StringBuilder builder = new();

		void FinalizeArg() {
			if (builder.Length <= 0) return;

			parts.Add(builder.ToString());
			builder.Clear();
		}

		foreach (char c in line) {
			if (!isEscaped) {
				switch (c) {
					case ' ' when !isInLiteral:
						FinalizeArg();
						continue;
					case '"':
						isInLiteral = !isInLiteral;
						continue;
						//case '\\' when isInLiteral:
						//	isEscaped = true;
						//	continue;
				}
			}

			builder.Append(c);
			isEscaped = false;
		}
		FinalizeArg();

		return parts.ToArray();
	}

	public override void Draw() {
		Graphics.Clear(Color.FromARGB(0xff101010));
		Graph.Draw();
	}

	public override bool Quit() {
		Graph.Deinit();

		m_cts.Cancel();
		m_commandPollThread.Join();
		return true;
	}

	private static Thread? s_thread;
	public static Editor? Open(Setup setup) {
		if (s_thread == null) {
			const string k_title = "PortalMouse Editor";
			s_thread = new(() => {
				BootConfig cfg = new() {
					WindowTitle = k_title,
					WindowDisplay = 0,
					WindowResizable = true,
					WindowVsync = true,
					WindowBorderless = false,
					WindowCentered = true,
					WindowFullscreen = false,
					WindowWidth = 1280,
					WindowHeight = 720,
				};

				SceneService.Boot(cfg);
				s_thread = null;
			}) {
				Name = k_title,
				IsBackground = true,
			};
			s_thread.Start();
		}

		Editor editor = new();
		foreach (Screen screen in setup.Screens) {
			editor.Graph.AddScreen(screen);
		}
		SceneService.Ins.ChangeScene(editor);

		return editor;
	}
}
