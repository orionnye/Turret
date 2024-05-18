using Godot;
using System;

public partial class PropertyControl : AspectRatioContainer
{
	[Export] public float value = 0;
	[Export] public RichTextLabel valueDisplay;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	public void setValue(float newValue) {
		value = newValue;
		valueDisplay.Text = newValue.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	private void _on_button_pressed() {
		// Increase Value by 1
		setValue(value + 1);
		GD.Print("Increase Value");
	}


	private void _on_button_2_pressed() {
		// Decrease Value by 1
		setValue(value - 1);
		GD.Print("Decrease Value");
	}
}
