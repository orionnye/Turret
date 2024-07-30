using Godot;
using Godot.Collections;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Turret : Node3D {
	[Export] public Gun gun;

	public Timer timer;
	public int health = 10;

	public StatBlock stats = new StatBlock().Zero();
	// Stat block
	// [Export] public float fireRate = 1f;
	// [Export] public float maxGain = 2;
	// [Export] public float gainRate = 0.01f;
	// [Export] public float dampLock = 30;
	// [Export] public float dampPassive = 5;
	// [Export] public float aimMargin = 0.2f;

	// Rotation and controls
	public float leftGain = 0;
	public float rightGain = 0;


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
	[Export] public TurretHandler UI;


	public bool IsActive() {
		// cycle through cameras and check if "current" var is toggled
		// Might need a more complicated check, uncertain if this is cause, likely though
		return thirdPersonCam.Current || topDownCam.Current;
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
	public void SetGlass(int value) {
		glass = value;
		UI.glass.SetText(glass.ToString());
	}
	// public void SetCamera( Camera3D cam) {
	// 	primaryCam = cam;
	// 	cam.Current = true;
	// }


	// -------------------- Called when the node enters the scene tree for the first time --------------------------
	public override void _Ready() {
		head = (RigidBody3D)GetNode<RigidBody3D>("Head");
		timer = (Timer)GetNode<Timer>("Timer");
		// SetfireRate(fireRate);
		// SetGlass(10);
		// SetCamera(topDownCam);
		// SetCamera(thirdPersonCam);

		// On Boot, set all values to false except for first index
		UI.Visible = false;
		// On first, make active
		Godot.Collections.Array<Godot.Node> turrets = GetTree().GetNodesInGroup("Turrets");
		int selfIndex = turrets.IndexOf(this);
		if (selfIndex == 0) {
			MakeCurrent();
		}
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
		
		// UI.headRot.SetText(headRot.ToString());
		// UI.aimRot.SetText(targetAngle.ToString());

		// Rotate Turret Right
		if (!turnLeft) {
			// GD.Print("Target Right");
			rightGain += stats.gainRate;
			leftGain = 0;
		}
		// Rotate Turret Left
		if (turnLeft) {
			// GD.Print("Target Left");
			leftGain += stats.gainRate;
			rightGain = 0;
		}
	}

	// -----------Player Controls----------------
	public void UserInput() {
		if (stats.dampPassive >= 0) {
			head.AngularDamp = stats.dampPassive;
		}
		if (Input.IsActionPressed("Left")) {
			// Rotate Turret Left
			if (leftGain <= stats.maxGain) {
				leftGain += stats.gainRate;
			}
		} else if (Input.IsActionJustReleased("Left")) {
			leftGain = 0;
		}
		if (Input.IsActionPressed("Right")) {
			// Rotate Turret Right
			if (rightGain <= stats.maxGain) {
				rightGain += stats.gainRate;
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
			leftGain = stats.maxGain;
			rightGain = stats.maxGain;
		}
	}
	public void MakeCurrent() {
		// topDownCam.Current = false;
		UI.Visible = true;
		topDownCam.MakeCurrent();
		// thirdPersonCam.MakeCurrent();
	}
	public void MakeInactive() {
		
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
		} else if (IsActive()){
			UserInput();
		}

		// Actual Turret Default Controls
		Vector3 torque = new Vector3(0, leftGain - rightGain, 0);
		if (stats.dampPassive >= 0) {
			head.AngularDamp = stats.dampPassive;
		}
		if (torque.Length() > 0) {
			head.ApplyTorque(torque);
		}
		UI.UpdateText();
	}


	private void _on_area_3d_body_entered(Node3D body) {
		// Replace with function body.
		if (body.GetType() == typeof(Bullet)) {
			body.QueueFree();
			// Implement taking Damage and Health
			// Die();
		}
		if (body.GetType() == typeof(Unit)) {
			// body.QueueFree();
			// Implement taking Damage and Health
			// Die();
		}
		if (body.GetType() == typeof(Glass)) {
			// Check for debt, then resolve it by absorbing glass nearby
			if (glass <= 0) {
				// resolve debt
				SetGlass(glass+1);
				body.QueueFree();
			}
		}
	}
	private void _on_timer_timeout() {
		// Replace with function body.
		if (gun != null) {
			gun.Activate();
		}
		// GD.Print("Fire Bullet!!!");
	}
}


