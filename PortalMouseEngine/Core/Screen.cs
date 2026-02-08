using System;
using System.Collections.Generic;
using System.Linq;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Engine.Utils.Misc;

namespace PortalMouse.Engine.Core;

public sealed class Screen {
	public readonly int Id;
	public readonly string Name;
	public readonly R2I LogicalRect;
	public readonly Frac Scale;

	public readonly Edge Left;
	public readonly Edge Right;
	public readonly Edge Top;
	public readonly Edge Bottom;

	public R2I PhysicalRect => new(LogicalRect.Pos, (V2I)((V2Frac)LogicalRect.Size * Scale));

	public IEnumerable<Edge> Edges => [Left, Right, Top, Bottom];

	private Screen(string name) {
		Name = name;

		Left = new Edge(this, Side.Left);
		Right = new Edge(this, Side.Right);
		Top = new Edge(this, Side.Top);
		Bottom = new Edge(this, Side.Bottom);
	}

	public Screen(int id, R2I logicalRect, Frac scale, string name) : this(name) {
		Id = id;
		LogicalRect = logicalRect;
		Scale = scale;
	}

	internal Screen(ScreenDesc screenDesc) : this(screenDesc.FriendlyName) {
		if (screenDesc.DisplayDevice != null) {
			string deviceId = screenDesc.DisplayDevice.Value.DeviceID;
			string idStr = deviceId[(deviceId.LastIndexOf('\\')+1)..];
			if (!int.TryParse(idStr, out Id)) throw new FormatException($"Failed to parse display id. Bad int parse. DeviceID was '{idStr}'");
		} else { //Parse out id
			const string idPrefix = @"\\.\DISPLAY";
			string szDevice = screenDesc.MonitorInfo.szDevice;
			if (!szDevice.StartsWith(idPrefix)) throw new FormatException($"Failed to parse monitor id. Bad prefix. szDevice was '{szDevice}'");

			string idStr = szDevice[idPrefix.Length..];
			if (!int.TryParse(idStr, out Id)) throw new FormatException($"Failed to parse monitor id. Bad int parse. szDevice was '{szDevice}'");
		}

		LogicalRect = (R2I)screenDesc.MonitorInfo.rcMonitor;
		Scale = screenDesc.Scale;
	}

	public ScreenLineSeg? TryHandle(LineSeg2Frac mouseMove) => Edges
		.Select(edge => edge.TryHandle(mouseMove))
		.FirstOrDefault(result => result != null);


	public Edge GetEdge(Side side) {
		return side switch {
			Side.Left => Left,
			Side.Right => Right,
			Side.Top => Top,
			Side.Bottom => Bottom,
			_ => throw new UnreachableException(),
		};
	}
}
