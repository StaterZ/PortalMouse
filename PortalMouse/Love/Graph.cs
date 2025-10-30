using Love;
using PortalMouse.Engine.Core;
using PortalMouse.Engine.Utils.Ext;
using PortalMouse.Engine.Utils.Math;
using PortalMouse.Frontend;
using PortalMouse.Love.Utils;
using PortalMouse.Love.Utils.Ext;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PortalMouse.Love;

public sealed class Graph {
	public readonly HashSet<Portal> m_drawnPortals = new();
	private readonly List<Node> m_nodes = new();
	public Node? Selected;

	public readonly Cam Cam = new() {
		Scale = 0.1f,
		Cfg = new() {
			ZoomSpeed = 1,
		},
	};

	public IReadOnlyCollection<Node> Nodes => m_nodes;

	public Vector2 MouseWorldPos => Cam.ViewToWorld(Mouse.GetPosition() - (Vector2)(SizeF)Graphics.GetDimensions() * 0.5f);
	public Vector2 MouseWorldPrevPos => Cam.ViewToWorld(Mouse.GetPreviousPosition() - (Vector2)(SizeF)Graphics.GetDimensions() * 0.5f);

	public void Deinit() {
		foreach (Node node in m_nodes) {
			node.Deinit();
		}
	}

	public void Update(float dt) {
		Selection.Reset();

		HandlePhysics(dt);

		for (int i = m_nodes.Count; i --> 0;) {
			m_nodes[i].Update(dt);
		}
	}

	public Vector2 GetPortalPos(EdgeRange edgeRange, float posAlongPortalNorm) {
		float portalPosAlongEdge = MathX.Map(posAlongPortalNorm, R1I.One, edgeRange.LocalRange);
		Vector2 portalPos = edgeRange.Edge.Side.ToAxis() switch {
			Axis.Horizontal => new(0, portalPosAlongEdge),
			Axis.Vertical => new(portalPosAlongEdge, 0),
			_ => throw new UnreachableException(),
		};
		Node node = m_nodes.First(node => node.Screen == edgeRange.Edge.Screen);
		Vector2 edgePos = edgeRange.Edge.LocalPos.ToLove();
		return node.Trs.GlobalRect.Location + edgePos + portalPos;
	}

	private void HandlePhysics(float dt) {
		const float RepulsionStrength = 50000f;   // strength of node-node repulsion
		const float SpringStrength = 20f;        // strength of link springs
		const float SpringRestLength = 2000f;     // desired link distance
		const float Damping = 0.9f;               // motion damping per frame
		const float MaxVelocity = 2000f;          // velocity cap to avoid explosion
		const float RotationalForce = 50000f;        // how strongly nodes align spring direction

		// 1 - Repulsion between all nodes
		foreach (Node node in m_nodes) {
			if (Selected == node) continue;

			RectangleF nodeRect = node.Trs.GlobalRect;
			Vector2 nodeCenter = nodeRect.Center;

			foreach (Node other in m_nodes) {
				if (other == node) continue;

				RectangleF otherRect = other.Trs.GlobalRect;
				Vector2 delta = nodeCenter - otherRect.Center;
				float distSqr = delta.LengthSquared();
				if (distSqr == 0) continue;

				// Repulsive force (inverse-square)
				float strength = RepulsionStrength / distSqr;
				Vector2 force = delta.Normalized() * strength;
				node.Acc += force;
				other.Acc -= force;
			}

			// 2 - Spring forces for connected edges
			foreach (Edge edge in node.Screen.Edges) {
				foreach (Portal portal in edge.Portals) {
					Vector2 a = GetPortalPos(portal.Desc.EdgeRange, 0.5f);
					Vector2 b = GetPortalPos(portal.Exit.Desc.EdgeRange, 0.5f);
					Vector2 delta = b - a;
					float dist = delta.Length();
					if (dist == 0) continue;

					float diff = dist - SpringRestLength;
					// Hooke’s law: F = -k * x
					Vector2 springDir = delta.Normalized();
					Vector2 springForce = springDir * (SpringStrength * diff);
					node.Acc += springForce;
					Node exitNode = m_nodes.First(node => node.Screen == portal.Exit.Desc.EdgeRange.Edge.Screen);
					exitNode.Acc -= springForce;

					// --- Rotational (orientation) force ---
					Vector2 desiredDir = portal.Desc.EdgeRange.Edge.Side.ToDirection().ToVec().ToLove();

					// Angle error between desired direction and actual spring direction
					float cross = springDir.X * desiredDir.Y - springDir.Y * desiredDir.X; // signed
					float misalignment = cross; // sin(angleError)

					// Apply a sideways (tangential) force to simulate rotational correction
					Vector2 lateral = new(-springDir.Y, springDir.X); // perpendicular to springDir
					Vector2 lateralForce = lateral * (-misalignment * RotationalForce);
					node.Acc += lateralForce;
					exitNode.Acc -= lateralForce;
				}
			}
		}

		foreach (Node node in m_nodes) {
			if (Selected == node) continue;

			node.Vel += node.Acc * dt;
			node.Acc = Vector2.Zero;
			node.Vel *= Damping;

			// Clamp velocity
			if (node.Vel.LengthSquared() > MaxVelocity * MaxVelocity) {
				node.Vel = node.Vel.Normalized() * MaxVelocity;
			}

			node.Trs.LocalRect.Location += node.Vel * dt;
		}
	}

	public void Draw() {
		Cam.Begin();

		Graphics.SetColor(Color.Gray);
		Graphics.SetLineWidth(10f);
		GraphicsX.DrawGrid(Cam, new SizeF(640, 360));

		Graphics.SetColor(Color.White);
		Graphics.SetLineWidth(20f);
		GraphicsX.DrawZeroLines(Cam);

		GraphicsX.DrawGizmo(1);

		m_drawnPortals.Clear();
		foreach (Node node in m_nodes) {
			node.Draw0();
		}
		foreach (Node node in m_nodes) {
			node.Draw1();
		}

		//Graphics.Circle(DrawMode.Fill, MouseWorldPos, 0.1f); //for debug
		Cam.End();

		if (!Selection.IsConsumed) {
			Cam.Update();
		}
		if (Keyboard.IsPressed(KeyConstant.Keypad0)) {
			Cam.Pos = Vector2.Zero;
		}
	}

	public void AddScreen(Screen screen) {
		m_nodes.Add(new Node(this, screen));
	}

	public Config Save(string path) {
		HashSet<Portal> savedPortals = new();
		return new() {
			Mappings = m_nodes
			.SelectMany(n => n.Screen.Edges)
			.SelectMany(e => e.Portals)
			.Where(p => savedPortals.Add(p) && savedPortals.Add(p.Exit))
			.Select(p => new Config.Mapping() {
				A = new Config.PortalEdge(p),
				B = new Config.PortalEdge(p.Exit),
			})
			.ToArray(),
		};
	}
}
