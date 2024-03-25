﻿using MouseControllerThing.Native;

namespace MouseControllerThing.Utils;

public struct R2I {
	public V2I Pos;
	public V2I Size;

	public R2I(V2I pos, V2I size) {
		Pos = pos;
		Size = size;
	}

	public R2I(User32.Rect rect) : this(
		new V2I(rect.Left, rect.Top),
		new V2I(rect.Right - rect.Left, rect.Bottom - rect.Top)
	) { }

	public readonly bool Contains(V2I p) {
		p -= Pos;
		return
			p.x >= 0 && p.x < Size.x &&
			p.y >= 0 && p.y < Size.y;
	}

	public override string ToString() => $"[X:{Pos.x},Y:{Pos.y},W:{Size.x},H:{Size.y}]";
}