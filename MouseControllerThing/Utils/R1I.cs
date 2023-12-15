namespace MouseControllerThing.Utils;

public struct R1I
{
	public int Begin;
	public int End;

	public int Size => End - Begin;

	public R1I(int begin, int end)
	{
		Begin = begin;
		End = end;
	}

	public override string ToString() =>
		$"[X:{Begin},W:{Size}]";
}
