using Godot;
using Godot.Collections;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Turret : Node3D {
	[Export] public float fireRate = 1f;
	[Export] public Gun gun;

	public Timer timer;

	// Rotation and controls
	public float leftGain = 0;
	public float rightGain = 0;
	[Export] public float maxGain = 2;
	[Export] public float gainRate = 0.01f;
	[Export] public float dampLock = 30;
	[Export] public float dampPassive = 5;

	[Export] public float aimMargin = 0.2f;

	// Access to Components 
	public RigidBody3D head;
	public Camera3D primaryCam;
	[Export] public Camera3D thirdPersonCam;
	[Export] public Camera3D topDownCam;

	// Currency Control
	public int glass = 10;
	
	// Debugging Flavor
	[Export] public Node3D marker1;
	[Export] public Node3D aimGhost;

	// UI Elements
	[Export] public Control UI;
	[Export] public DisplayProperty headRotText;
	[Export] public DisplayProperty fireRateText;
	[Export] public DisplayProperty aimRotText;
	[Export] public DisplayProperty glassText;
	[Export] public DisplayProperty rightGainText;
	[Export] public DisplayProperty leftGainText;


	public bool IsActive() {
		// cycle through cameras and check if "current" var is toggled
		// Might need a more complicated check, uncertain if this is cause, likely though
		return thirdPersonCam.Current || topDownCam.Current;
	}

	public void SetfireRate(float value) {
		float min = 0.1f;
		float max = 5f;
		if (value < max && value > min) {
			fireRate = value;
			timer.WaitTime = value;
			fireRateText.SetText(fireRate.ToString());
		}
	}
	public void SetGlass(int value) {
		glass = value;
		glassText.SetText(glass.ToString());
	}
	public void SetCamera( Camera3D cam) {
		primaryCam = cam;
		cam.Current = true;
	}


	// -------------------- Called when the node enters the scene tree for the first time --------------------------
	public override void _Ready() {
		head = (RigidBody3D)GetNode<RigidBody3D>("Head");
		timer = (Timer)GetNode<Timer>("Timer");
		SetfireRate(fireRate);
		SetGlass(10);
		// SetCamera(topDownCam);
		SetCamera(thirdPersonCam);
	}


	public Vector3 GetMouseRotation() {
		// Get player rotation based on mouse
		Vector2 mouseRelative = GetMouseRelative();
		Vector3 mouseInSpace = new Vector3(mouseRelative.X, GlobalPosition.Y, mouseRelative.Y);

		return mouseInSpace;
	}
	public Vector2 GetMouseRelative() {
		// Gets the mouse position on screen
		Vector2 mouseInViewport = GetViewport().GetMousePosition();
		// Gets the player Position in ViewPort
		Vector2 userInViewport = GetViewport().GetCamera3D().UnprojectPosition(GlobalPosition);
		// Compares mouse position in viewport compared to playerPos in viewport
		Vector2 mouseRelative = userInViewport - mouseInViewport;
		// return the comparison
		return mouseRelative;
	}
	public Vector3 MouseRayCast() {
		var spaceState = GetWorld3D().DirectSpaceState;
		var mousePos = GetViewport().GetMousePosition();
		var origin = GetViewport().GetCamera3D().ProjectRayOrigin(mousePos);
		var end = origin+GetViewport().GetCamera3D().ProjectRayNormal(mousePos)*1000f;
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		var result = spaceState.IntersectRay(query);
		
		if (result.Count > 0) { return (Vector3)result["position"]; }
		return new Vector3();
	}

	public Unit GetLastUnit() {
		Godot.Collections.Array<Godot.Node> units = GetTree().GetNodesInGroup("Units");
		Unit lastUnit = null;
		if (units.Count > 0) {
			lastUnit = (Unit)units.Last();
		}
		return lastUnit;
	}
	public Unit GetClosestUnit() {
		Godot.Collections.Array<Godot.Node> units = GetTree().GetNodesInGroup("Units");
		if (units.Count > 0) {
			Unit closestUnit = (Unit)units.First();
			float distance = (closestUnit.Position - Position).Length();
			foreach (Unit unit in units) {
				float currentDist = (unit.Position - Position).Length();
				if (currentDist < distance) {
					closestUnit = unit;
					distance = currentDist;
				}
			}
			return closestUnit;
		}
		return null;
	}
	public void trackUnit(Unit unit) {
		if (unit != null) {
			// Track unit by finding out his angle reltaive to your current one
			// WTF why isn't this working
			aimGhost.LookAt(unit.GlobalPosition);
			trackAngle(aimGhost.GlobalRotation.Y);
		}
	}
	public void trackMouse() {
		Vector3 mousePos = MouseRayCast();
		marker1.GlobalPosition = mousePos;
		aimGhost.LookAt(mousePos);
		trackAngle(aimGhost.GlobalRotation.Y);
	}
	public void trackAngle(float targetAngle) {
		// Define Rotation for decision
		float headRot = head.GlobalRotation.Y;

		// INNER BOUNDS Calculations
		// Base Assumption for Inner Bounds is that both are opposite sides of 0
		// Check for both sides being the same and recalculate innerBounds
		float diff = (headRot+(float)Math.PI) - (targetAngle) - (float)Math.PI;
		bool turnLeft = true;

		if (Math.Abs(diff) <= Math.PI) {
			if (targetAngle < headRot) {
				turnLeft = false;
			}
		} else {
			if (targetAngle > headRot) {
				turnLeft = false;
			}
		}
		
		headRotText.SetText(headRot.ToString());
		aimRotText.SetText(targetAngle.ToString());

		// Rotate Turret Right
		if (!turnLeft) {
			// GD.Print("Target Right");
			rightGain += gainRate;
			leftGain = 0;
		}
		// Rotate Turret Left
		if (turnLeft) {
			// GD.Print("Target Left");
			leftGain += gainRate;
			rightGain = 0;
		}
	}

	// -----------Player Controls----------------
	public void UserInput() {
		head.AngularDamp = dampPassive;
		if (Input.IsActionPressed("Left")) {
			// Rotate Turret Left
			if (leftGain <= maxGain) {
				leftGain += gainRate;
			}
		} else if (Input.IsActionJustReleased("Left")) {
			leftGain = 0;
		}
		if (Input.IsActionPressed("Right")) {
			// Rotate Turret Right
			if (rightGain <= maxGain) {
				rightGain += gainRate;
			}
		} else if (Input.IsActionJustReleased("Right")) {
			rightGain = 0;
		}

		Vector3 torque = new Vector3(0, leftGain - rightGain, 0);
		if (Input.IsActionPressed("Left") && Input.IsActionPressed("Right")) {
			head.AngularVelocity = new Vector3(0, 0, 0);
			torque.Y = 0;
		}
		if (Input.IsActionJustReleased("Right") && Input.IsActionJustReleased("Left")) {
			leftGain = maxGain;
			rightGain = maxGain;
		}
	}
	public void unitToggle() {
		// Break into two functions, findNextUnit and focusOnUnit
		// change active turret here
		// Find a list of turrets in scene
		Godot.Collections.Array<Godot.Node> turrets = GetTree().GetNodesInGroup("Turrets");
		// Do I need an index to get the "next" item in the array?
		int selfIndex = turrets.IndexOf(this);

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
		// topDownCam.Current = false;
		UI.Visible = false;
		nextTurret.UI.Visible = true;
		nextTurret.topDownCam.MakeCurrent();
		// GD.Print("selfIndex: ", selfIndex);
	}
	// Big Spin move check for moving at max speed and spawn Chainsaw
	public Vector3 GetSpin() {
		return new Vector3(0, 0, 0);
	}


	// -------------------------------------- Called every frame. 'delta' is the elapsed time since the previous frame -------------------------------------
	public override void _Process(double delta) {
		Unit target = GetClosestUnit();
		if (target != null) {
			trackUnit(target);
		}
		// if (Input.IsActionJustPressed("Cycle")) {
		// 	// Toggle Active Camera maybe?
		// 	GD.Print(topDownCam.Current, thirdPersonCam.Current);
		// }
		// trackMouse();

		// UserInput();

		// Actual Turret Default Controls
		Vector3 torque = new Vector3(0, leftGain - rightGain, 0);
		rightGainText.SetText(rightGain.ToString());
		leftGainText.SetText(leftGain.ToString());
		head.AngularDamp = dampPassive;
		if (torque.Length() > 0) {
			head.ApplyTorque(torque);
		}
	}


	private void _on_area_3d_body_entered(Node3D body) {
		// Replace with function body.
		if (body.GetType() == typeof(Bullet)) {
			body.QueueFree();
			// Implement taking Damage and Health
			// Die();
		}
		if (body.GetType() == typeof(Glass)) {
			SetGlass(glass+1);
			body.QueueFree();
		}
	}
	private void _on_timer_timeout() {
		// Replace with function body.
		if (gun != null) {
			gun.Activate();
		}
		// GD.Print("Fire Bullet!!!");
	}
	private void _on_turret_switch_pressed()
	{
		GD.Print("active?: ", IsActive());
		GD.Print("activeCams: ", topDownCam.Current, thirdPersonCam.Current);
		if (IsActive()) {
			unitToggle();
		}
	}
}
