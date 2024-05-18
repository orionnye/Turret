using Godot;
using System;

public partial class Domain : Node3D
{
	public float sunAngle = 0.5f;
	[Export] public Node3D Actors;
	[Export] public PackedScene defaultUnit;
	[Export] public float spawnRadius = 2f;
	[Export] public float spawnTimer = 0.1f;
	public Timer timer;
	[Export] public float count = 5;
	[Export] public bool combat = true;
	public RichTextLabel RoundEnd;

	// Stats about how to spawn and store Environmental features.
	// Although environmental stats should be reduced and summarized for external algorithms
	// Domain shouldn't mutate any child properties, just give them access to information as efficient as possible

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		timer = GetNode<Timer>("Timer");
		timer.WaitTime = spawnTimer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	private void _on_timer_timeout() {
		// Replace with function body
		// Adds a Unit to the Map
		Unit unit = (Unit)defaultUnit.Instantiate();
		var rng = new RandomNumberGenerator();
		
		Vector3 randPos = new Vector3(rng.Randf()*spawnRadius - spawnRadius/2,  0, rng.Randf()*spawnRadius - spawnRadius/2);
		Vector3 excludedPos = randPos + randPos.Normalized()*spawnRadius;
		excludedPos.Y = 1;

		// Only spawn entities outside a radius around center
		unit.Position = excludedPos;
		Actors.AddChild(unit);
		// Count the total points expended
		if (count <= 0) {
			combat = false;
			// Trigger end combat function and pause spawn timer
			timer.Paused = true;
		} else {
			count -= 1;
		}
	}
}
