using Godot;
using System;

public partial class KeybindListenerGroup : Control
{
    [Export] private BaseScreenManager screenMan;

    // last focused node before opening the popup
    private Control lastFocusNode;
    private KeybindsActionPanel currentActionPanel;

    private bool isListening = false;
	
    private const float waitDuration = 3;
    private float waitTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Visible = false;
        SetProcess(false);
    }

    public void StartListening(KeybindsActionPanel panel)
    {
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

        if (@event is InputEventKey || @event is InputEventJoypadButton || @event is InputEventJoypadMotion)
		{
            currentActionPanel.AddEventToAction(@event);
            CallDeferred("StopListening");
        }
    }
}