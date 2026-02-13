using System.Collections.Generic;
using Love;
using PortalMouse.Love.Utils.Ext;

namespace PortalMouse.Love.Utils;

public class TRS {
	public required RectangleF LocalRect;
	public required Vector2 Anchor;
	public required Vector2 Pivot;

	private readonly List<TRS> m_children = new();

	private RectangleF? m_globalRectCache;

	public RectangleF GlobalRect {
		get {
			if (m_globalRectCache.HasValue) return m_globalRectCache.Value;

			Vector2 pos = LocalRect.Lerp(-Pivot);
			if (Parent != null) {
				pos += Parent.GlobalRect.Lerp(Anchor);
			}

			RectangleF result = new(pos, LocalRect.Size);
			m_globalRectCache = result;
			return result;
		}
	}

	public TRS(TRS? parent) {
		Parent = parent;
	}

	public TRS? Parent {
		get;
		set {
			if (field == value) return;

			field?.m_children.Remove(this);
			field = value;
			field?.m_children.Add(this);

			MarkDirty();
		}
	}

	public IReadOnlyList<TRS> Children => m_children;

	public void MarkDirty() {
		m_globalRectCache = null;
		foreach (var child in Children) {
			child.MarkDirty();
		}
	}
}
