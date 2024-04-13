namespace MouseControllerThing.Utils.Maths;

public struct R1I {
	public int Begin;
	public int End;

	public int Size => End - Begin;

	public R1I(int begin, int end) {
		Begin = begin;
		End = end;
	}

	public override string ToString() =>
		$"[X:{Begin},W:{Size}]";

	public bool Contains(int point) =>
		Begin <= point && point < End;

	public int Clamp(int value) =>
		Math.Clamp(value, Begin, Math.Max(Begin, End - 1));

	public static R1I InitBeginSize(int begin, int size) => new(begin, begin + size);
}
