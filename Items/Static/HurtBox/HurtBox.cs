using Godot;
using System;

public partial class HurtBox : Area3D
{
	public float health = 1;
	public float damage;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
