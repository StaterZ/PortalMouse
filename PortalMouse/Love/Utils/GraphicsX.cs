using System.Linq;
using Love;
using PortalMouse.Engine.Utils.Ext;
using PortalMouse.Love.Utils.Ext;

namespace PortalMouse.Love.Utils;

public static class GraphicsX {
	public static void Print(string text, Vector2 pos, AlignMode alignment, float angle, float size) {
		Font font = Graphics.GetFont();
		float scale = size / font.GetHeight();
		Graphics.Printf(
			text,
			pos.X,
			pos.Y,
			font.GetWidth(text),
			alignment,
			angle,
			scale,
			scale
		);
	}

	public static void PrintCentered(string text, Vector2 pos, AlignMode alignment, float angle, float size) {
		Graphics.Push();
		Graphics.Translate(pos);
		Graphics.Rotate(angle);
		Font font = Graphics.GetFont();
		Graphics.Scale(size / font.GetHeight());

		Size textSize = GetTextSize(text, font);
		Graphics.Translate((Vector2)(Point)textSize / -2);

		//Graphics.Rectangle(DrawMode.Line, new RectangleF(Vector2.Zero, textSize));
		Graphics.Printf(
			text,
			0,
			0,
			textSize.Width,
			alignment
		);
		Graphics.Pop();
	}

	public static void PrintCenteredAutoSize(string text, RectangleF rect, AlignMode alignment, float angle) {
		Font font = Graphics.GetFont();
		Size textSize = GetTextSize(text, font);
		SizeF size = rect.Size.FitInside(textSize);
		int numLines = GetNumLines(text);
		PrintCentered(text, rect.Center, alignment, angle, size.Height / numLines);
	}

	private static Size GetTextSize(string text, Font font) => new(
		font.GetWidth(text),
		font.GetHeight() * GetNumLines(text)
	);

	private static int GetNumLines(string text) => 1 + text.Count(c => c == '\n');

	public static void DrawGizmo(float unitLength) {
		Graphics.SetColor(Color.Red);
		Graphics.Line(new Vector2(0, 0), new Vector2(unitLength, 0));
		Graphics.SetColor(Color.Green);
		Graphics.Line(new Vector2(0, 0), new Vector2(0, unitLength));
		Graphics.SetColor(Color.White);
		Graphics.Circle(DrawMode.Fill, Vector2.Zero, Graphics.GetLineWidth());
	}

	public static void DrawZeroLines(Cam cam) {
		SizeF half = Graphics.GetDimensions() * 0.5f;
		half /= cam.Scale;
		Vector2 pos = cam.Pos;

		//X Axis
		Graphics.Push();
		Graphics.Translate(0, pos.Y);
		Graphics.Line(new Vector2(0, -half.Height), new Vector2(0, half.Height));
		Graphics.Pop();

		//Y Axis
		Graphics.Push();
		Graphics.Translate(pos.X, 0);
		Graphics.Line(new Vector2(-half.Width, 0), new Vector2(half.Width, 0));
		Graphics.Pop();
	}

	public static void DrawGrid(Cam cam, SizeF cellSize) {
		Size viewSize = Graphics.GetDimensions();
		SizeF worldHalf = viewSize * 0.5f;
		worldHalf /= cam.Scale;
		Vector2 worldPos = cam.Pos;
		SizeF viewCellSize = cellSize * cam.Scale;
		Size numCells = Size.Ceiling(new SizeF((Vector2)(SizeF)viewSize / (Vector2)viewCellSize));

		float lwh = Graphics.GetLineWidth() / 2;
		float coverMetric = ((viewCellSize.Width + viewCellSize.Height) * (lwh * 2) - lwh.Sqr() * 4) / viewCellSize.Area(); //percentage of cell covered by surrounding grid lines
		const float k_desiredCoverMetric = 0.1f; //100% alpha at 100 zoom with 0.01 line width. therefore: 0.01 / 100

		Vector4 prevColor = Graphics.GetColor();
		Graphics.SetColor(prevColor with {
			a = Mathf.Clamp01(k_desiredCoverMetric / coverMetric), //as we zoom out, reduce alpha to keep "brightness" constant
		});

		//X Axis Lines
		for (int y = 0; y < numCells.Height; y++) {
			Graphics.Push();
			Graphics.Translate(worldPos.X, ((int)((worldPos.Y - worldHalf.Height) / cellSize.Height) + y) * cellSize.Height);
			Graphics.Line(new Vector2(-worldHalf.Width, 0), new Vector2(worldHalf.Width, 0));
			Graphics.Pop();
		}

		//Y Axis Lines
		for (int x = 0; x < numCells.Width; x++) {
			Graphics.Push();
			Graphics.Translate(((int)((worldPos.X - worldHalf.Width) / cellSize.Width) + x) * cellSize.Width, worldPos.Y);
			Graphics.Line(new Vector2(0, -worldHalf.Height), new Vector2(0, worldHalf.Height));
			Graphics.Pop();
		}

		Graphics.SetColor(prevColor);
	}
}
