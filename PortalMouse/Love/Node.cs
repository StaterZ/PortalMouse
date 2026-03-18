using System.Diagnostics;
using Love;
using PortalMouse.Engine.Core;
using PortalMouse.Engine.Utils.Ext;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Love.Utils;
using PortalMouse.Love.Utils.Ext;

namespace PortalMouse.Love;

public class Node {
	public TRS Trs { get; set; }

	public Vector2 Acc;
	public Vector2 Vel;
	public readonly Screen Screen;
	public readonly Graph m_graph;

	public Node(Graph graph, Screen screen) {
		m_graph = graph;
		Screen = screen;
		Trs = new TRS(null) {
			Anchor = default,
			LocalRect = screen.LogicalRect.ToLove(),
			Pivot = new Vector2(0.0f, 0.0f),
		};
	}

	public void Deinit() {
	}

	public void Update(float dt) {
		if (m_graph.Selected == this) {
			if (Mouse.IsDown(0)) {
				Selection.Consume();
			} else {
				m_graph.Selected = null;
			}
		} else if (
			m_graph.Selected == null &&
			Trs.GlobalRect.Contains(m_graph.MouseWorldPos) &&
			Mouse.IsDown(0) &&
			Selection.Consume()
		) {
			m_graph.Selected = this;
		}

		if (m_graph.Selected == this) {
			Trs.LocalRect.Location += m_graph.MouseWorldPos - m_graph.MouseWorldPrevPos;
			Trs.MarkDirty();
		}
	}

	public void Draw0() {
		// RectangleF rect = Trs.GlobalRect;
		// Graphics.SetColor(Color.Blue);
		// RectangleF insetRect = Screen.PhysicalRect.ToLove();
		// insetRect.Inflate(new SizeF(Graphics.GetLineWidth(), Graphics.GetLineWidth()) / -2);
		// Graphics.Rectangle(DrawMode.Line, insetRect);
		//
		// void DrawLine(V2I t) => Graphics.Line(t.Lerp(Screen.PhysicalRect).ToLoveVector(), rect.Lerp(t.ToLoveVector()));
		// DrawLine(new V2I(0, 0));
		// DrawLine(new V2I(1, 0));
		// DrawLine(new V2I(0, 1));
		// DrawLine(new V2I(1, 1));
	}
	
	public void Draw1() {
		RectangleF rect = Trs.GlobalRect;
		Graphics.SetColor(Color.FromARGB(0xff202020));
		Graphics.Rectangle(DrawMode.Fill, rect);

		Graphics.SetColor(Color.White);
		RectangleF insetRect = rect;
		insetRect.Inflate(new SizeF(Graphics.GetLineWidth(), Graphics.GetLineWidth()) / -2);
		Graphics.Rectangle(DrawMode.Line, insetRect);
	}

	public void Draw2() {
		Graphics.SetColor(Color.White);
		RectangleF textRect = Trs.GlobalRect;
		textRect.Inflate(-100, -100);
		GraphicsX.PrintCenteredAutoSize(
			$"{Screen.Name}_{Screen.Id}\n[W{Screen.LogicalRect.Size.x}, H{Screen.LogicalRect.Size.y}]",
			textRect,
			AlignMode.Center,
			0
		);

		foreach (Edge edge in Screen.Edges) {
			foreach (Portal portal in edge.Portals) {
				EdgeRange entryEdgeRange = portal.Desc.EdgeRange;
				EdgeRange exitEdgeRange = portal.Exit.Desc.EdgeRange;

				Vector2[] vertices = [
					m_graph.GetPortalPos(entryEdgeRange, 0),
					m_graph.GetPortalPos(entryEdgeRange, 1),
					m_graph.GetPortalPos(exitEdgeRange, 1),
					m_graph.GetPortalPos(exitEdgeRange, 0),
				];

				bool isBackgroundMissing = m_graph.m_drawnPortals.Add(portal) && m_graph.m_drawnPortals.Add(portal.Exit);
				if (isBackgroundMissing) {
					Color c = Color.Gray;
					c.Af = 0.5f;
					Graphics.SetColor(c);
					Graphics.Polygon(DrawMode.Fill, vertices);
				}

				const float ratio = 0.75f;
				Vector2 textPos = (vertices[0] + vertices[1]) / 2 * ratio + (vertices[2] + vertices[3]) / 2 * (1 - ratio);

				string text = $"Range: [{portal.Desc.EdgeRange.LocalRange.Begin}..{portal.Desc.EdgeRange.LocalRange.End}]";
				if (portal.Desc.EdgeBarrier != 0) {
					text += $"\nBarrier: {portal.Desc.EdgeBarrier}";
				}
				Graphics.SetColor(Color.White);
				GraphicsX.PrintCentered(text, textPos, AlignMode.Center, edge.Side.ToAxis() switch {
					Axis.Horizontal => Mathf.TAU / 4,
					Axis.Vertical => 0,
					_ => throw new UnreachableException(),
				}, 200);
			}
		}
	}
}
