using MouseControllerThing.Native;
using System.Diagnostics;

namespace MouseControllerThing.Testing;

//reference:
//https://github.com/MouseUnSnag/MouseUnSnag/blob/master/MouseUnSnag/Hooking/HookHandler.cs

public class HookHandler {
	private IntPtr moduleHandle = IntPtr.Zero;
	private IntPtr hookHandle = IntPtr.Zero;

	public void SetHook(HookType hookType, User32.HookProc proc) {
		if (hookHandle != IntPtr.Zero)
			throw new InvalidOperationException();

		using Process currentProcess = Process.GetCurrentProcess();
		using ProcessModule currentModule = currentProcess.MainModule ?? throw new NullReferenceException("Main Module not found");
		if (moduleHandle == IntPtr.Zero) {
			moduleHandle = Kernel32.GetModuleHandle(currentModule.ModuleName);
		}

		hookHandle = User32.SetWindowsHookEx(hookType, proc, moduleHandle, 0);
	}

	public void UnsetHook() {
		if (hookHandle == IntPtr.Zero)
			return;

		User32.UnhookWindowsHookEx(hookHandle);
		hookHandle = IntPtr.Zero;
	}
}