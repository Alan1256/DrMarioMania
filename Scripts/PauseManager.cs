using Godot;
using System;
using System.Collections.Generic;

public partial class PauseManager : BaseHistoryScreenManager
{
    [ExportGroup("Local References")]
    [Export] private MusicDetails musicDetails;
    [Export] private AnimationPlayer pauseAni;
    [ExportGroup("External References")]
    [Export] private GameManager gameMan;
    private bool menuVisible = false;

    public override void _Ready()
	{
        base._Ready();
    }

    public void SetPauseMenuVisibility(bool b)
    {
        pauseAni.Play(b ? "Show" : "Hide");
        menuVisible = true;
        
        if (musicDetails != null && b)
            musicDetails.UpdateDetails();
    }

    public override void SetScreen(int nextScreen)
	{
		base.SetScreen(nextScreen);

        // show music details if on screen 0, otherwise hide
        if (musicDetails != null)
            musicDetails.Visible = currentScreen == 0;
    }

    public override void GoBack()
    {
        backFrame = Engine.GetFramesDrawn();
        
        PopHistory();

        if (screenHistory.Count < 1)
        {
            gameMan.SetIsPaused(false);
            return;
        }

        screens[currentScreen].ResetLastHoverNode();
        screens[currentScreen].Visible = false;
        screens[prevScreen].Visible = true;

		currentScreen = prevScreen;

		GrabFocusOfLastButton();

        // show music details if on screen 0, otherwise hide
        if (musicDetails != null)
            musicDetails.Visible = currentScreen == 0;
    }
}
