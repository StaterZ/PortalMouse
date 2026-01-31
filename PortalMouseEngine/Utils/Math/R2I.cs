using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PortalMouse.Engine.Utils.Math;

public struct R2I {
	public static readonly R2I Zero = new(V2I.Zero, V2I.Zero);
	public static readonly R2I One = new(V2I.Zero, V2I.One);

	//TODO: why is this pos,size when R1I is min,max? wonky...
	public V2I Pos;
	public V2I Size;

	public readonly R1I X => R1I.InitBeginSize(Pos.x, Size.x);
	public readonly R1I Y => R1I.InitBeginSize(Pos.y, Size.y);

	public R2I(V2I pos, V2I size) {
		Pos = pos;
		Size = size;
	}

	public readonly bool Contains(V2I p) {
		p -= Pos;
		return
			p.x >= 0 && p.x < Size.x &&
			p.y >= 0 && p.y < Size.y;
	}

	public readonly bool Contains(V2Frac p) {
		p -= Pos;
		return
			p.x >= 0 && p.x < Size.x &&
			p.y >= 0 && p.y < Size.y;
	}

	public override readonly string ToString() =>
		$"[X:{Pos.x},Y:{Pos.y},W:{Size.x},H:{Size.y}]";

	public readonly R1I this[Axis axis] => R1I.InitBeginSize(Pos[axis], Size[axis]);
}
