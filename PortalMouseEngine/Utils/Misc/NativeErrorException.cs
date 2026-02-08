using System;

namespace PortalMouse.Engine.Utils.Misc;

[Serializable]
public class NativeErrorException : Exception {
	public NativeErrorException() { }

	public NativeErrorException(string? message) : base(message) { }

	public NativeErrorException(string? message, Exception? innerException) : base(message, innerException) { }
}
