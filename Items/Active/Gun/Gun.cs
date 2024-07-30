using Godot;
using System;

public partial class Gun : Node3D
{
	[Export] public int bulletCount = 1;
	[Export] public float bulletSpeed = 3;
	// Global transform.basis
	[Export] public Vector3 barrelExit;
	[Export] public PackedScene bulletScene;
	[Export] public AnimationPlayer anim;

	public virtual void Activate() {
		// Fires a bullet from turret
		// spawn a bullet and add a velocity to the bullet facing gun direction
		for (int i = 0; i < bulletCount; i++) {
			Bullet bullet = (Bullet)bulletScene.Instantiate();
			GetTree().Root.AddChild(bullet);
			bullet.Position = ToGlobal(barrelExit);
			bullet.ApplyCentralImpulse(bulletSpeed*GlobalTransform.Basis.Z.Normalized());
		}
		anim.Play("Fire");
		// GD.Print("Firing Bullet");
	}

	public void Deactivate() {

	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Activate();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
