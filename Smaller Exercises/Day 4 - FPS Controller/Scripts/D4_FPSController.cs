using Godot;
using System;

public partial class D4_FPSController : CharacterBody3D
{
    public enum PlayerStance
    {
        STANDING,
        CROUCHED,
        CLIMBING,
        STUNNED
    }

    // Camera Holder
    [Export]
    public Node3D head;
    // Cameras
    [Export]
    public Camera3D camera;
    // Ray Caster to check if we are legal to uncrouch
    [Export]
    public RayCast3D CrouchChecker;

    // State Handling
    public PlayerStance playerStance = PlayerStance.STANDING;

    // Player Handling
    public const float baseFoV = 90;
    public const float changeFoV = 1.5f;
    public const float ClimbSpeed = 1.14f;
    public const float RunSpeed = 3.8f;
    public const float Speed = 2.11f;
    public const float CrouchSpeed = 1.14f;
    public const float JumpVelocity = 3.85f;
    public const float CrouchJumpAssist = 0.15f;
    public const float Sensitivity = 0.001f;
    public const bool RunWalkSwap = false;
    // Run Walk Swap changes if we want Sprint to be the Default speed or Walking to be. Think CSGO Vs. Apex on shift hold

    // Crouch Handling
    public bool crouchJumped;

    // Head Bob
    const float bobFreq = 0.10f;
    const float bobAmp = 0.01f;
    float tBob;

    public override void _Ready()
    {
        // Lock the Input as this is First Person
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        // Mouse Look
        if (inputEvent is InputEventMouseMotion motionEvent)
        {
            head.RotateY(-motionEvent.Relative.X * Sensitivity);
            camera.RotateX(-motionEvent.Relative.Y * Sensitivity);

            Vector3 cameraRot = camera.Rotation;
            cameraRot.X = Mathf.Clamp(camera.Rotation.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
            camera.Rotation = cameraRot;
        }

        // Crouch and Uncrouch Events
        // We want to check to make sure we aren't stunned nor in an illegal spot to uncrouch
        if (Input.IsActionJustPressed("Crouch") && playerStance != PlayerStance.STUNNED && playerStance != PlayerStance.CLIMBING && !CrouchChecker.IsColliding())
        {
            playerStance = (playerStance == PlayerStance.CROUCHED) ? PlayerStance.STANDING : PlayerStance.CROUCHED;
            MovementStateChange(playerStance);

            if (!IsOnFloor() && playerStance == PlayerStance.CROUCHED && !crouchJumped)
            {
                Vector3 JumpCrouch = Position;
                JumpCrouch.Y += CrouchJumpAssist;
                Position = JumpCrouch;

                crouchJumped = true;
            }
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        // Movement Code
        if (playerStance != PlayerStance.STUNNED && playerStance != PlayerStance.CLIMBING)
        {
            float calculationSpeed = (RunWalkSwap) ? RunSpeed : Speed;
            Vector3 velocity = Velocity;

            // Add the gravity.
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
            }

            // Resets Crouch Jump Height Prevention after first go
            if (IsOnFloor() && crouchJumped)
            {
                crouchJumped = false;
            }

            // Handle Jump.
            if (Input.IsActionJustPressed("Jump") && IsOnFloor())
            {
                velocity.Y = JumpVelocity;
            }

            // Handle Sprint Speed
            if (Input.IsActionPressed("Sprint") && playerStance != PlayerStance.CROUCHED)
            {
                calculationSpeed = (RunWalkSwap) ? Speed : RunSpeed;
            }

            // Handle Crouch Speed
            if (playerStance == PlayerStance.CROUCHED)
            {
                calculationSpeed = CrouchSpeed;
            }

            // Get the input direction and handle the movement/deceleration.
            // As good practice, you should replace UI actions with custom gameplay actions.
            Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
            Vector3 direction = (head.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
            if (IsOnFloor())
            {
                if (direction != Vector3.Zero)
                {
                    velocity.X = direction.X * calculationSpeed;
                    velocity.Z = direction.Z * calculationSpeed;
                }
                else
                {
                    velocity.X = Mathf.MoveToward(Velocity.X, 0, calculationSpeed);
                    velocity.Z = Mathf.MoveToward(Velocity.Z, 0, calculationSpeed);
                }
            }
            else
            {
                velocity.X = Mathf.Lerp(velocity.X, direction.X * calculationSpeed, (float)delta * 5.0f);
                velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * calculationSpeed, (float)delta * 5.0f);
            }

            // Head Bob
            tBob += (float)delta + velocity.Length() * Convert.ToInt32(IsOnFloor());
            Transform3D lookTransform = camera.Transform;
            lookTransform.Origin = headBob(tBob);
            camera.Transform = lookTransform;

            Velocity = velocity;
            MoveAndSlide();
        }
    }

    private Vector3 headBob(float time)
    {
        var pos = Vector3.Zero;
        pos.Y = Mathf.Sin(time * bobFreq) * bobAmp;
        pos.X = Mathf.Cos(time * bobFreq / 2) * bobAmp;
        return pos;
    }

    // Plays the given Animation to Move our Camera
    public void MovementStateChange(PlayerStance stanceChange)
    {
        switch (stanceChange)
        {
            case PlayerStance.STANDING:
                GetNode<AnimationPlayer>("CrouchPlayer").PlayBackwards("Standing To Crouch");
                break;
            case PlayerStance.CROUCHED:
                GetNode<AnimationPlayer>("CrouchPlayer").Play("Standing To Crouch");
                break;
            case PlayerStance.STUNNED:
                break;
        }
    }

    // Animation to Move Camera has finished, change the Collider 
    // Another Method of handling for this would be editing the Collider size
    private void _on_animation_player_animation_finished(string animationName)
    {
        if (animationName == "Standing To Crouch")
        {
            CollisionStateChange();
        }
    }

    private void CollisionStateChange()
    {
        // Another Method of handling for this would be editing the Collider size
        switch (playerStance)
        {
            case PlayerStance.STANDING:
                GetNode<CollisionShape3D>("StandingCollision").Disabled = false;
                GetNode<CollisionShape3D>("CrouchCollision").Disabled = true;
                break;
            case PlayerStance.CROUCHED:
                GetNode<CollisionShape3D>("CrouchCollision").Disabled = false;
                GetNode<CollisionShape3D>("StandingCollision").Disabled = true;
                break;
        }
    }
}
