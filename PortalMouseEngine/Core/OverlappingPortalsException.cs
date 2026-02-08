using System;

namespace PortalMouse.Engine.Core {
	[Serializable]
	public class OverlappingPortalsException : Exception {
		private readonly Portal aPortal;
		private readonly Portal bPortal;

		public OverlappingPortalsException(Portal aPortal, Portal bPortal) {
			this.aPortal = aPortal;
			this.bPortal = bPortal;
		}

		public override string ToString() => $"Overlapping portals '{aPortal.Desc.EdgeRange}' and '{bPortal.Desc.EdgeRange}'. This is not supported.";
	}
}