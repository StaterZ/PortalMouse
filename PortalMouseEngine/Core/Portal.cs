using PortalMouse.Engine.Utils.Math;

namespace PortalMouse.Engine.Core;

public class Portal {
	public readonly PortalDesc Desc;
	public Portal Exit;

	private Portal(PortalDesc desc, Portal exit) {
		Desc = desc;
		Exit = exit;
	}

	public Frac Map(Frac value) =>
		MathX.Map(value, Desc.EdgeRange.Range, Exit.Desc.EdgeRange.Range);

	public static void Bind(PortalDesc a, PortalDesc b) {
		Portal aPortal = new(a, null!);
		Portal bPortal = new(b, null!);

		aPortal.Exit = bPortal;
		bPortal.Exit = aPortal;

		if (
			!aPortal.Desc.EdgeRange.Edge.Add(aPortal) ||
			!bPortal.Desc.EdgeRange.Edge.Add(bPortal)
		) throw new OverlappingPortalsException(aPortal, bPortal);
	}
}
