using Godot;
using System;

public partial class Spawner : Node3D
{
	// Unit stats
	[Export] public PackedScene spawn;
	[Export] public Node3D Actors;
	[Export] public int cost = 5;
	[Export] public int health = 20;

	// Items
	[Export] public PackedScene glassScene = (PackedScene)GD.Load("res://Items/Static/Glass/Glass.tscn");



	// Spawner stats
	[Export] public float spawnTimer = 0.5f;
	public Timer timer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		timer = GetNode<Timer>("SpawnTimer");
		timer.WaitTime = spawnTimer;
	}


	// Triggered Death
	public void Die() {
		for (int i = 0; i < cost; i++) {
			Glass glass = (Glass)glassScene.Instantiate();
			glass.grabbed = false;
			glass.Freeze = false;
			glass.Position = GlobalPosition;
			GetTree().Root.AddChild(glass);
		}
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	// Signal Handling
	private void _on_spawn_timer_timeout()
	{
		// Impulse strength, could be a global property
		float implStr = 2;
		float yeetStr = 5;
		// Spawn Unit
		Unit unit = (Unit)spawn.Instantiate();
		unit.host = this;
		unit.Position = GlobalPosition;
		RandomNumberGenerator rng = new RandomNumberGenerator();
		Vector3 randomVector = new Vector3(rng.RandfRange(-implStr, implStr), rng.Randf()*yeetStr, rng.RandfRange(-implStr, implStr));
		unit.ApplyCentralImpulse(randomVector);
		Actors.AddChild(unit);
	}
	private void _on_area_3d_body_entered(Node3D body)
	{
		if (body.GetType() == typeof(Bullet)) {
			body.QueueFree();
			health -= 1;
			if (health <= 0) {
				Die();
			}
		}
	}
}
