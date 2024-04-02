using System.Text.Json.Serialization;

namespace MouseControllerThing.Core;

public class Config {
	[JsonInclude, JsonPropertyName("portals")] public Portal[] Portals = null!;

	public class Portal {
		[JsonInclude, JsonPropertyName("a")] public EdgeRange A = null!;
		[JsonInclude, JsonPropertyName("b")] public EdgeRange B = null!;
	}

	public class EdgeRange {
		[JsonInclude, JsonPropertyName("screen")] public int Screen;
		[JsonInclude, JsonPropertyName("side"), JsonConverter(typeof(JsonStringEnumConverter))] public Side Side;
		[JsonInclude, JsonPropertyName("begin")] public int? Begin;
		[JsonInclude, JsonPropertyName("end")] public int? End;
	}
}
