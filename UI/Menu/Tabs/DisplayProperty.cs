using Godot;
using System;

public partial class DisplayProperty : PanelContainer
{
	public RichTextLabel rtl;
	public Label label;
	[Export] public string name = "PlAcEhOlDeR";
	[Export] public string text = "PlAcEhOlDeR";

	// Property Setters
	public void SetText(string text) {
		rtl.Text = text;
	}
	public void SetLabel(string text) {
		label.Text = text;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		label = (Label)GetNode("HoriContainer/Label");
		rtl = (RichTextLabel)GetNode("HoriContainer/Text");
		label.Text = name;
		rtl.Text = text;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
