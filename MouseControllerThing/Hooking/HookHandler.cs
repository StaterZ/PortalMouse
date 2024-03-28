using MouseControllerThing.Native;
using System.Diagnostics;

namespace MouseControllerThing.Hooking;

//reference:
//https://github.com/MouseUnSnag/MouseUnSnag/blob/master/MouseUnSnag/Hooking/HookHandler.cs

public class HookHandler {
	private IntPtr m_moduleHandle = IntPtr.Zero;
	private IntPtr m_hookHandle = IntPtr.Zero;

	public void SetHook(HookType hookType, User32.HookProc proc) {
		if (m_hookHandle != IntPtr.Zero)
			throw new InvalidOperationException();

		using Process currentProcess = Process.GetCurrentProcess();
		using ProcessModule currentModule = currentProcess.MainModule ?? throw new NullReferenceException("Main Module not found");
		if (m_moduleHandle == IntPtr.Zero) {
			string moduleName = currentModule.ModuleName ?? throw new NullReferenceException("Main Module has no name");
			m_moduleHandle = Kernel32.GetModuleHandle(moduleName);
		}

		m_hookHandle = User32.SetWindowsHookEx(hookType, proc, m_moduleHandle, 0);
	}

	public void UnsetHook() {
		if (m_hookHandle == IntPtr.Zero)
			throw new InvalidOperationException();

		User32.UnhookWindowsHookEx(m_hookHandle);
		m_hookHandle = IntPtr.Zero;
	}
}
