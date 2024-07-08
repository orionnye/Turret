using Godot;
using System;

public partial class TurretHandler : Control
{
	// Balance: stored here and resolved to positive before mutations occur

	// HUD Displays
	// Income Attribute
	[Export] public DisplayProperty glass;
	// Offensive Attribute
	[Export] public DisplayProperty fireRate;
	// Reactive Attribute
	[Export] public DisplayProperty gainRate;
	// Reactive Sub-Values
	[Export] public DisplayProperty rightGain;
	[Export] public DisplayProperty leftGain;
	[Export] public DisplayProperty headRot;
	[Export] public DisplayProperty aimRot;
	

	public void unitToggle(Turret current) {
		// change active turret here
		// Find a list of turrets in scene
		Godot.Collections.Array<Godot.Node> turrets = GetTree().GetNodesInGroup("Turrets");
		int selfIndex = turrets.IndexOf(current);
		int nextIndex = selfIndex + 1;

		// If total turrets are 1, then there's no need to toggle
		if (turrets.Count < 1) {
			return;
		}
		if (nextIndex >= turrets.Count) {
			nextIndex = 0;
		}

		// access the nextTurret
		Turret nextTurret = (Turret)turrets[nextIndex];
		Visible = false;
		nextTurret.MakeCurrent();
		// GD.Print("selfIndex: ", selfIndex);
	}

	// public void SetfireRate(float value) {
	// 	float min = 0.1f;
	// 	float max = 5f;
	// 	if (value < max && value > min) {
	// 		fireRate = value;
	// 		timer.WaitTime = value;
	// 		UI.fireRate.SetText(fireRate.ToString());
	// 	}
	// }
	// public void SetGlass(int value) {
	// 	Turret host = (Turret)GetParent();
	// 	host.glass = value;
	// 	glass.SetText(glass.ToString());
	// }
	// public void SetCamera( Camera3D cam) {
	// 	primaryCam = cam;
	// 	cam.Current = true;
	// }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public void UpdateText() {
		// Update all text Fields based off the corresponding Turret values
		Turret host = (Turret)GetParent();
		rightGain.SetText(host.rightGain.ToString());
		leftGain.SetText(host.leftGain.ToString());
	}

	private void _on_turret_switch_pressed() {
		Turret host = (Turret)GetParent();
		unitToggle(host);
	}
	private void _on_increment_turn_speed_pressed() {
		Turret host = (Turret)GetParent();

	}
}




