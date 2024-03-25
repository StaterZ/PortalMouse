using MouseControllerThing.Native;
using System.Runtime.InteropServices;

namespace MouseControllerThing.Hooking;

public class LLMouseHook : IDisposable
{
	private readonly User32.HookProc? m_hookCallback = null!; //required to keep memory alive
	private readonly IntPtr m_llMouseHookHandle = IntPtr.Zero;
	private readonly HookHandler hookHandler = new();
	public Action<Point> managedCallback;

	public LLMouseHook(Action<Point> callback) {
		managedCallback = callback;

		m_hookCallback = LlMouseHookCallback; //required to keep memory alive
		hookHandler.SetHook(HookType.WH_MOUSE_LL, m_hookCallback); //Setup a mouse hook
	}

	~LLMouseHook() {
		ReleaseUnmanagedResources();
	}

	private IntPtr LlMouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
		if (nCode == 0 && (uint)wParam == User32.WmMouseMove) {
			User32.Msllhookstruct hookStruct = (User32.Msllhookstruct)Marshal.PtrToStructure(lParam, typeof(User32.Msllhookstruct))!;
			managedCallback(hookStruct.pt);
		}

		return User32.CallNextHookEx(m_llMouseHookHandle, nCode, wParam, lParam);
	}

	private void ReleaseUnmanagedResources() {
		hookHandler.UnsetHook();
	}

	public void Dispose() {
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}
}
