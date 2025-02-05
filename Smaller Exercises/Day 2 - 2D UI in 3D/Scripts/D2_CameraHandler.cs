using Godot;
using System;

public partial class D2_CameraHandler : Camera2D
{
	Vector2 previousPosition;
	bool dragging = false;
	bool mouseInUIElement = false;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton && @event.IsActionPressed("D2_Drag") && mouseInUIElement)
		{
			// Pass the Input Event and make sure our flags are properly set during the Input Event
			GetViewport().SetInputAsHandled();

			if (@event.IsPressed())
			{
				previousPosition = eventMouseButton.Position;
				dragging = true;
			}
			else
			{
				dragging = false;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Drag the Camera
		if (Input.IsActionPressed("D2_Drag") && dragging)
		{
			GetViewport().SetInputAsHandled();
			Position += previousPosition - GetViewport().GetMousePosition();

			// Range Limit on the Camera
			Position = new Vector2(Mathf.Clamp(Position.X, -2000, 2000),
				Mathf.Clamp(Position.Y, -2000, 2000));
			previousPosition = GetViewport().GetMousePosition();
		}

		// Zoom In MiniMap
		if (Input.IsActionJustPressed("D2_ScrollUp") && mouseInUIElement)
		{
			Zoom = new Vector2(Mathf.Clamp(Zoom.X + 0.1f, 0.5f, 1), Mathf.Clamp(Zoom.Y + 0.1f, 0.5f, 1));
		}

		// Zoom Out MiniMap
		if (Input.IsActionJustPressed("D2_ScrollDown") && mouseInUIElement)
		{
			Zoom = new Vector2(Mathf.Clamp(Zoom.X - 0.1f, 0.5f, 1), Mathf.Clamp(Zoom.Y - 0.1f, 0.5f, 1));
		}

		// Break the Dragging flag just incase
		if (Input.IsActionJustReleased("D2_Drag") && dragging)
		{
			dragging = false;
		}
	}

	public void MouseEnter()
	{
		// Checks if we have our Mouse in the UI element to allow the Dragging of the Mini Map
		mouseInUIElement = true;
	}

	public void MouseExit()
	{
		// Mouse has left the UI element, flag so we know not to allow dragging after
		mouseInUIElement = false;
	}
}
