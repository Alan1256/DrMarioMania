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
            if (evnt is InputEventJoypadMotion)
            {
                InputEventJoypadMotion jm = evnt as InputEventJoypadMotion;
                eventText = jm.Axis.ToString() + (jm.AxisValue > 0.0f ? "+" : "-");
            }
            else
                eventText = (evnt as InputEventJoypadButton).ButtonIndex.ToString();
        }
        // if keyboard...
        else
        {
            eventText = (evnt as InputEventKey).AsTextPhysicalKeycode();
        }

        Text = eventText;
    }
}