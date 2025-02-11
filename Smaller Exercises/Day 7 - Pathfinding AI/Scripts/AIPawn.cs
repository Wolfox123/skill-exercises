using Godot;
using System;

public partial class AIPawn : CharacterBody3D
{
	float speed = 2.0f;
	[Export] Marker3D TargetMark;
	[Export] NavigationAgent3D NavAgent;
	[Export] NavigationRegion3D NavMesh;

    public override void _Ready()
    {
		// Due to how AI's function with Character bodies, we need to skip the first frame
		// This is to make sure the Navigation of the AI does not begin till after the Navigation
		// Server has updated. This should only require a single frame hence our treaded function
		SetPhysicsProcess(false);
		physicsSkip();
    }

	private async void physicsSkip()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        SetPhysicsProcess(true);

		// Set the Target Position now that we are updated
        NavAgent.TargetPosition = TargetMark.GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
		// Begin moving in our general direction through Navigation Actor
        var destination = NavAgent.GetNextPathPosition();
		var local_dest = (destination - GlobalPosition).Normalized();
		var velocity = local_dest * speed;

		// Once this Velocity is updated a signal is called to move them "Safely" to the position
		// This works kinda like a lerp between Nav points while playing nice with actors around it
		// There is more documentation on Actor Avoidance in Godot Docs than I could ever right up here
		NavAgent.Velocity = velocity;

		// This can be called at runtime to update the mesh
		// Requirements could be such as a level changing or the Player performing an
		// action that would distrupt the sceneGodot 
		// THIS SHOULD NOT BE USED OFTEN!!! This is a resource heavy operation and will lag your game
		// This is where Safe Velocity comes in as the Actor will try to avoid objects, but it's not fool proof to say the least
		if (Input.IsActionJustPressed("D7_ReBake"))
		{
			NavMesh.BakeNavigationMesh(true);
		}
	}

	public void _on_navigation_agent_3d_velocity_computed(Vector3 safeVelocity)
	{
		Velocity = safeVelocity;
		MoveAndSlide();
	}
}
