using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Utils.Ext;

public static class FloatExt {
	extension(float self) {
		public float Sqr() => self * self;
		public float Lerp(R1F range) => self * range.Size + range.Begin;
		public float InvLerp(R1F range) => (self - range.Begin) / range.Size;
		public float Map(R1F from, R1F to) => self.InvLerp(from).Lerp(to);
	}
}