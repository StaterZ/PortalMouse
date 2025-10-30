using Love;
using System;

namespace PortalMouse.Love.Utils.Ext;

public static class SizeFExt {
	public static float Area(this SizeF self) => self.Width * self.Height;
	public static SizeF Floor(this SizeF self) => new(Mathf.Floor(self.Width), Mathf.Floor(self.Height));
	public static SizeF FitInside(this SizeF container, SizeF contained) {
		float scale = Math.Min(container.Width / contained.Width, container.Height / contained.Height);
		return contained * scale;
	}
}
