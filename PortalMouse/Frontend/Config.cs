using PortalMouse.Core;
using System.Text.Json.Serialization;

namespace PortalMouse.Frontend;

public class Config {
	[JsonInclude, JsonPropertyName("mappings")] public Mapping[] Mappings = null!;

	public class Mapping {
		[JsonInclude, JsonPropertyName("a")] public EdgeRange A = null!;
		[JsonInclude, JsonPropertyName("b")] public EdgeRange B = null!;
	}

	public class EdgeRange {
		[JsonInclude, JsonPropertyName("screen")] public int Screen;
		[JsonInclude, JsonPropertyName("side"), JsonConverter(typeof(JsonStringEnumConverter))] public Side Side;
		[JsonInclude, JsonPropertyName("begin")] public string? Begin;
		[JsonInclude, JsonPropertyName("end")] public string? End;
	}
}
