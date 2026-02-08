using System;

namespace PortalMouse.Engine.Utils.Math;

public struct R2Frac {
	public static readonly R2Frac Zero = new(V2Frac.Zero, V2Frac.Zero);
	public static readonly R2Frac One = new(V2Frac.Zero, V2Frac.One);
	public static readonly R2Frac Half = new(V2Frac.Zero, V2Frac.Half);

	//TODO: why is this pos,size when R1Frac is min,max? wonky...
	public V2Frac Pos;
	public V2Frac Size;
	
	public readonly V2Frac Max => Pos + Size;

	public readonly R1Frac X => R1Frac.InitBeginSize(Pos.x, Size.x);
	public readonly R1Frac Y => R1Frac.InitBeginSize(Pos.y, Size.y);

	public R2Frac(V2Frac pos, V2Frac size) {
		Pos = pos;
		Size = size;
	}

	public readonly bool Contains(V2Frac p) {
		p -= Pos;
		return
			p.x >= 0 && p.x <= Size.x &&
			p.y >= 0 && p.y <= Size.y;
	}

	public readonly R1Frac this[Axis axis] => R1Frac.InitBeginSize(Pos[axis], Size[axis]);

	public readonly override string ToString() => $"[X:{Pos.x},Y:{Pos.y},W:{Size.x},H:{Size.y}]";
	public readonly bool Equals(R2Frac other) => Pos == other.Pos && Size == other.Size;
	public readonly override bool Equals(object? obj) => obj is R2Frac other && Equals(other);
	public readonly override int GetHashCode() => HashCode.Combine(Pos, Size);
	
	public static R2Frac operator +(R2Frac lhs, V2Frac rhs) => new(lhs.Pos + rhs, lhs.Size);
	public static R2Frac operator -(R2Frac lhs, V2Frac rhs) => new(lhs.Pos - rhs, lhs.Size);
	
	public static implicit operator R2Frac(R2I self) => new(self.Pos, self.Size);
	public static explicit operator R2I(R2Frac self) => new((V2I)self.Pos, (V2I)self.Size);
}
