using Godot;
using System;
using System.Linq;

public partial class Glass : MeshInstance3D
{
	public float rotSpeed = 0.01f;
	public float rotSpeedHigh = 0.03f;
	public float rotSpeedLow = 0.01f;

	public bool grabbed = true;
	public Node3D target;

	public Vector3 GetMouseRotation() {
		// Get player rotation based on mouse
		Vector2 mouseRelative = GetMouseRelative();
		Vector3 mouseInSpace = new Vector3(mouseRelative.X, GlobalPosition.Y, mouseRelative.Y);

		return mouseInSpace;
	}

	public Vector2 GetMouseRelative() {
		// Gets the mouse position on screen
		Vector2 mouseInViewport = GetViewport().GetMousePosition();
		// Gets the player Position in ViewPort
		Vector2 userInViewport = GetViewport().GetCamera3D().UnprojectPosition(GlobalPosition);
		// Compares mouse position in viewport compared to playerPos in viewport
		Vector2 mouseRelative = userInViewport - mouseInViewport;
		// return the comparison
		return mouseRelative;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		target = getTarget();
	}

	public Turret getTarget() {
		Godot.Collections.Array<Godot.Node> turrets = GetTree().GetNodesInGroup("Turrets");
		Turret closest = (Turret)turrets.First();
		foreach (Turret turret in turrets) {
			float closestDist = (closest.Position - Position).Length();
			float currentDist = (turret.Position - Position).Length();
			if (currentDist < closestDist) {
				closest = turret;
			}
		}
		return closest;
	}

	public void pullSelf() {
		Position = Position.Lerp(target.Position, 0.1f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// RotateY(rotSpeed);
		// RotateX(rotSpeed);
		// RotateZ(rotSpeed);
		// Automatically collect item
		// Get Turret, if collides with Turret Area3D: Collect
		if (!grabbed) {
			pullSelf();
			LookAt(GetMouseRotation());
		}
		// Follow the mouse
	}
	private void _on_area_3d_mouse_entered() {
		// Replace with function body
		// Speed up rotation
		rotSpeed = rotSpeedHigh;
		// GD.Print("Jaboogah");
	}

	private void _on_area_3d_mouse_exited() {
		// Replace with function body
		// Slow down rotation
		rotSpeed = rotSpeedLow;
	}
}


