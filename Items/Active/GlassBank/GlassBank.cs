using Godot;
using System;

public partial class GlassBank : RigidBody3D
{
	// Break down the core stats of the glass bank

	// Pull
	[Export] public float pullReach = 10;
	[Export] public float pullForce = 0.1f;
	// fall off range?
	// possible 3rd stat?

	// Store
	[Export] public float funds;
	[Export] public int storeMax = 0;
	// passive gain possible third stat?

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
	}
}
