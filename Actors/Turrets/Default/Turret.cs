using Godot;
using System;
using System.Linq;

public partial class Turret : Node3D {
	[Export] public float fireRate = 1f;
	[Export] public Gun gun;

	// Rotation and controls
	public float leftGain = 0;
	public float rightGain = 0;
	[Export] public float maxGain = 2;
	[Export] public float gainRate = 0.01f;
	[Export] public float dampLock = 30;
	[Export] public float dampPassive = 5;

	// Access to Components 
	public RigidBody3D head;

	// Currency Control
	public int glass = 10;
	[Export] public RichTextLabel glassDisplay;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		head = (RigidBody3D)GetNode<RigidBody3D>("Head");
		Timer timer = (Timer)GetNode<Timer>("Timer");
		timer.WaitTime = fireRate;
	}
	// public Vector3 GetMouseRotation() {
	// 	// Get player rotation based on mouse
	// 	Vector2 mouseRelative = GetMouseRelative();
	// 	Vector3 mouseInSpace = new Vector3(mouseRelative.X, GlobalPosition.Y, mouseRelative.Y);

	// 	return mouseInSpace;
	// }
	// public Vector2 GetMouseRelative() {
	// 	// Gets the mouse position on screen
	// 	Vector2 mouseInViewport = GetViewport().GetMousePosition();
	// 	// Gets the player Position in ViewPort
	// 	Vector2 userInViewport = GetViewport().GetCamera3D().UnprojectPosition(GlobalPosition);
	// 	// Compares mouse position in viewport compared to playerPos in viewport
	// 	Vector2 mouseRelative = userInViewport - mouseInViewport;
	// 	// return the comparison
	// 	return mouseRelative;
	// }
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
	public void trackUnit(Unit unit) {
		if (unit != null) {
			float ideal = Position.SignedAngleTo(unit.Position, new Vector3(0, 1, 0));
			Node3D head = (Node3D)GetNode<Node3D>("Head");
			// Track enemy!!!!!
			// Don't look_at
			// find ideal angle,
			// lerp current angle to ideal angle
			head.RotateY((head.Rotation.Y - ideal)*0.1f);
			// ideal.Position = head.Position;
			// ideal.Rotation = head.Rotation;
			// ideal.LookAt(unit.GlobalPosition);
			// Lerp rotation at a weight
			// towards unit position
			// head.Rotation = head.Rotation.Lerp(ideal.Rotation, 0.1f);
			// Consider directional impulses for tactile mobile controls
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// GetNode<Node3D>("Head").LookAt(GetMouseRotation());
		// Unit target = GetClosestUnit();
		// if (target != null) {
		// 	// GetNode<Node3D>("Head").LookAt(GetLastUnit().GlobalPosition);
		// 	trackUnit(target);
		// }
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
			// head.AngularVelocity = new Vector3(0, 0, 0);
			// head.AngularDamp = dampLock;
			// torque.Y = 0;
		}
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
		// Replace with function body.
		GD.Print("collectingGlass?");
		if (body.GetType() == typeof(Glass)) {
			glass += 1;
			glassDisplay.Text = glass.ToString();
			body.QueueFree();
		}
	}
	private void _on_timer_timeout() {
		// Replace with function body.
		gun.Activate();
		// GD.Print("Fire Bullet!!!");
	}
}
