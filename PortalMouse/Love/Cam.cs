using Love;

namespace PortalMouse.Love;

public class Cam {
	public required Config Cfg;
	public Vector2 Pos;
	public float Scale = 1;
	private bool m_selected;

	public void Begin() {
		Graphics.Push();
		Graphics.Translate((Vector2)(SizeF)Graphics.GetDimensions() * 0.5f);
		Graphics.Scale(Scale);
		Graphics.Translate(-Pos);
	}

	public void End() {
		Graphics.Pop();
	}

	public void Update() {
		{
			float prevScale = Scale;
			Scale *= Mathf.Pow(Mathf.Pow(2f, 1f / 3f), Mouse.GetScrollY() * Cfg.ZoomSpeed);
			Vector2 centerMouse = Mouse.GetPosition() - (Vector2)(SizeF)Graphics.GetDimensions() * 0.5f;
			Pos -= centerMouse / Scale - centerMouse / prevScale;
		}

		if (m_selected) {
			if ((Mouse.IsReleased(0) || Mouse.IsReleased(1)) && Selection.Consume()) {
				m_selected = false;
			}
		} else if ((Mouse.IsPressed(0) || Mouse.IsPressed(1)) && Selection.Consume()) {
			m_selected = true;
		}

		if (m_selected) {
			Vector2 delta = Mouse.GetPosition() - Mouse.GetPreviousPosition();
			Pos -= delta / Scale;
		}
	}

	public Vector2 ViewToWorld(Vector2 pos) => pos / Scale + Pos;

	public class Config {
		public required float ZoomSpeed;
	}
}
