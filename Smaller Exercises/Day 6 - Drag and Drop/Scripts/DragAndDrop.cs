using Godot;
using System;

public partial class DragAndDrop : PanelContainer
{
	public enum slotType
	{
		UNTYPED,
		WEAPON,
		ARMOR,
		TRINKET
	}

	[Export] public Label itemName;
	[Export] public DropObject itemInformation;
	[Export] public slotType slotTyping = slotType.UNTYPED;

	public override void _Ready()
	{
		// Gets information from attached Object
		itemName.Text = itemInformation.itemName;
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		// Set our Drag preview of the item
		SetDragPreview(GetPreview());

		return itemInformation;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		// Sees if we can drop this item in the slot by returning what we are hovering over
		return data.Obj is DropObject;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		Texture2D tempTexture = itemInformation.Texture;
		string tempHash = itemInformation.itemName;
		slotType tempTyping = itemInformation.slotType;

		// Swaps the Slot information
		if (data.Obj is DropObject dataProperty)
		{
			itemInformation.Texture = dataProperty.Texture;
			itemInformation.itemName = dataProperty.itemName;
			itemInformation.slotType = dataProperty.slotType;
			itemName.Text = itemInformation.itemName;

			dataProperty.Texture = tempTexture;
			dataProperty.itemName = tempHash;
			dataProperty.slotType = tempTyping;
			dataProperty.GetParent().GetParent().GetChildOrNull<Label>(1).Text = tempHash;
		}
	}

	public Control GetPreview()
	{
		// Create a Preview Item through a new Texture Rect
		TextureRect previewTexture = new TextureRect();

		// After Creation of new, set the Texture and Size
		previewTexture.Texture = itemInformation.Texture;
		previewTexture.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		previewTexture.Size = new Vector2(120, 120);

		// Add it as a child of our Cursor
		Control preview = new Control();
		preview.AddChild(previewTexture);

		return preview;
	}
}
