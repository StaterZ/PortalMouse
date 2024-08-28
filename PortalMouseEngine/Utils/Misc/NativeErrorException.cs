using System.Runtime.Serialization;

namespace PortalMouse.Engine.Utils.Misc;

[Serializable]
public class NativeErrorException : Exception {
	public NativeErrorException() { }

	public NativeErrorException(string? message) : base(message) { }

	public NativeErrorException(string? message, Exception? innerException) : base(message, innerException) { }

	protected NativeErrorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
