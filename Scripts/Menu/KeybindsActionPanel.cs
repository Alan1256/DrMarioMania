using Godot;
using System;

public partial class KeybindsActionPanel : Control
{
    [Export] private Label actionNameLabel;
    public string ActionName { get { return actionNameLabel.Text; } set { actionNameLabel.Text = value; } }
    private string actionID;
    public string ActionID { get { return actionID; } set { actionID = value; } }
    private string KeyboardActionID { get { return keybindsCon.ActionList.KeyboardPrefix + actionID; } }
    private string ControllerActionID { get { return keybindsCon.ActionList.ControllerPrefix + actionID; } }
	private KeybindsContainer keybindsCon;
    public KeybindsContainer KeybindsCon { get { return keybindsCon; } set { keybindsCon = value; } }
    [Export] private Container keyboardActionsContainer;
    [Export] private Container controllerActionsContainer;
    [Export] private Button addButton;

	private int initialHeight = 16;
	private int heightSteps = 18;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

	}

	public void ListenForNewEvent()
	{
        keybindsCon.ListenerGroup.StartListeningForNew(this);
    }

	public void ListenToReplaceEvent(InputEvent evnt)
	{
        keybindsCon.ListenerGroup.StartListeningToReplace(this, evnt);
    }

	public void ClearActionEvents(bool isControllerInput)
	{
		InputMap.ActionEraseEvents(isControllerInput ? ControllerActionID : KeyboardActionID);
        UpdateVisuals();
    }

	public void AddEventToAction(InputEvent evnt)
	{
		// determine whether event is controller input or not
        bool isControllerInput = evnt is InputEventJoypadButton || evnt is InputEventJoypadMotion;

		// get action id based on isControllerInput
        string actID = isControllerInput ? ControllerActionID : KeyboardActionID;
		
		// add event if it doesn't exist already
		if (!InputMap.ActionHasEvent(actID, evnt))
        	InputMap.ActionAddEvent(actID, evnt);

        UpdateVisuals();
    }

	public void ReplaceEventToAction(InputEvent newEvnt, InputEvent oldEvnt, bool isControllerInput)
	{
		// get action id based on isControllerInput
        string actID = isControllerInput ? ControllerActionID : KeyboardActionID;

		// remove old event
        InputMap.ActionEraseEvent(actID, oldEvnt);

        // add event if it doesn't exist already
		if (!InputMap.ActionHasEvent(actID, newEvnt))
        	InputMap.ActionAddEvent(actID, newEvnt);

        UpdateVisuals();
    }

	public void RevertEventsToDefault(CommonGameSettings settings)
	{
		// erase all of this panel's action's events, then restore defaults from CommonGameSettings
		var RestoreEventsForAction = (string actID) =>
        {
			InputMap.ActionEraseEvents(actID);

            // add default events to the action
			foreach (InputEvent evnt in settings.GetDefaultKeybindsForAction(actID))
			{
				InputMap.ActionAddEvent(actID, evnt);
			}
        };

		// do it for keyboard actions first then controller
        RestoreEventsForAction(KeyboardActionID);
        RestoreEventsForAction(ControllerActionID);
    }

	private int CreateEventButtons(string actID, Container container)
	{
		if (InputMap.HasAction(actID))
		{
			var actionEvents = InputMap.ActionGetEvents(actID);
			int buttonCount = container.GetChildCount();

			for (int i = 0; i < Mathf.Max(actionEvents.Count, buttonCount); i++)
			{
				// if beyond no. of actions...
				if (i >= actionEvents.Count)
				{
					// queue button for deletion (or just hide if i is 0)
					if (i == 0)
						container.GetChild<Button>(i).Visible = false;
					else
						container.GetChild<Button>(i).QueueFree();

				}
				// if within no. of actions...
				else
				{
					KeybindsActionEventButton button;

					// if beyond no. of buttons...
					if (i >= buttonCount)
					{
						// create duplicate button
						button = container.GetChild(0).Duplicate() as KeybindsActionEventButton;
						container.AddChild(button);
					}
					else
					{
						// use existing button and show it
						button = container.GetChild<KeybindsActionEventButton>(i);
						button.Visible = true;
					}

                    // set button text to event
                    button.SetActionIDAndEvent(actID, actionEvents[i], container == controllerActionsContainer);
                }
			}

            return actionEvents.Count;
        }
		else
			GD.PrintErr("action id doesn't exist: " + KeyboardActionID);

		return 0;
	}

	public void UpdateVisuals()
	{		
        int keyBtnCount = CreateEventButtons(KeyboardActionID, keyboardActionsContainer);
        int conBtnCount = CreateEventButtons(ControllerActionID, controllerActionsContainer);

		// update height of panel to fit all action event buttons
        int maxActions = Mathf.Max(keyBtnCount, conBtnCount);
        UpdatePanelHeight(maxActions);
    }

	private void UpdatePanelHeight(int maxActions)
	{
        CustomMinimumSize = new Vector2(CustomMinimumSize.X, initialHeight + heightSteps * maxActions);
    }
}
