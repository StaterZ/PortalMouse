﻿using System.Collections;

namespace MouseControllerThing.Utils.Ext {
	public static class IListExt {
		public static bool IsInRange(this IList self, int index) => index >= 0 && index < self.Count;
	}
}
