using Godot;
using System;

public partial class UITracker : Control
{
	public void trackHost() {
		Node3D parent = (Node3D)GetParent();
		Vector2 parentPosInCam = GetViewport().GetCamera3D().UnprojectPosition(parent.GlobalPosition);
		GlobalPosition = parentPosInCam;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		trackHost();
		Visible = true;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// get parent and assign current position to parents position in camera
		// get position of parent in viewport
		trackHost();
	}
}
