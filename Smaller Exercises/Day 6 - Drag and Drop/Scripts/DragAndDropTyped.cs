using Godot;
using System;

public partial class DragAndDropTyped : DragAndDrop
{
    [Export] Label slotTypeLabel;

    public override void _Ready()
    {
        base._Ready();

        // Sets the Slot Typing Text
        slotTypeLabel.Text = "Typing: " + slotTyping.ToString();
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        // Checks to see if the Slot typing is the same or Untyped
        // If it is, we can drop this item here
        if(data.Obj is DropObject _data)
        {
            if (_data.slotType == slotTyping || slotTyping == slotType.UNTYPED)
            {
                return true;
            }
        }

        return false;
    }
}
