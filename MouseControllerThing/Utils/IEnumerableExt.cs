namespace MouseControllerThing.Utils;

public static class IEnumerableExt {
	public static IEnumerable<(int index, T item)> ZipIndex<T>(this IEnumerable<T> self) {
		int i = 0;
		foreach (T item in self) {
			yield return (i, item);
			i++;
		}
	}
}
