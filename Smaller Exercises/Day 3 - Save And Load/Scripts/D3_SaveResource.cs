using Godot;
using System;

[GlobalClass, Tool]
public partial class D3_SaveResource : Resource
{
    // Save Path String
    const string SAVE_GAME_BASE_PATH = "user://save";

    // Detects if the players have a stale save and we need to update it
    [Export] public int version = 1;

    // You may save raw data here or place child resources! It's really powerful to use Resources however
    // There is a fatel flaw in standard Godot Save loading and that is a security issue, you may use https://github.com/derkork/godot-safe-resource-loader/ to bypass this issue as 
    // it checks against the internal Resources against the one attempting to be loaded. This isn't fool proof but it's a good bypass. 
    // For now, we are just trying to save a String from a changed UI
    [Export] public string textUI;

    /// ----- FUNCTIONS -----

    // Write Save Game
    // Creates and Writes the Game to the given Path
    public void writeSaveGame()
    {
        ResourceSaver.Save(this, getSavePath());
    }

    // Save Exists returns a Bool on if a Save exists in Memory
    public static bool saveExists()
    {
        return ResourceLoader.Exists(getSavePath());
    }

    // Load an Existing Save and Return the Memory into Cache.
    public static Resource loadSaveGame()
    {
        return ResourceLoader.Load(getSavePath(), "", ResourceLoader.CacheMode.Replace);
    }

    // Deletes the Given save file however it should be noted the save should be offloaded from memory to make sure it is deleted safely
    public static void deleteSaveGame()
    {
        DirAccess.RemoveAbsolute(getSavePath());
    }

    // Returns the Save String Path
    // Prefix is to save this as a Exercise
    static string getSavePath()
    {
        string extension = OS.IsDebugBuild() ? ".tres" : ".res";
        string prefix = "_D3";
        return SAVE_GAME_BASE_PATH + prefix + extension;
    }
}
