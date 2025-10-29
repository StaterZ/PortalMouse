using PortalMouse.Engine.Core;
using System.Text.Json.Serialization;

namespace PortalMouse.Frontend;

public class Config {
	[JsonInclude, JsonPropertyName("mappings")] public Mapping[] Mappings = null!;

	public class Mapping {
		[JsonInclude, JsonPropertyName("a")] public PortalEdge A = null!;
		[JsonInclude, JsonPropertyName("b")] public PortalEdge B = null!;
	}

	public class PortalEdge {
		[JsonInclude, JsonPropertyName("screen")] public int Screen;
		[JsonInclude, JsonPropertyName("side"), JsonConverter(typeof(JsonStringEnumConverter))] public Side Side;
		[JsonInclude, JsonPropertyName("barrier")] public int? Barrier;
		[JsonInclude, JsonPropertyName("begin")] public string? Begin;
		[JsonInclude, JsonPropertyName("end")] public string? End;
	}
}
