using PortalMouse.Engine.Core;
using PortalMouse.Engine.Utils.Ext;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;
using System.IO;
using System.Text;
using System.Text.Json;
using Screen = PortalMouse.Engine.Core.Screen;

namespace PortalMouse.Frontend;

public static class FrontendUtils {
	public static void PrintSetupStats(Setup setup) {
		Terminal.Inf("Detected Screens:");
		if (setup.Screens.Count > 0) {
			StringBuilder builder = new();
			foreach (Screen screen in setup.Screens) {
				builder.Append($"    {screen.Id} : {screen.PhysicalRect} @ {(float)(screen.Scale * 100)}%");
				if (screen.Scale != Frac.One) {
					builder.Append($" -> {screen.LogicalRect}");
				}
				Terminal.Inf(builder.ToString());
				builder.Clear();
			}
		} else {
			Terminal.Wrn($"    None???");
		}
		Terminal.BlankLine();
	}

	public static Config? LoadConfig(string path) {
		if (!File.Exists(path)) {
			Terminal.Err($"Path '{path}' not found, aborting");
			return null;
		}

		string configText = File.ReadAllText(path);
		Config? config = JsonSerializer.Deserialize<Config>(configText);
		if (config == null) {
			Terminal.Err("Failed to parse config, aborting");
			return null;
		}

		return config;
	}

	public static bool ApplyConfig(Config config, Setup setup) {
		if (config.Mappings.Length <= 0) {
			Terminal.Wrn("No mappings present in config!");
			return true;
		}

		foreach (Config.Mapping mapping in config.Mappings) {
			bool TryParseScreen(int screenId, out Screen screen) {
				Screen? foundScreen = setup.Screens.FirstOrDefault(screen => screen.Id == screenId);
				if (foundScreen == null) {
					Terminal.Err($@"Screen id out of range. '{screenId}' supplied, but valid ids are: {setup.Screens.Aggregate(new StringBuilder(), (builder, screen) => {
						if (builder.Length > 0) builder.Append(", ");
						builder.Append(screen.Id);
						return builder;
					})}, aborting");
					screen = default!;
					return false;
				}

				screen = foundScreen;
				return true;
			}

			bool TryParseRange(Config.EdgeRange edgeRange, Edge edge, out R1I range) {
				bool TryParseAnchor(string anchorStr, Edge edge, out int anchor) {
					R1I validPixelRange = new(0, edge.Length); //begin is 0 since this is in local space
					R1I validPercentRange = new(0, 100);

					if (anchorStr.EndsWith("px")) {
						if (!int.TryParse(anchorStr[..^2], out int value)) {
							Terminal.Err($"Failed to parse anchor. '{anchorStr}' supplied, but int is malformed");
							anchor = default;
							return false;
						}

						if (
							value < validPixelRange.Begin ||
							value > validPixelRange.End
						) {
							Terminal.Err($"Anchor is out of range. '{anchorStr}' supplied, but valid range is {validPixelRange.Begin}px-{validPixelRange.End}px");
							anchor = default;
							return false;
						}

						anchor = value;
						return true;
					}
					if (anchorStr.EndsWith("%")) {
						if (!int.TryParse(anchorStr[..^1], out int value)) {
							Terminal.Err($"Failed to parse anchor. '{anchorStr}' supplied, but int is malformed");
							anchor = default;
							return false;
						}

						if (
							value < validPercentRange.Begin ||
							value > validPercentRange.End
						) {
							Terminal.Err($"Anchor is out of range. '{anchorStr}' supplied, but valid range is {validPercentRange.Begin}%-{validPercentRange.End}%");
							anchor = default;
							return false;
						}

						anchor = MathX.Map(value, validPercentRange, validPixelRange);
						return true;
					}

					anchor = default;
					return false;
				}

				const string beginDefault = "0%";
				if (!TryParseAnchor(edgeRange.Begin ?? beginDefault, edge, out int begin)) {
					range = default;
					return false;
				}

				const string endDefault = "100%";
				if (!TryParseAnchor(edgeRange.End ?? endDefault, edge, out int end)) {
					range = default;
					return false;
				}

				range = new R1I(begin, end);
				return true;
			}

			if (!TryParseScreen(mapping.A.Screen, out Screen aScreen)) return false;
			Edge aEdge = aScreen.GetEdge(mapping.A.Side);
			if (!TryParseRange(mapping.A, aEdge, out R1I aRange)) return false;

			if (!TryParseScreen(mapping.B.Screen, out Screen bScreen)) return false;
			Edge bEdge = bScreen.GetEdge(mapping.B.Side);
			if (!TryParseRange(mapping.B, bEdge, out R1I bRange)) return false;

			if (mapping.A.Side != mapping.B.Side.Opposite()) throw new ConfigException($"The portals A and B need to be on opposite sides. A is '{mapping.A.Side}', B is '{mapping.B.Side}'. This means A needs to be '{mapping.B.Side.Opposite()}' OR B needs to be'{mapping.A.Side.Opposite()}'");

			Terminal.Inf($"Mapping 'screen{mapping.A.Screen} {aEdge.Side} [{aRange.Begin}-{aRange.End}]' to 'screen{mapping.B.Screen} {bEdge.Side} [{bRange.Begin}-{bRange.End}]'");
			Portal.Bind(
				new EdgeSpan(aEdge, aRange),
				new EdgeSpan(bEdge, bRange)
			);
		}

		return true;
	}
}
