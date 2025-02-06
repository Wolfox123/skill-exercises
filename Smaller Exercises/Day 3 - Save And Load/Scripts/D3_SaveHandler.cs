using Godot;
using System;

public partial class D3_SaveHandler : Node
{
    int versionNumber = 1;

    D3_SaveResource save;
    D3_SaveResource saveDefault;

    [Export] TextEdit UITextEditor;

    /// ----- GAME NAVIGATION CODE -----

    public override void _Ready()
    {
        // Load the Game
        // We should not be handling this in Ready, but instead a different type of broker
        // This is just for simplicity for the documentation
        attemptLoadFromUI();
    }

    // Delete and get new data
    public void CreateNewSave()
    {
        // Free the Memory Slot
        if (D3_SaveResource.saveExists())
        {
            // Clear all Referenced Data to successfully Delete Save Data
            // However given the simplicity of the project, we don't have anything to unload other than the save and save Default
            save.Unreference();
            save = null;
        }

        // Delete the Save
        D3_SaveResource.deleteSaveGame();

        // Start of New Game, get everything loaded
        createOrLoadSave();

        // Place the Loaded Text into the UI
        UITextEditor.Text = save.textUI;

        // Save the New Game Default Data
        saveGame();

        GD.Print("Successfully Started a New Game");
    }

    // Load Data and set it to the UI
    public void attemptLoadFromUI()
    {
        // Start of Game, get everything loaded
        createOrLoadSave();

        // Place the Loaded Text into the UI
        UITextEditor.Text = save.textUI;

        GD.Print("Successfully Loaded Game Data");
    }

    /// ----- SAVE & LOAD CODE ------
    public void createOrLoadSave()
    {
        // This is when we are loading the game
        bool saveExists = D3_SaveResource.saveExists();

        if (saveExists)
        {
            GD.Print("Save Exists, Attempt Load");
            save = (D3_SaveResource)D3_SaveResource.loadSaveGame();

            // Check Save Version
            if (save.version != versionNumber)
            {
                // We have mismatched Versions, delete the old one or handle it in another way like update what needs to be
                // Unload the Save for deleting
                save.Unreference();
                save = null;

                // Delete the Save
                D3_SaveResource.deleteSaveGame();

                // Flag that no Save Exists
                saveExists = false;
            }
        }

        if (!saveExists)
        {
            // Creates a save based on a default save resource we have internally
            GD.Print("Save does not exist, Load default");
            saveDefault = (D3_SaveResource)ResourceLoader.Load("res://Smaller Exercises/Day 3 - Save And Load/Resources/defaultSave.tres", "", ResourceLoader.CacheMode.Replace);
            save = (D3_SaveResource)saveDefault.Duplicate(true);
        }

        // We may then run any additional Code here to signify we have create or loaded the saved data.
    }

    public void saveGame()
    {
        save.writeSaveGame();
    }

    /// ----- UI CODE -----

    // We have changed the Text in the UI, save it to Memory
    // CALLED BY SIGNAL
    public void TextEdited()
    {
        // Place the String into the Save Resource
        save.textUI = UITextEditor.Text;

        // Save it
        saveGame();
    }
}
