﻿using PortalMouse.Engine.Hooking;
using PortalMouse.Engine.Native;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;
using System.Runtime.InteropServices;

namespace PortalMouse.Engine.Observers;

public class LLMHookObserver : MouseObserver {
	private readonly IntPtr m_llMouseHookHandle = IntPtr.Zero;
	private readonly HookHandler m_hookHandler = new();

	public LLMHookObserver(Func<V2I, V2I?> callback, Action<Exception> exceptionHandler) : base(callback, exceptionHandler) {
		m_hookHandler.SetHook(HookType.WH_MOUSE_LL, HookCallback);
	}

	private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
		if (
			nCode >= 0 &&
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
				m_hookHandler.UnsetHook();
				m_exceptionHandler(ex);
			}
		}

		return User32.CallNextHookEx(m_llMouseHookHandle, nCode, wParam, lParam);
	}

	protected override void ReleaseUnmanagedResources() {
		if (m_hookHandler.IsHooked) {
			m_hookHandler.UnsetHook();
		}

		base.ReleaseUnmanagedResources();
	}
}
