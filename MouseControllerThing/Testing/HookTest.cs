using MouseControllerThing.Native;
using System.Runtime.InteropServices;

namespace MouseControllerThing.Testing;

public class HookTest : IDisposable
{
	private readonly User32.HookProc? callback = null!; //required to keep memory alive
	private readonly IntPtr llMouseHook = IntPtr.Zero;
	private readonly HookHandler hookHandler = new();

	public HookTest() {
		// initialize our delegate
		callback = LlMouseHookCallback;

		// setup a keyboard hook
		hookHandler.SetHook(HookType.WH_MOUSE_LL, callback);
	}

	public void Dispose() {
		hookHandler.UnsetHook();
	}

	private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam) {
		if (code < 0) {
			//you need to call CallNextHookEx without further processing
			//and return the value returned by CallNextHookEx
			return User32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
		}
		// we can convert the 2nd parameter (the key code) to a System.Windows.Forms.Keys enum constant
		Keys keyPressed = (Keys)wParam.ToInt32();
		Console.WriteLine(keyPressed);
		//return the value returned by CallNextHookEx
		return User32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
	}

	private IntPtr LlMouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
		if (nCode == 0 && (uint)wParam == User32.WmMouseMove) {
			Point mouse = GetMouseLocation(lParam);
			Console.WriteLine($"Geh! - {mouse}");
			// if (!_cursorScreenBounds.Contains(mouse) && NativeMethods.GetCursorPos(out var cursor) && _mouseLogic.HandleMouse(mouse, cursor, out var newCursor))
			// {
			// 	User32.SetCursorPos(newCursor);
			// 	return (IntPtr) 1;
			// }
		}

		return User32.CallNextHookEx(llMouseHook, nCode, wParam, lParam);
	}

	private static Point GetMouseLocation(IntPtr lParam) {
		User32.Msllhookstruct hookStruct = (User32.Msllhookstruct)Marshal.PtrToStructure(lParam, typeof(User32.Msllhookstruct))!;
		return hookStruct.pt;
	}
}
