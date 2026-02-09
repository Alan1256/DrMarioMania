using Godot;
using System;

public partial class KeybindsActionEventButton : Button
{
    [Export] private KeybindsActionPanel actionPanel;

    private InputEvent actionEvent;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Pressed += () => actionPanel.ListenToReplaceEvent(actionEvent);
    }

    public void SetActionIDAndEvent(string actID, InputEvent evnt, bool isController)
    {
        actionEvent = evnt;

        // set button text to event
        string eventText;

        // if controller...
        if (isController)
        {
            eventText = evnt.AsText().Replace("Joypad", "");
        }
        // if keyboard...
        else
        {
            eventText = (evnt as InputEventKey).AsTextPhysicalKeycode();
        }

        Text = eventText;
    }
}