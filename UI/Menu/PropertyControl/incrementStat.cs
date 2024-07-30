using Godot;
using System;

public partial class incrementStat : HBoxContainer
{
	// Interactive stats
	public Boolean queued = false;

	// Upgrade Stats
	[Export] public int cost = 1;
	[Export] public float increment = 0.1f;
	[Export] public int min = -2;
	[Export] public int max = 0;
	// [Export] public String statTitle = "N/A:";
	public StatBlock stats = new StatBlock();

	// Called when the node enters the scene tree for the first time.
	public String Translate(StatBlock block) {
		String translated = "";
		// Sadly I need to iterate manually over these stats, NOT SCALABLE
		// but manageable for now, very sad
		translated += block.fireRate.ToString() + ", ";
		translated += block.maxGain.ToString() + ", ";
		translated += Math.Sign(block.gainRate).ToString() + block.gainRate.ToString() + ", ";
		translated += block.dampLock.ToString() + ", ";
		translated += block.dampPassive.ToString() + ", ";
		translated += block.aimMargin.ToString() + ", ";
		return translated;
	}
	public override void _Ready() {
		Label label = (Label)GetNode("Label");
		Label costLabel = (Label)GetNode("Cost");
		// Should be randomizing stats here, randomize function is just weeeeak
		stats = stats.Random();
		// label.Text = statTitle;

		// Later the cool language iconography will be displayed here
		label.Text = stats.GetLetters();
		costLabel.Text = cost.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	private void _on_increment_button_pressed() {
		// Replace with function body.
		if (!queued) {
			// GD.Print("Jaboogah");
			queued = true;
		}
	}
}


