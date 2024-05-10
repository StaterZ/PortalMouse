using PortalMouse.Native;
using PortalMouse.Utils.Misc;

namespace PortalMouse.Hooking;

//reference:
//https://github.com/MouseUnSnag/MouseUnSnag/blob/master/MouseUnSnag/Hooking/HookHandler.cs

internal class HookHandler {
	private User32.LowLevelMouseProc? m_hookCallback; //required to keep memory alive
	private IntPtr m_hookHandle = IntPtr.Zero;

	public bool IsHooked => m_hookHandle != IntPtr.Zero;

	public bool SetHook(HookType hookType, User32.LowLevelMouseProc proc) {
		if (IsHooked)
			throw new InvalidOperationException("Hook already set");

		m_hookCallback = proc; //required to keep memory alive
		m_hookHandle = User32.SetWindowsHookEx(hookType, m_hookCallback, IntPtr.Zero, 0);
		if (!IsHooked) {
			m_hookCallback = null;
			return false;
		}

		return true;
	}

	public void UnsetHook() {
		if (!IsHooked)
			throw new InvalidOperationException();

		NativeHelper.AssertSuccess(User32.UnhookWindowsHookEx(m_hookHandle), nameof(User32.UnhookWindowsHookEx));
		m_hookHandle = IntPtr.Zero;
		m_hookCallback = null;
	}
}
