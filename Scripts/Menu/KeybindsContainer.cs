using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class KeybindsContainer : Control
{
    [Export] private string keyboardPrefix;
    public string KeyboardPrefix { get { return keyboardPrefix; } }
    [Export] private string controllerPrefix;
    public string ControllerPrefix { get { return controllerPrefix; } }
    [Export] private Array<string> actionNames;
    [Export] private Array<string> actionIDs;
    [Export] private KeybindsActionPanel firstActionPanel;
    [Export] private CommonGameSettings commonGameSettings;

    private List<KeybindsActionPanel> actionPanels = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        for (int i = 0; i < actionNames.Count; i++)
		{
			KeybindsActionPanel panel;
            
            if (i == 0)
                panel = firstActionPanel;
            else
            {
                panel = firstActionPanel.Duplicate() as KeybindsActionPanel;
                AddChild(panel);
            }

            panel.ActionName = actionNames[i];
            panel.ActionID = actionIDs[i];
            panel.KeybindsCon = this;
            panel.UpdateVisuals();
            actionPanels.Add(panel);
        }
	}
}
