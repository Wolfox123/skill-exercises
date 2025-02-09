using Godot;
using System;

public partial class DropObject : TextureRect
{
    [Export] public DragAndDrop.slotType slotType;
    [Export] public string itemName = "Empty";
}
