using Godot;
using System;

public partial class PushUpWarningGroup : Control
{
	[Export] private Label pushUpCountdownLabel;
	[Export] private Control pushUpRowBox;

    private const int minCountdownToBeVisible = 3;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Visible = false;
	}

	public void SetPushUpCountdownLabel(int countdown)
	{
		if (countdown > minCountdownToBeVisible)
		{
            Visible = false;
            return;
        }

        Visible = true;
        pushUpCountdownLabel.Text = "" + countdown;
	}

	public void SetPushUpRows(int rows)
	{
        pushUpRowBox.Size = new Vector2(pushUpRowBox.Size.X, GameConstants.tileSize * rows);
    }

	public void SetWarningWidth(int jarWidth)
	{
        pushUpRowBox.Size = new Vector2(GameConstants.tileSize * jarWidth, pushUpRowBox.Size.Y);
        pushUpRowBox.Position = new Vector2((GameConstants.tileSize * jarWidth - GameConstants.tileSize) / -2.0f, pushUpRowBox.Position.Y);
    }
}
