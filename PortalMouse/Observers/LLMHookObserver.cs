using PortalMouse.Hooking;
using PortalMouse.Native;
using PortalMouse.Utils.Math;
using PortalMouse.Utils.Misc;
using System.Runtime.InteropServices;

namespace PortalMouse.Observers;

public class LLMHookObserver : MouseObserver
{
	private readonly IntPtr m_llMouseHookHandle = IntPtr.Zero;
	private readonly HookHandler hookHandler = new();

	public LLMHookObserver(Func<V2I, V2I?> callback) : base(callback)
	{
		hookHandler.SetHook(HookType.WH_MOUSE_LL, HookCallback);
	}

	private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
	{
		if (
			nCode >= 0 &&
			lParam != IntPtr.Zero &&
			((uint)wParam & User32.WmMouseMove) != 0
		)
		{
			User32.MSLLHOOKSTRUCT hookStruct = (User32.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(User32.MSLLHOOKSTRUCT))!;

			V2I? movedPos = m_callback((V2I)hookStruct.pt);
			if (movedPos.HasValue)
			{
				NativeHelper.CursorPos = movedPos.Value;

				//Return 1, that's what LBM does at least...
				//https://github.com/mgth/LittleBigMouse/blob/a327a1aa3d7e2c015c594449b687c23d57a54503/LittleBigMouse.Daemon/LittleBigMouse.Hook/HookerMouse.cpp#L75
				return (IntPtr)1;
			}
		}

		return User32.CallNextHookEx(m_llMouseHookHandle, nCode, wParam, lParam);
	}

	protected override void ReleaseUnmanagedResources()
	{
		hookHandler.UnsetHook();
	}
}
