namespace PortalMouse.Engine.Utils.Misc;

[Serializable]
public class UnreachableException : Exception {
	public UnreachableException() { }

	public UnreachableException(string? message) : base(message) { }

	public UnreachableException(string? message, Exception? innerException) : base(message, innerException) { }
}
