using Godot;
using System;

public partial class MusicDetails : Control
{
    [Export] private Label titleLabel;
    [Export] private Label gameLabel;
    [Export] private MusicList musicList;
    [Export] private CommonGameSettings commonGameSettings;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

	}

	public void UpdateDetails()
	{
		GD.Print("a");
        titleLabel.Text = musicList.GetCurrentMusicTitle(commonGameSettings.CurrentMusic, commonGameSettings.CurrentCustomMusicFile);
		GD.Print("b");
        gameLabel.Text = musicList.GetCurrentMusicGame(commonGameSettings.CurrentMusic);
		GD.Print("c");
    }
}
