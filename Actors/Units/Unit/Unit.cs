using Godot;
using System;
using System.Linq;
using System.Threading;

public partial class Unit : RigidBody3D {
	// Persistance Stats
	[Export] public int health = 1;
	[Export] public float maxHealth = 1;
	[Export] public int cost = 2;
	
	// Navigation stats
	[Export] public float speed = 0.01f;
	[Export] public float contentment = 0.5f;
	[Export] public float curiosity = 4f;
	public Vector3 targetPos = new Vector3(0, 0, 0);
	public Node3D target;

	// Items
	[Export] public PackedScene item;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// getRandomPos();
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
	// Unit has movement that can be called here, but no physics is occuring, these are wandering Area 3Ds
	// This file needs movement control and Actions on interact with specific objects

	private Vector3 GetMotion() {
		Vector3 motion = new Vector3(0, 0, 0);
		// Motion should take into account rotation, for now we'll leave that to inertia
		if ( target != null ) {
			Vector3 direction = (target.GlobalPosition - GlobalPosition).Normalized();
			motion = direction*speed;
		}
		return motion;
	}

	private void getRandomPos() {
		Vector3 desiredPos = Position;
		// This function returns a random deviation from current position, not ideal, great place holder
		var rng = new RandomNumberGenerator();
		Vector3 mutation = new Vector3(rng.Randf()*curiosity - curiosity/2, 0, rng.Randf()*curiosity - curiosity/2);
		desiredPos += mutation;
		targetPos = desiredPos;
		// GD.Print("Print myself picking shit");
	}
	public Glass getGlass() {
		Glass glass = (Glass)item.Instantiate();
		glass.GlobalPosition = GlobalPosition;
		return glass;
	}
	// Triggered Death
	public void Die() {
		for (int i = 0; i < cost; i++) {
			Glass glass = (Glass)item.Instantiate();
			glass.grabbed = false;
			glass.Freeze = false;
			glass.Position = GlobalPosition;
			GetTree().Root.AddChild(glass);
		}
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// if ((Position - targetPos).Length() > contentment) {
		// 	move();
		// }
	}

	// call physics process overload
	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		base._IntegrateForces(state);
		Vector3 motionInput = GetMotion();
		// Vector3 motionInput = new Vector3(0.1f, 0, 0.1f);
		// // // Mouse Functions
		// // create invisible 3D node that follows mouse position and pass it into here for player controls
		// // this.LookAt(GetMouseRotation());

		// // dampen motion during in-action
		// if (motionInput.Length() == 0) {
		// 	// GD.Print("NO USER INPUT");
		// 	LinearDamp = 0.99F;
		// 	Inertia = Inertia.Lerp(Vector3.Zero, LinearDamp); //This is the impact of friction improperly implented
		// 	// LinearVelocity = Vector3.Zero;
		// }
		LookAt(target.Position);
		ApplyCentralImpulse(motionInput);
		// Correct rotation to make the unit face the turret
	}

	private void _on_timer_timeout() {
		// Replace with function body.
		// getRandomPos();
	}
	private void _on_area_3d_body_entered(Node3D body) {
		// Replace with function body.
		if (body.GetType() == typeof(Bullet)) {
			body.QueueFree();
			Die();
		}
	}
}




