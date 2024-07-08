using Godot;
using System;

public partial class Exchange : Control
{
	[Export] public string costCurrency = "R";
	[Export] public string gainCurrency = "C";
	[Export] public float skew = 0.5f;
	[Export] public float total = 2;
	[Export] public RichTextLabel costText;
	[Export] public RichTextLabel gainText;

	public int getRNums(float value) {
		// Get Rune numbers
		// Account for Radix and Value
		return (int)value;
	}
	public int getCNums(float value) {
		// Get regular numbers with currency sign after
		return (int)value;
	}

	public void setCostText(string text) {
		costText.Text = text;
	}
	public void setGainText(string text) {
		gainText.Text = text;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Correct the cost and Gain Text to the appropriate Languages
		// if ()
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
