using Godot;
using System;

[GlobalClass, Tool]
public partial class D1ItemDataRes : Resource
{
    // Setup for our Item Data
    [Export] public string itemName { get; set; }
    [Export] public Texture2D itemMesh { get; set; }
    [Export] public Vector2 itemOffset { get; set; }
    [Export] public float itemPixelScale { get; set; } = 0.01f;
    [Export] public Vector3 position { get; set; }
}
