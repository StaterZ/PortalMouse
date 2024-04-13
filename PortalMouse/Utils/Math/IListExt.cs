namespace PortalMouse.Utils.Math;

public static class IListExt {
	/// <summary>
	/// If the value is found then `success = true` is returned with the index of the matching element.
	/// If the value is not found then `success = false` is returned with the index where a matching element could be inserted while maintaining sorted order.
	/// </summary>
	public static (bool success, int index) BetterBinarySearch<T, TKey>(this IList<T> list, TKey key, Func<T, TKey> keySelector)
		where TKey : IComparable<TKey>
	{
		//algorithm based off rust std impl: lib\rustlib\src\rust\library\core\src\slice\mod.rs:2827 - 'binary_search_by'

		int min = 0; //inclusive bound
		int max = list.Count; //exclusive bound
		while (min < max) {
			int size = max - min;
			int mid = min + size / 2;

			T midItem = list[mid];
			TKey midKey = keySelector(midItem);

			switch (midKey.CompareTo(key)) {
				case < 0: // midKey < key
					min = mid + 1;
					break;
				case > 0: // midKey > key
					max = mid;
					break;
				default:  // midKey == key
					return (true, mid);
			}
		}

		return (false, min);
	}
}
