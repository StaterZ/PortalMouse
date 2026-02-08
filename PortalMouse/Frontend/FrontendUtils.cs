using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using PortalMouse.Engine.Core;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Frontend;

public static class FrontendUtils {
	public static void PrintSetupStats(Setup setup) {
		Terminal.Inf("Detected Screens:");
		if (setup.Screens.Count > 0) {
			StringBuilder builder = new();
			static string GetUniqueName(Screen screen) => $"{screen.Id} ({screen.Name})";
			int maxUniqueNameLength = setup.Screens.Max(screen => GetUniqueName(screen).Length);
			foreach (Screen screen in setup.Screens) {
				builder.Append($"    {GetUniqueName(screen).PadRight(maxUniqueNameLength)} : {screen.PhysicalRect} @ {(float)(screen.Scale * 100)}%");
				if (screen.Scale != Frac.One) {
					builder.Append($" -> {screen.LogicalRect}");
				}
				Terminal.Inf(builder.ToString());
				builder.Clear();
			}
		} else {
			Terminal.Wrn("    None???");
		}
		Terminal.BlankLine();
	}

	public static Config? LoadConfig(string path) {
		if (!File.Exists(path)) {
			Terminal.Err($"Path '{path}' not found");
			return null;
		}

		string configText = File.ReadAllText(path);
		Config? config = JsonSerializer.Deserialize<Config>(configText);
		if (config == null) {
			Terminal.Err("Failed to parse config");
			return null;
		}

		return config;
	}

	public static void ApplyConfig(Config config, Setup setup) {
		if (config.Mappings.Length <= 0) {
			Terminal.Wrn("No mappings present in config!");
			return;
		}

		foreach (Config.Mapping mapping in config.Mappings) {
			PortalDesc? TryParsePortalEdge(Config.PortalEdge portalEdge) {
				Screen? TryParseScreen(int screenId) {
					Screen? foundScreen = setup.Screens.FirstOrDefault(screen => screen.Id == screenId);
					if (foundScreen != null) return foundScreen;

					Terminal.Err($@"Screen id out of range. '{screenId}' supplied, but valid ids are: {setup.Screens.Aggregate(new StringBuilder(), (builder, screen) => {
						if (builder.Length > 0) {
							builder.Append(", ");
						}
						builder.Append(screen.Id);
						return builder;
					})}");
					return null;
				}

				R1I? TryParseRange(Edge edge) {
					bool TryParseAnchor(string anchorStr, Edge edge, out int anchor) {
						R1I validPixelRange = new(0, edge.ScreenRangeAlongEdgeAxis.Size); //begin is 0 since this is in local space
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
					if (!TryParseAnchor(portalEdge.Begin ?? beginDefault, edge, out int begin)) {
						return null;
					}

					const string endDefault = "100%";
					if (!TryParseAnchor(portalEdge.End ?? endDefault, edge, out int end)) {
						return null;
					}

					if (begin > end) throw new ConfigException($"Begin({begin}) can't be larger the end({end})");

					return new R1I(begin, end);
				}

				Screen? screen = TryParseScreen(portalEdge.Screen);
				if (screen == null) return null;

				Edge edge = screen.GetEdge(portalEdge.Side);
				R1I? range = TryParseRange(edge);
				if (!range.HasValue) return null;

				EdgeRange edgeRange = new(edge, range.Value);
				return new PortalDesc(edgeRange, portalEdge.Barrier ?? 0);
			}

			PortalDesc? a = TryParsePortalEdge(mapping.A);
			if (a == null) continue;

			PortalDesc? b = TryParsePortalEdge(mapping.B);
			if (b == null) continue;

			if (a.Value.EdgeRange.Edge.Side == b.Value.EdgeRange.Edge.Side) throw new ConfigException($"The portals A and B need to be on different sides. Both A and B is '{a.Value.EdgeRange.Edge.Side}'");

			Terminal.Inf($"Mapping '{a}' to '{b}'");
			Portal.Bind(a.Value, b.Value);
		}
	}
}
