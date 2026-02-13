using System;
using Love;

namespace PortalMouse.Love.Utils.Ext;

public static class SizeFExt {
	extension(SizeF self) {
		public float Area() => self.Width * self.Height;
		public SizeF Floor() => new(Mathf.Floor(self.Width), Mathf.Floor(self.Height));
		public SizeF FitInside(SizeF contained) {
			float scale = Math.Min(self.Width / contained.Width, self.Height / contained.Height);
			return contained * scale;
		}
	}
}
