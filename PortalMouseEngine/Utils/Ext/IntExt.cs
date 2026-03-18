using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Utils.Ext;

public static class IntExt {
	extension(int self) {
		public int Sqr() => self * self;
		public int Lerp(R1I range) => self * range.Size + range.Begin;
		public int InvLerp(R1I range) => (self - range.Begin) / range.Size;
		public int Map(R1I from, R1I to) => (self - from.Begin) * to.Size / from.Size + to.Begin; //we can't decompose this. we need to multiply before we divdie
	}
}
