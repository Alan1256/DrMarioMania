using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class KeybindsContainer : Control
{
    [Export] private KeybindsActionPanel firstActionPanel;
    [Export] private KeybindListenerGroup listenerGroup;
    public KeybindListenerGroup ListenerGroup { get { return listenerGroup; } }
    [Export] private CommonGameSettings commonGameSettings;
    [Export] private CustomisableActionList actionList;
    public CustomisableActionList ActionList { get { return actionList; } }

    private List<KeybindsActionPanel> actionPanels = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        for (int i = 0; i < actionList.ActionNames.Count; i++)
		{
			KeybindsActionPanel panel;
            
            if (i == 0)
                panel = firstActionPanel;
            else
            {
                panel = firstActionPanel.Duplicate() as KeybindsActionPanel;
                AddChild(panel);
            }

            panel.ActionName = actionList.ActionNames[i];
            panel.ActionID = actionList.ActionIDs[i];
            panel.KeybindsCon = this;
            panel.UpdateVisuals();
            actionPanels.Add(panel);
        }
	}

    public void RevertKeybindsToDefault()
    {
        // for each panel...
        foreach (KeybindsActionPanel panel in actionPanels)
        {
            panel.RevertEventsToDefault(commonGameSettings);
        }

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        // update visuals for each panel
        foreach (KeybindsActionPanel panel in actionPanels)
        {
            panel.UpdateVisuals();
        }
    }
}
