﻿namespace MouseControllerThing.Utils;

public static class MyMath {
	public static int Map(int value, R1I from, R1I to) {
		return (int)((value - from.Begin) / (float)(from.End - from.Begin) * (to.End - to.Begin)) + to.Begin;
	}
	
	public static V2I Map(V2I value, R2I from, R2I to) {
		return (value - from.Pos) * to.Size / from.Size + to.Pos;
	}

	public static V2I MapVec(V2I value, R2I from, R2I to) {
		return value * to.Size / from.Size;
	}
}
