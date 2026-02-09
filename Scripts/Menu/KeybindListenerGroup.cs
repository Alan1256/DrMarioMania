using Godot;
using System;

public partial class KeybindListenerGroup : Control
{
    [Export] private BaseScreenManager screenMan;
    [Export] private string genericText;
    [Export] private string keyboardText;
    [Export] private string controllerText;
    [Export] private Label label;

    // last focused node before opening the popup
    private Control lastFocusNode;

    private KeybindsActionPanel currentActionPanel;
    private InputEvent eventToReplace = null;

    private bool IsReplacing { get { return eventToReplace != null; } }
    private bool IsReplacingKeyboard { get { return eventToReplace is InputEventKey; } }

    private bool isListening = false;
    private const float waitDuration = 3;
    private float waitTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Visible = false;
        SetProcess(false);
    }

    public void StartListeningToReplace(KeybindsActionPanel panel, InputEvent evnt)
    {
        eventToReplace = evnt;
        StartListening(panel);
    }

    public void StartListeningForNew(KeybindsActionPanel panel)
    {
        eventToReplace = null;
        StartListening(panel);
    }

    private void StartListening(KeybindsActionPanel panel)
    {
        if (IsReplacing)
            label.Text = IsReplacingKeyboard ? keyboardText : controllerText;
        else
            label.Text = genericText;

        Visible = true;
        isListening = true;

        currentActionPanel = panel;

        lastFocusNode = GetViewport().GuiGetFocusOwner();
        GetViewport().GuiReleaseFocus();

        screenMan.SetCanUseKeyInput(false);

        waitTimer = waitDuration;

		SetProcess(true);
    }

    public async void StopListening()
    {
        Visible = false;
        isListening = false;

        lastFocusNode.GrabFocus();
		SetProcess(false);

		// re-enabling screenman input support is delayed to avoid going back a screen if a back key/button was pressed
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		screenMan.SetCanUseKeyInput(true);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        waitTimer -= (float)delta;

		if (waitTimer <= 0)
            StopListening();
    }

	// Listen for inputs
    public override void _Input(InputEvent @event)
    {
        if (!isListening)
            return;

        
        // input is valid if it is keyboard input + if not replacing or replacing a keyboard input
        bool isValidInput = @event is InputEventKey && (!IsReplacing || IsReplacingKeyboard);

        // input is valid if it is controller input + if not replacing or replacing a controller input
        if (!isValidInput)
            isValidInput = (@event is InputEventJoypadButton || @event is InputEventJoypadMotion) && (!IsReplacing || !IsReplacingKeyboard);

        if (isValidInput)
		{
            if (IsReplacing)
                currentActionPanel.ReplaceEventToAction(@event, eventToReplace, !IsReplacingKeyboard);
            else
                currentActionPanel.AddEventToAction(@event);
            
            CallDeferred("StopListening");
        }
    }
}