using System;
using Love;

namespace PortalMouse.Love;

public class SceneService : Scene {
	public static SceneService Ins { get; private set; } = null!;

	public Scene? Scene { get; private set; }

	private Scene? m_queuedScene;
	private readonly BootConfig m_cfg;
	private bool m_isLoaded;
	private readonly object m_queuedSceneLock = new();

	public SceneService(BootConfig cfg) => m_cfg = cfg;

	public static void Boot(BootConfig cfg) {
		if (Ins != null) throw new Exception();

		Ins = new SceneService(cfg);
		global::Love.Boot.Run(Ins, Ins.m_cfg);
	}

	public void ChangeScene(Scene? scene) {
		lock (m_queuedSceneLock) {
			m_queuedScene = scene;
		}
	}

	private void ChangeSceneImmediate(Scene? scene) {
		if (scene == Scene) return;

		Scene?.Quit();
		if (Scene != null && scene == null) {
			Window.Close();
		}
		if (Scene == null && scene != null) {
			OpenWindow(m_cfg);
		}
		Scene = scene;
		m_queuedScene = Scene;
		Scene?.Load();
	}

	public override void Load() {
		if (m_isLoaded) return;

		Scene?.Load();
		m_isLoaded = true;
	}

	public override bool Quit() {
		if (Scene?.Quit() ?? false) {
			ChangeSceneImmediate(null);
		}
		return false;
	}

	public override void Update(float dt) {
		if (m_queuedScene != Scene) {
			lock (m_queuedSceneLock) {
				ChangeSceneImmediate(m_queuedScene);
			}
		}
		Scene?.Update(dt);
	}

	public static void OpenWindow(BootConfig cfg) {
		if (cfg.WindowTitle != null) {
			Window.SetTitle(cfg.WindowTitle);
		}

		WindowSettings settings = new() {
			FullscreenType = cfg.WindowFullscreenType,
			Fullscreen = cfg.WindowFullscreen,
			Vsync = cfg.WindowVsync,
			MSAA = cfg.WindowMSAA,
			Resizable = cfg.WindowResizable,
			MinWidth = cfg.WindowMinWidth,
			MinHeight = cfg.WindowMinHeight,
			Borderless = cfg.WindowBorderless,
			Centered = cfg.WindowCentered,
			Display = cfg.WindowDisplay,
			HighDpi = cfg.WindowHighdpi,
		};
		if (cfg.WindowX.HasValue) {
			settings.X = cfg.WindowX.Value;
		}

		if (cfg.WindowY.HasValue) {
			settings.Y = cfg.WindowY.Value;
		}

		Window.SetMode(cfg.WindowWidth, cfg.WindowHeight, settings);
	}

	public override bool Equals(object? obj) => Scene?.Equals(obj) ?? base.Equals(obj);
	public override int GetHashCode() => Scene?.GetHashCode() ?? base.GetHashCode();
	public override string? ToString() => Scene?.ToString() ?? base.ToString();

	public override void DirectoryDropped(string path) => Scene?.DirectoryDropped(path);
	public override void Draw() => Scene?.Draw();
	public override bool ErrorHandler(Exception e) => Scene?.ErrorHandler(e) ?? base.ErrorHandler(e);
	public override void FileDropped(string fileFilePath) => Scene?.FileDropped(fileFilePath);
	public override void JoystickAdded(Joystick joystick) => Scene?.JoystickAdded(joystick);
	public override void JoystickAxis(Joystick joystick, float axis, float value) => Scene?.JoystickAxis(joystick, axis, value);
	public override void JoystickGamepadAxis(Joystick joystick, GamepadAxis axis, float value) => Scene?.JoystickGamepadAxis(joystick, axis, value);
	public override void JoystickGamepadPressed(Joystick joystick, GamepadButton button) => Scene?.JoystickGamepadPressed(joystick, button);
	public override void JoystickGamepadReleased(Joystick joystick, GamepadButton button) => Scene?.JoystickGamepadReleased(joystick, button);
	public override void JoystickHat(Joystick joystick, int hat, JoystickHat direction) => Scene?.JoystickHat(joystick, hat, direction);
	public override void JoystickPressed(Joystick joystick, int button) => Scene?.JoystickPressed(joystick, button);
	public override void JoystickReleased(Joystick joystick, int button) => Scene?.JoystickReleased(joystick, button);
	public override void JoystickRemoved(Joystick joystick) => Scene?.JoystickRemoved(joystick);
	public override void KeyPressed(KeyConstant key, Scancode scancode, bool isRepeat) => Scene?.KeyPressed(key, scancode, isRepeat);
	public override void KeyReleased(KeyConstant key, Scancode scancode) => Scene?.KeyReleased(key, scancode);
	public override void LowMemory() => Scene?.LowMemory();
	public override void MouseFocus(bool focus) => Scene?.MouseFocus(focus);
	public override void MouseMoved(float x, float y, float dx, float dy, bool isTouch) => Scene?.MouseMoved(x, y, dx, dy, isTouch);
	public override void MousePressed(float x, float y, int button, bool isTouch) => Scene?.MousePressed(x, y, button, isTouch);
	public override void MouseReleased(float x, float y, int button, bool isTouch) => Scene?.MouseReleased(x, y, button, isTouch);
	public override void TextEditing(string text, int start, int end) => Scene?.TextEditing(text, start, end);
	public override void TextInput(string text) => Scene?.TextInput(text);
	public override void TouchMoved(long id, float x, float y, float dx, float dy, float pressure) => Scene?.TouchMoved(id, x, y, dx, dy, pressure);
	public override void TouchPressed(long id, float x, float y, float dx, float dy, float pressure) => Scene?.TouchPressed(id, x, y, dx, dy, pressure);
	public override void TouchReleased(long id, float x, float y, float dx, float dy, float pressure) => Scene?.TouchReleased(id, x, y, dx, dy, pressure);
	public override void WheelMoved(int x, int y) => Scene?.WheelMoved(x, y);
	public override void WindowFocus(bool focus) => Scene?.WindowFocus(focus);
	public override void WindowResize(int w, int h) => Scene?.WindowResize(w, h);
	public override void WindowVisible(bool visible) => Scene?.WindowVisible(visible);
}
