using System.Collections.Generic;
using System.Text;

namespace PortalMouse.Engine.Utils.Ext;

public static class IEnumerableExt {
	public static IEnumerable<(int index, T item)> Enumerate<T>(this IEnumerable<T> self) {
		int i = 0;
		foreach (T item in self) {
			yield return (i, item);
			i++;
		}
	}
	
	public static string Delimit<T>(this IEnumerable<T> self, string sep) {
		StringBuilder builder = new();
		foreach ((int index, T item) in self.Enumerate()) {
			if (index > 0) {
				builder.Append(sep);
			}
			
			builder.Append(item);
		}
		return builder.ToString();
	}
}
