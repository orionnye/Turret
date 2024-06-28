using Godot;
using System;

public partial class PropertyControl : AspectRatioContainer
{
	public float value;
	[Export] public RichTextLabel valueDisplay;
	[Export] public Turret target;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		if (target != null) {
			value = target.fireRate;
		}
	}

	public void setValue(float newValue) {
		value = newValue;
		valueDisplay.Text = Math.Round(newValue).ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	private void _on_button_pressed() {
		// Increase Value by 1
		setValue(value + 0.1f);
		// Set target Property on "target" var equal to value
		target.SetfireRate(value);
		// GD.Print("Increase Value");
	}

	private void _on_button_2_pressed() {
		// Decrease Value by 1
		setValue(value - 0.1f);
		// Set target Property on "target" var equal to value
		target.SetfireRate(value);
		// GD.Print("Decrease Value");
	}
}
