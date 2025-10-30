namespace PortalMouse.Love;

public static class Selection {
	public static bool IsConsumed { get; private set; }

	public static bool Consume() => !IsConsumed && (IsConsumed = true);
	public static void Reset() => IsConsumed = false;
}
