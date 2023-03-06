using System.Text.Json.Serialization;

namespace MouseControllerThing.Core;

public class Config {
	[JsonInclude, JsonRequired] public Mapping[] mappings = null!;

	public class Mapping {
		[JsonInclude, JsonRequired] public EdgeRange a = null!;
		[JsonInclude, JsonRequired] public EdgeRange b = null!;
	}

	public class EdgeRange {
		[JsonInclude, JsonRequired] public int screen;
		[JsonInclude, JsonRequired, JsonConverter(typeof(JsonStringEnumConverter))] public Side side;
		[JsonInclude] public int? begin;
		[JsonInclude] public int? end;
	}
}
