using System.Runtime.InteropServices;
using MouseControllerThing.Native;

namespace MouseControllerThing.Testing;

public class HookTest
{
	private readonly User32.HookProc? callback = null!; //required to keep memory alive
	private IntPtr _llMouseHook = IntPtr.Zero;
	private HookHandler hookHandler;

	private Rectangle _cursorScreenBounds;


	public HookTest()
	{
		// initialize our delegate
		callback = LlMouseHookCallback;

		// setup a keyboard hook
		User32.SetWindowsHookEx(HookType.WH_MOUSE_LL, callback, IntPtr.Zero, 0);
	}

	private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
	{
		if (code < 0)
		{
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

	private IntPtr LlMouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
	{
		if (nCode == 0 && (uint)wParam == User32.WmMouseMove)
		{
			Point mouse = GetMouseLocation(lParam);
			Console.WriteLine($"Geh! - {mouse}");
			// if (!_cursorScreenBounds.Contains(mouse) && NativeMethods.GetCursorPos(out var cursor) && _mouseLogic.HandleMouse(mouse, cursor, out var newCursor))
			// {
			// 	User32.SetCursorPos(newCursor);
			// 	return (IntPtr) 1;
			// }
		}

		return User32.CallNextHookEx(_llMouseHook, nCode, wParam, lParam);
	}

	private static Point GetMouseLocation(IntPtr lParam)
	{
		User32.Msllhookstruct hookStruct = (User32.Msllhookstruct)Marshal.PtrToStructure(lParam, typeof(User32.Msllhookstruct))!;
		return hookStruct.pt;
	}
}
