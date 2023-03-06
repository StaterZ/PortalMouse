using System.Text.Json.Serialization;

namespace MouseControllerThing;

public class Config {
	[JsonInclude] public Mapping[] mappings = null!;

	public class Mapping {
		[JsonInclude] public Edge a = null!;
		[JsonInclude] public Edge b = null!;
	}

	public class Edge {
		[JsonInclude] public int screen = -1;
		[JsonInclude, JsonConverter(typeof(JsonStringEnumConverter))] public Side side;
		[JsonInclude] public int? begin;
		[JsonInclude] public int? end;
	}
}