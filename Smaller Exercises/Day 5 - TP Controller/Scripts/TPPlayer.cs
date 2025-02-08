using Godot;
using System;
using static D4_FPSController;

public partial class TPPlayer : CharacterBody3D
{
	// Camera Information
	[ExportGroup("Visual Information")]
	[Export] Node3D visualNode;

    // Camera Information
    [ExportGroup("Camera Information")]
	[Export] Node3D cameraMount;
    [Export] public float horizontalSensitivity = 0.05f;
    [Export] public float verticalSensitivity = 0.05f;

    // Animation Information
    [ExportGroup("Animation Information")]
    [Export] AnimationPlayer animPlayer;

    public const float Speed = 3.0f;
    public const float RunSpeed = 5.0f;
    public const float JumpVelocity = 4.5f;

	// Programmer Note: Dropping the File names of DX, they are too messy even for practice

    public override void _Ready()
	{
		// Lock the Input as this is First Person
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		// Rotate's the Actor, the Model, and the Camera mount in specific directions to get a Good Third Person Controller
		// Rotate Y will rotate the whole model while the Visual node will counter rotate to offset idle
		// Camera Mount allows up and down rotation
		if (@event is InputEventMouseMotion mouseMotion)
		{
			RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * horizontalSensitivity));
			visualNode.RotateY(Mathf.DegToRad(mouseMotion.Relative.X * horizontalSensitivity));
			cameraMount.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * verticalSensitivity));
		}
	}

	public override void _PhysicsProcess(double delta)
    {
		// Very simple Character Controller in the same vain as our D4 Project
        float calculationSpeed = Speed;
        Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
        }

        // Handle Sprint Speed
        if (Input.IsActionPressed("Sprint"))
        {
            calculationSpeed = RunSpeed;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			// Animation - Switch to Walking
			if (animPlayer.CurrentAnimation != "walking" && calculationSpeed == Speed)
			{
				animPlayer.Play("walking");
			}

            // Animation - Switch to Running
            if (animPlayer.CurrentAnimation != "running" && calculationSpeed == RunSpeed)
            {
                animPlayer.Play("running");
            }

            visualNode.LookAt(Position + direction);

			velocity.X = direction.X * calculationSpeed;
			velocity.Z = direction.Z * calculationSpeed;
		}
		else
        {
            // Animation - Switch to Idle
            if (animPlayer.CurrentAnimation != "idle")
            {
                animPlayer.Play("idle");
            }
            
			velocity.X = Mathf.MoveToward(Velocity.X, 0, calculationSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, calculationSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
