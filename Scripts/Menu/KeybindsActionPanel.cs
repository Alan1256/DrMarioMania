using Godot;
using System;

public partial class KeybindsActionPanel : Control
{
    [Export] private Label actionNameLabel;
    public string ActionName { get { return actionNameLabel.Text; } set { actionNameLabel.Text = value; } }
    private string actionID;
    public string ActionID { get { return actionID; } set { actionID = value; } }
    private string KeyboardActionID { get { return keybindsCon.KeyboardPrefix + actionID; } }
    private string ControllerActionID { get { return keybindsCon.ControllerPrefix + actionID; } }
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

	private void PressAddButton()
	{

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
					Button button;

					// if beyond no. of buttons...
					if (i >= buttonCount)
					{
						// create duplicate button
						button = container.GetChild(0).Duplicate() as Button;
						container.AddChild(button);
					}
					else
					{
						// use existing button and show it
						button = container.GetChild<Button>(i);
						button.Visible = true;
					}

					// set button text to event
					string eventText;

					// if controller...
					if (container == controllerActionsContainer)
					{
						eventText = actionEvents[i].AsText().Replace("Joypad", "");
					}
					// if keyboard...
					else
					{
						eventText = (actionEvents[i] as InputEventKey).AsTextPhysicalKeycode();
					}

					button.Text = eventText;
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
