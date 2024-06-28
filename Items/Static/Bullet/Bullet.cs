using Godot;
using System;

public partial class Bullet : RigidBody3D
{
	// Properly implement Health and damage variability in bullets later
	[Export] public int health = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (Position.Y <= 0) {
			QueueFree();
		}
	}
}
