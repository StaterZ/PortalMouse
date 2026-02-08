using System;
using System.Runtime.InteropServices;
using PortalMouse.Engine.Native;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Observers;

public class LLMHookObserver : MouseObserver {
	private readonly User32.LowLevelMouseProc? m_hookCallback; //required to keep memory alive
	private readonly IntPtr m_hookHandle;

	public LLMHookObserver(Func<V2I, V2I?> callback, Action<Exception> exceptionHandler) : base(callback, exceptionHandler) {
		m_hookCallback = HookCallback;
		m_hookHandle = User32.SetWindowsHookEx(User32.HookType.WH_MOUSE_LL, m_hookCallback, IntPtr.Zero, 0);
	}

	private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam) {
		if (
			code >= 0 &&
			lParam != IntPtr.Zero &&
			((uint)wParam & User32.WmMouseMove) != 0
		) {
			try {
				User32.MSLLHOOKSTRUCT hookStruct = (User32.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(User32.MSLLHOOKSTRUCT))!;

				V2I? movedPos = m_callback((V2I)hookStruct.pt);
				if (movedPos.HasValue) {
					NativeHelper.CursorPos = movedPos.Value;

					//Return 1, that's what LBM does at least...
					//https://github.com/mgth/LittleBigMouse/blob/a327a1aa3d7e2c015c594449b687c23d57a54503/LittleBigMouse.Daemon/LittleBigMouse.Hook/HookerMouse.cpp#L75
					return (IntPtr)1;
				}
			} catch(Exception ex) {
				UnsetHook();
				m_exceptionHandler(ex);
			}
		}

		return User32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
	}

	protected override void ReleaseUnmanagedResources() {
		UnsetHook();
		base.ReleaseUnmanagedResources();
	}

	private void UnsetHook() => NativeHelper.AssertSuccess(User32.UnhookWindowsHookEx(m_hookHandle), nameof(User32.UnhookWindowsHookEx));
}
