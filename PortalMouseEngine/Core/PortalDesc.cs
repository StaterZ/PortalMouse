namespace PortalMouse.Engine.Core;

public record struct PortalDesc(EdgeRange EdgeRange, int EdgeBarrier) {
	public override string? ToString() {
		string result = EdgeRange.ToString();
		if (EdgeBarrier > 0) {
			result += $" (barrier:{EdgeBarrier})";
		}
		return result;
	}
}
