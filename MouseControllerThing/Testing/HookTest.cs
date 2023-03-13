using MouseControllerThing.Native;

namespace MouseControllerThing.Testing;

/*
public class HookTest
{
	private User32.HookProc? callback = null; //required to keep memory alive

	public HookTest()
	{
		// initialize our delegate
		callback = new User32.HookProc(MyCallbackFunction);

		// setup a keyboard hook
		User32.SetWindowsHookEx(HookType.WH_MOUSE_LL, callback, IntPtr.Zero, (uint)AppDomain.GetCurrentThreadId());
	}

	private int MyCallbackFunction(int code, IntPtr wParam, IntPtr lParam)
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
}
*/
