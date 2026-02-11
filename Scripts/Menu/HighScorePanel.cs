using Godot;
using Godot.Collections;
using System;

public partial class HighScorePanel : Panel
{
    [Export] private HighScoreList highScoreList;
    [Export] private Label scoresLabel;
    [Export] private ScrollContainer scrollContainer;
    [Export] private CommonGameSettings commonGameSettings;

    [ExportGroup("Game Mode-dependant")]
    [Export] private Array<Control> difficultyControls;
    [Export] private Array<Control> scoreKeepControls;

    private const int listSize = 40;

    public void UpdateVisuals()
    {
        scrollContainer.ScrollVertical = 0;
        scoresLabel.Text = "";

        Godot.Collections.Array<int> scores = highScoreList.CurrentGameTypeHighScores;
        int scoreCount = scores == null ? 0 : scores.Count;

        for (int i = 0; i < listSize; i++)
        {
            scoresLabel.Text += (i + 1) + (i > 8 ? ". " : ".  ");
            scoresLabel.Text += i >= scoreCount ? "-" : scores[i];

            if (i != listSize - 1)
                scoresLabel.Text += "\n";
        }

        // show/hide difficulty/score-keep related ui elements depending on game mode
        bool showDifficulty = commonGameSettings.GameMode != 0;
        bool showScoreKeep = commonGameSettings.GameMode == 0;

        foreach (Control control in difficultyControls)
        {
            control.Visible = showDifficulty;
        }

        foreach (Control control in scoreKeepControls)
        {
            control.Visible = showScoreKeep;
        }
    }
}
