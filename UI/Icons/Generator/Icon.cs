using Godot;
using System;

public partial class Icon : Control
{
	[Export] public PackedScene iStat;
	public StatBlock stats = new StatBlock();

	public Vector2 dimensions = new Vector2(128, 128);
	public Vector2 position = new Vector2(300, 200);

	private float animationTimer = 0;
	private float animationLength = (float)Math.PI*2;

	// Experimental Stats
	// If we can make a pretty picture with these then we'll shift everything over to use it

	// Health and passive motor controls
	// Constitution makes the user SAFE
	[Export] public float Constitution = 5;

	// Bullet Damage and passive damage scaling
	// Strength makes the user POWERFUL
	[Export] public float Strength = 5;

	// Turn Speed and passive handling speed (Everything should be faster)
	// Dexterity makes the user FASTER
	[Export] public float Dexterity = 7;

	// Vison Cone Range, UI Depth and passive Information Gain
	// Wisdom makes the user AWARE
	[Export] public float Wisdom = 5;

	// Generates Glass and passive commerce effects (glass attraction, shared glass with other turrets)
	// Charisma makes the user ATTRACTIVE
	[Export] public float Charisma = 1.6f;

	// Automation Quality and passive decision tier increased (enables more larger scope decisions)
	// Intellignce makes the user ABLE
	[Export] public float Intelligence = 3.6f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// base._Ready();
		// stats = (StatBlock)iStat.Instantiate();
		// stats.fireRate = 3;
		// I can render
		// drawStats();
		// _Draw();
	}

	public void SemiCircle(byte count, Vector2 pos, float radius, float startAngle, float rotRange, Color color) {
		// how to radix for eachline?

		int lineWidth = 10;
		float spacing = 0.2f;
		float rot = (float)((rotRange-spacing*count)/count);

		Vector2 noise = new Vector2(0, 0);
		Vector2 angle = new Vector2(radius, 0);
		// needs spacing

		// Generate parrellel lines, count in base 1 since the text is being fucky: we'll figure it out later
		for (int i = 0; i < count; i++) {
			float iAngle = spacing + rot;
			// Vector2 start = pos + angle.Rotated((float)rot*i) + noise*GD.Randf()/2;
			// Vector2 end = pos + angle.Rotated((float)rot*i + (float)rot - spacing) + noise*GD.Randf();
			Vector2 start = pos + angle.Rotated(startAngle + iAngle*i);
			Vector2 end = pos + angle.Rotated(startAngle + iAngle*(i+1));

			DrawLine(start, end, color, lineWidth);
		}
	}
	public void radialMeter(byte count, Vector2 pos, float innerRadius, float outerRadius, float startAngle, float fillAngle, Color color) {
		int lineWidth = 10;
		float rot = fillAngle/count;
		// center on startAngle

		Vector2 noise = new Vector2(5, 5);
		// Generate parrellel lines, count in base 1 since the text is being fucky: we'll figure it out later
		for (int i = 1; i <= count; i++) {
			float iAngle = startAngle - (rot*count/2) + rot*i;
			Vector2 start = pos + new Vector2(innerRadius, 0).Rotated(iAngle);
			Vector2 end = pos + new Vector2(outerRadius, 0).Rotated(iAngle);
			
			DrawLine(start, end, color, lineWidth);
		}
	}
	public void PipBar(byte count, Vector2 pos, Vector2 pipDim, Vector2 offset, Color color, float noiseRange = 0) {
		// Should enable user to pass in a function for the stack
		for (int i = 0; i < count; i++) {
			Vector2 noise = new Vector2(GD.Randf()*noiseRange, GD.Randf()*noiseRange);

			Vector2 boxDim = pipDim + noise;
			Vector2 boxPos = pos + offset*i + noise;

			// draw rectangle for now
			DrawRect(new Rect2(boxPos.X, boxPos.Y, boxDim.X, boxDim.Y), color);
		}
		// Should be able to stack
	}
	public void Stack(byte count, Vector2 offset, float noiseRange = 0) {
		// Create a Stack of a specific draw function
		// Should look like a dishevelled pile of stuff

		Vector2 pos = new Vector2(200, 200);
		// Should enable user to pass in a function for the stack
		for (int i = 0; i < count; i++) {
			Color color = Colors.Red;
			Vector2 noise = new Vector2(GD.Randf()*noiseRange, GD.Randf()*noiseRange);

			Vector2 boxDim = new Vector2(100, 100) + noise;
			Vector2 boxPos = pos + offset*i + noise;

			// draw rectangle for now
			DrawRect(new Rect2(boxPos.X, boxPos.Y, boxDim.X, boxDim.Y), color);
		}
	}


	public void circuit(int input) {
		float lineWidth = 5;
		int inputLineCount = 8;

		Vector2 pos = new Vector2(200, 200);
		Vector2 boxDim = new Vector2(60, 60);
		Vector2 wireSpacing =  (dimensions - boxDim) / 2;
		Vector2 boxPos = pos + ((dimensions - boxDim) / 2);
		float wireSegmentLength = wireSpacing.X / 3;
		float startSpacing = dimensions.Y / (inputLineCount+1);
		float enterspacing = boxDim.Y / (inputLineCount + 1);

		// draw entering circuits
		for (var i = 1; i <= inputLineCount; i++) {
			Vector2 iStart = new Vector2(0, startSpacing*i)+pos;
			Vector2 iEnd = new Vector2(0, enterspacing*i)+boxPos;
			Vector2 iAngle = iStart + new Vector2(wireSegmentLength, 0);
			Vector2 iAngleEnd = iEnd + new Vector2(-wireSegmentLength, 0);
			// wireStart
			DrawLine(iStart, iAngle, Colors.Black, lineWidth);
			// wireAngle
			DrawLine(iAngle, iAngleEnd, Colors.Black, lineWidth);
			// wireEnter
			DrawLine(iAngleEnd, iEnd, Colors.Black, lineWidth);
		}
		// draw exit circuits
		for (var i = 1; i <= inputLineCount; i++) {
			Vector2 iStart = new Vector2(dimensions.X, startSpacing*i)+pos;
			Vector2 iEnd = new Vector2(boxDim.X, enterspacing*i)+boxPos;
			Vector2 iAngle = iStart + new Vector2(-wireSegmentLength, 0);
			Vector2 iAngleEnd = iEnd + new Vector2(+wireSegmentLength, 0);
			// wireStart
			DrawLine(iStart, iAngle, Colors.Black, lineWidth);
			// wireAngle
			DrawLine(iAngle, iAngleEnd, Colors.Black, lineWidth);
			// wireEnter
			DrawLine(iAngleEnd, iEnd, Colors.Black, lineWidth);
		}
		// draw the mutation box
		DrawRect(new Rect2(boxPos.X, boxPos.Y, boxDim.X, boxDim.Y), Colors.Red);
		DrawRect(new Rect2(pos.X, pos.Y, dimensions.X, dimensions.Y), Colors.Black, false);
	}
	public void displayStat(Vector2 pos, float value, Color color) {
		Vector2 boxDim = new Vector2(20, 50);
		Vector2 offset = new Vector2(25, 0);
		// Generate parrellel lines, count in base 1 since the text is being fucky: we'll figure it out later
		for (int i = 0; i < value; i++) {
			Vector2 theSpot = pos+offset*i;
			DrawRect(new Rect2(theSpot.X, theSpot.Y, boxDim.X, boxDim.Y), color, true);
		}
	}
	public void spiralStat(Vector2 pos, float value, Color color) {
		int lineWidth = 20;
		float spacing = 0.01f;
		double rot = (Math.PI/value)+ 0.1;
		int radius = 20;

		Vector2 noise = new Vector2(5, 5);

		// Generate parrellel lines, count in base 1 since the text is being fucky: we'll figure it out later
		for (int i = 0; i < value; i++) {
			Vector2 angle = new Vector2(radius, 0);
			Vector2 start = pos + angle.Rotated((float)rot*i) + noise*GD.Randf()/2;
			Vector2 end = pos + angle.Rotated((float)rot*i + (float)rot - spacing) + noise*GD.Randf();
			// Vector2 theSpot = pos+offset*i;
			DrawLine(start, end, color, lineWidth);
			// DrawRect(new Rect2(theSpot.X, theSpot.Y, boxDim.X, boxDim.Y), color, true);
		}
	}

	public void drawStats() {
		float lineWidth = 10;
		// render stats as numbers for debugging

		// Before moving position to add a buffer, consider defining buffer?
		Vector2 pos = new Vector2(300, 300);
		Vector2 offset = new Vector2(0, 50);

		// FireRate
		// displayStat(pos, stats.fireRate, Colors.Yellow);
		spiralStat(pos, stats.fireRate, Colors.Yellow);
		// MaxGain
		// spiralStat(pos+offset, stats.maxGain, Colors.Black);
		// displayStat(pos+offset, stats.maxGain, Colors.Black);
		spiralStat(pos+offset, stats.maxGain, Colors.Black);
		// GainRate
		// displayStat(pos+offset*2, stats.gainRate, Colors.Gray);
		spiralStat(pos+offset*2, stats.gainRate, Colors.Gray);
		// DampLock
		// displayStat(pos+offset*3, stats.dampLock, Colors.DarkBlue);
		spiralStat(pos+offset*3, stats.dampLock, Colors.DarkBlue);
		// DampPassive
		// displayStat(pos+offset*4, stats.dampPassive, Colors.Blue);
		spiralStat(pos+offset*4, stats.dampPassive, Colors.Blue);
		// AimMargin
		// displayStat(pos+offset*5, stats.aimMargin, Colors.Chartreuse);
		spiralStat(pos+offset*5, stats.aimMargin, Colors.Chartreuse);

		// spiral offset
		Vector2 spiralPos = new Vector2(10, 10);
		Vector2 spiralOffset = new Vector2(5, 5);

		spiralStat(spiralPos+spiralOffset*GD.Randf(), stats.fireRate, Colors.Yellow);
		spiralStat(spiralPos+spiralOffset*GD.Randf(), stats.maxGain, Colors.Black);
		spiralStat(spiralPos+spiralOffset*GD.Randf(), stats.gainRate, Colors.Gray);
		spiralStat(spiralPos+spiralOffset*GD.Randf(), stats.dampLock, Colors.DarkBlue);
		spiralStat(spiralPos+spiralOffset*GD.Randf(), stats.dampPassive, Colors.Blue);
		spiralStat(spiralPos+spiralOffset*GD.Randf(), stats.aimMargin, Colors.Chartreuse);

		// Vector2 boxDim = new Vector2(500, 500);
		// for (int i = 0; i < stats.fireRate; i++) {
		// 	int j = i*50;
		// 	Vector2 start = new Vector2(j, j)+pos;
		// 	Vector2 end = new Vector2(j*2, j*2)+pos;
		// 	DrawLine(start, end, Colors.Black, lineWidth);
		// 	DrawRect(new Rect2(pos.X+i, pos.Y+i, boxDim.X, boxDim.Y), Colors.Black, false, 50);
		// }
		// var i = GD.Randf()*20;
		// var i = 600;
	}

	public override void _Draw() {
		// circuit(5);
		// drawStats();
		float timerSin = (float)Math.Sin((double)animationTimer);
		float timerCos = (float)Math.Cos((double)animationTimer);
		float timerTan = (float)Math.Tan((double)animationTimer);

		// TurnSpeed
		Color turnMeter = Colors.Red;
		// spiralStat(new Vector2(480, 200), Dexterity, Colors.Red);
		// Top bar
		radialMeter((byte)Dexterity, new Vector2(600, 300), 130, 250, -2.2f, -1.9f, turnMeter);
		radialMeter((byte)Dexterity, new Vector2(600, 290), 130, 250, 2.5f, 1.3f, turnMeter);

		// Light
		// VisionCone
		Color light = Colors.Yellow;
		radialMeter((byte)Wisdom, new Vector2(600, 295), 150, 300, Charisma, (float)Math.PI, light);

		// Gun Barrel
		Color gun = Colors.LightGray;
		PipBar((byte)Strength, new Vector2(480, 280), new Vector2(20, 30), new Vector2(-24, 0), gun);

		// Turret Head
		// SemiCircle((byte)4, new Vector2(600, 200), 90, 2.8f, 3.8f);
		Color head = Colors.WhiteSmoke;
		SemiCircle((byte)Wisdom, new Vector2(600, 300), 100, 2.8f, 3.8f, head);
		SemiCircle((byte)Wisdom, new Vector2(600+timerSin, 305+timerCos), 80, 2.8f, 3.8f, head);
		SemiCircle((byte)Wisdom, new Vector2(600+timerSin*3, 310+timerCos*3), 60, 2.8f, 3.8f, head);
		SemiCircle((byte)Wisdom, new Vector2(600+timerSin*5, 315+timerCos*5), 40, 2.8f, 3.8f, head);

		// Turret Body
		Color body = Colors.DimGray;
		PipBar((byte)Constitution, new Vector2(500, 320), new Vector2(200, 20), new Vector2(0, 25), body, 0.5f);

		// Glass Generation
		


	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// GD.Print("boogus");
		QueueRedraw();
		animationTimer += (float)delta;
		if (animationTimer > animationLength) {
			animationTimer = 0;
		}
	}
	private void _on_con_value_changed(double value) {
		// Replace with function body.
		Constitution = (float)value;
	}
	private void _on_str_value_changed(double value) {
		// Replace with function body.
		Strength = (float)value;
	}
	private void _on_dex_value_changed(double value) {
		// Replace with function body.
		Dexterity = (float)value;
	}
	private void _on_wis_value_changed(double value) {
		// Replace with function body.
		Wisdom = (float)value;
	}
	private void _on_riz_value_changed(double value) {
		// Replace with function body.
		Charisma = (float)value;
	}
	private void _on_int_value_changed(double value) {
		// Replace with function body.
		Intelligence = (float)value;
	}
}









