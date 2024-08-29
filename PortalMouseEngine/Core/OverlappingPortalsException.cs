using System.Collections;

namespace PortalMouse.Engine.Core {
	[Serializable]
	internal class OverlappingPortalsException : Exception {
		private readonly Portal aPortal;
		private readonly Portal bPortal;

		public OverlappingPortalsException(Portal aPortal, Portal bPortal) {
			this.aPortal = aPortal;
			this.bPortal = bPortal;
		}

		public override IDictionary Data => base.Data;

		public override string Message => base.Message;

		public override string ToString() {
			return $"Overlapping portals '{aPortal.EdgeSpan}' and '{bPortal.EdgeSpan}'. This is not supported.";
		}
	}
}