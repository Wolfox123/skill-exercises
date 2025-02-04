using Godot;
using System;
using System.IO;

public partial class D1ItemSpawner : Node3D
{
    private string itemPrefab = "res://Smaller Exercises/Day 1 - Resources/Prefabs/ItemObject.tscn";

    public override void _Ready()
    {
        // Spawn our Cache
        // PARAM: Cache Our Item Data Function
        SpawnItemsData(CacheItemData("res://Smaller Exercises/Day 1 - Resources/Resources/"));
    }

    // Cache Item Data Function
    // PARAM: String - Path : Path to load items
    // RETURN: Array - Resources : The Loaded Item
    private Godot.Collections.Array<Resource> CacheItemData(string path)
    {
        // Load all our files into an Array of Resources
        Godot.Collections.Array<Resource> resources = new Godot.Collections.Array<Resource>();

        // Check the Path and see if there are files to load
        DirAccess dir_access = DirAccess.Open(path);
        if (dir_access == null)
        {
            return new Godot.Collections.Array<Resource>();
        }

        string[] files = dir_access.GetFiles();
        if (files == null)
        {
            return new Godot.Collections.Array<Resource>();
        }

        foreach (string file_name in files)
        {
            // Handle the Load of the selected files
            // This can be replaced with a Safe Load if you are attempting to load files outside the project
            Resource loaded_resource = GD.Load<Resource>(path + file_name.TrimSuffix(".remap"));
            if (loaded_resource == null)
            {
                continue;
            }

            // Check against the Resource Type we are wanting to load if we care
            // In this case we do, so check to make sure the D1ItemDataRes has it's data filled out, if not return an error for our Designer
            if (loaded_resource is D1ItemDataRes itemData)
            {
                if (itemData.itemMesh != null)
                {
                    resources.Add(loaded_resource);
                }
                else
                {
                    GD.PrintErr("Resource " + itemData + " Does not have a Mesh Texture");
                }
            }
        }

        return resources;
    }

    // Spawns all Items Data Res in a Given Resource Array
    // PARAM: Array - cachedItems : All items to Spawn
    private void SpawnItemsData(Godot.Collections.Array<Resource> cachedItems)
    {
        // Loop through each of the Resources and spawn them
        foreach (Resource objData in cachedItems)
        {
            // Make sure they are Item Data
            if (objData is D1ItemDataRes itemData)
            {
                Sprite3D spawnedItem = SpawnObject(itemPrefab, this).GetNode<Sprite3D>(".");
                spawnedItem.Texture = itemData.itemMesh;
                spawnedItem.Offset = itemData.itemOffset;
                spawnedItem.PixelSize = itemData.itemPixelScale;
                spawnedItem.GlobalPosition = itemData.position;
                spawnedItem.GetNode<Label3D>("Label3D").Text = itemData.itemName;
            }
        }
    }

    // Spawn Object is a Generic Spawner function I created
    // PARAM: String - ObjectHash : Node Object to Spawn, normally set in the Resource data or hard coded
    // PARAM: Node - Parent : What to parent the spawned Child To.
    private Node SpawnObject(string objectHash, Node parent)
    {
        PackedScene scene = GD.Load<PackedScene>(objectHash);
        Node node = scene.Instantiate();
        parent.AddChild(node);
        return node;
    }
}
