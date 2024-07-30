using Godot;
using System;

public partial class TurretHandler : Control
{
	// Balance: stored here and resolved to positive before mutations occur

	// HUD Production
	[Export] public PackedScene iStat;

	// Income Attribute
	[Export] public DisplayProperty glass;

	// Offensive Attribute
	[Export] public DisplayProperty fireRate;
	
	// Limiting Attributes
	[Export] public DisplayProperty maxGain;
	[Export] public DisplayProperty dampLock;
	[Export] public DisplayProperty dampPassive;
	[Export] public DisplayProperty aimMargin;
	

	// Reactive Attribute
	[Export] public DisplayProperty gainRate;

	// Reactive Sub-Values
	[Export] public DisplayProperty rightGain;
	[Export] public DisplayProperty leftGain;
	[Export] public DisplayProperty headRot;
	[Export] public DisplayProperty aimRot;
	
	// SpedingTree
	[Export] public VBoxContainer upgrades;

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
		UpdateShop();
	}

	public void UpdateShop() {
		Turret host = (Turret)GetParent();
		// GD.Print("Upgrade: ", upgrades);
		foreach (incrementStat upgrade in upgrades.GetChildren()) {
			// GD.Print("report on queued status: ", upgrade.queued);
			if (upgrade.queued && host.glass >= upgrade.cost) {
				host.stats = host.stats.Add(upgrade.stats);
				// update stats on recieving addition
				upgrades.RemoveChild(upgrade);
				upgrade.QueueFree();
				host.glass -= upgrade.cost;
				AddPurchase();
			}
		}
	}
	public void AddPurchase() {
		// Adds a purchase to the stat Shop
		Turret host = (Turret)GetParent();

		// instance a child and add it as a child the shop
		incrementStat clone = (incrementStat)iStat.Instantiate();
		upgrades.AddChild(clone);
	}

	public void UpdateText() {
		// Update all text Fields based off the corresponding Turret values
		Turret host = (Turret)GetParent();
		glass.SetText(host.glass.ToString());
		fireRate.SetText(host.stats.fireRate.ToString());
		maxGain.SetText(host.stats.maxGain.ToString());
		gainRate.SetText(host.stats.gainRate.ToString());
		dampLock.SetText(host.stats.dampLock.ToString());
		dampPassive.SetText(host.stats.dampPassive.ToString());
		aimMargin.SetText(host.stats.aimMargin.ToString());
		rightGain.SetText(host.rightGain.ToString());
		leftGain.SetText(host.leftGain.ToString());
	}

	private void _on_turret_switch_pressed() {
		Turret host = (Turret)GetParent();
		unitToggle(host);
	}
	private void _on_increment_turn_speed_pressed() {
		Turret host = (Turret)GetParent();
		int cost = 1;
		float increment = 0.1f;
		if (host.glass >= cost) {
			host.glass -= cost;
			host.stats.gainRate += increment;
		}
	}
}
