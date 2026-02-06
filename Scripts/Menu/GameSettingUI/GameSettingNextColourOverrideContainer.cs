using Godot;
using System;
using System.Collections.Generic;

public partial class GameSettingNextColourOverrideContainer : FlowContainer
{
    [Export] protected bool isForPowerUps;
    [Export] protected Button firstButton;
    [Export] protected Label colourCountLabel;
    [Export] protected CommonGameSettings commonGameSettings;
    [Export] protected ThemeList themeList;
    
    protected List<Button> buttons = new List<Button>();
	protected List<Sprite2D> buttonSprites = new List<Sprite2D>();
    protected int buttonSpriteOffset = 7;

    protected List<int> ChosenOverrideColours
    {
        get
        {
            if (isForPowerUps)
                return commonGameSettings.CurrentPlayerGameSettings.ChosenPowerUpSpecificColours;
            else
                return commonGameSettings.CurrentPlayerGameSettings.ChosenPillSpecificColours;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		for (int i = 0; i < GameConstants.noOfColours; i++)
		{
			Button button;
            
            if (i == 0)
                button = firstButton;
            else
            {
                button = firstButton.Duplicate() as Button;
                AddChild(button);
            }

			buttons.Add(button);
			buttonSprites.Add(button.GetChild<Sprite2D>(0));

			if (i == 0)
            {
                buttonSprites[0].Hframes = isForPowerUps ? GameConstants.PowerUpTileSetWidth : GameConstants.pillTileSetWidth;
                buttonSprites[0].Vframes = GameConstants.noOfColours;
                
                // as power-ups have an extra row for rainbow tiles, add 1 to vframes for power-ups
                if (isForPowerUps)
                    buttonSprites[0].Vframes++;
            }

            // for power-ups, shift one frame down to the pass rainbow row
			buttonSprites[i].Frame = (i + (isForPowerUps ? 1 : 0)) * buttonSprites[i].Hframes;

            // use pill single frame rather than the first one (left segment)
            if (!isForPowerUps)
                buttonSprites[i].Frame += 4;

            int colour = i + 1;
			button.Pressed += () => SetColourState(button.ButtonPressed, colour);
		}

		SetTextures(commonGameSettings.CurrentTheme);
	}

    // adds/removes specific colour
    protected void SetColourState(bool state, int colour)
	{
		if (state)
		{
			if (!ChosenOverrideColours.Contains(colour))
				ChosenOverrideColours.Add(colour);
		}
		else
		{
			if (ChosenOverrideColours.Contains(colour))
				ChosenOverrideColours.Remove(colour);
		}
		
		buttonSprites[colour - 1].SelfModulate = new Color(1,1,1, state ? 1 : 0.25f);
		buttonSprites[colour - 1].Position = new Vector2(buttonSpriteOffset, state ? buttonSpriteOffset + 1 : buttonSpriteOffset);

		// Sort order of colours
		ChosenOverrideColours.Sort();

		UpdateColourCount();
	}
    
    public void UpdateVisuals()
	{
        RemoveUnusedColours();

        SetTextures(commonGameSettings.CurrentTheme);
		UpdateColourCount();
		RefreshButtonStates();
	}

    // Updates the on/off states and visibility of each button based on whether it exists in ChosenOverrideColours and ChosenColours
	protected void RefreshButtonStates()
	{
        PlayerGameSettings settings = commonGameSettings.CurrentPlayerGameSettings;

		for (int i = 0; i < buttons.Count; i++)
		{
            // (i + 1) because first colour, red, has the id of 1 (0 is reserved for non-coloured tiles in the jar grid)
            int col = i + 1;

            // only make this colour visible if it exists in the level
            buttons[i].Visible = settings.ChosenColours.Contains(col);
            
            // make pressed if colour is present in ChosenOverrideColours
            buttons[i].ButtonPressed = ChosenOverrideColours.Contains(col);

			// unpressed buttons should have partially-transparent sprites
			buttonSprites[i].SelfModulate = new Color(1,1,1, buttons[i].ButtonPressed ? 1 : 0.25f);
			buttonSprites[i].Position = new Vector2(buttonSpriteOffset, buttons[i].ButtonPressed ? buttonSpriteOffset + 1 : buttonSpriteOffset);
		}
	}

    // Updates the size of the ChosenColours array depending on ColourCount's value, either adding additional colours or removing unnessicary ones
	protected void RemoveUnusedColours()
	{
        PlayerGameSettings settings = commonGameSettings.CurrentPlayerGameSettings;
        
        // for each chosen override colour
        for (int i = ChosenOverrideColours.Count - 1; i >= 0; i--)
        {
            int overrideCol = ChosenOverrideColours[i];

            // if level colours doesn't contain overrideCol, remove the colour from ChosenOverrideColours
            if (!settings.ChosenColours.Contains(overrideCol))
            {
                ChosenOverrideColours.RemoveAt(i);
            }
        }
	}

    // updates colour count label
    protected void UpdateColourCount()
	{
        if (colourCountLabel == null)
            return;

        int colourCount = ChosenOverrideColours.Count;

		colourCountLabel.Text = colourCount == 0 ? "ANY" : colourCount.ToString();
	}

    // sets all textures to given theme's power-up or pill textures
    protected void SetTextures(int theme)
    {
        Texture2D newTex = isForPowerUps ? themeList.GetPowerUpTileTexture(theme) : themeList.GetPillTileTexture(theme);

		// skip loop if already using same theme textures
		if (buttonSprites[0].Texture == newTex)
			return;

		for (int i = 0; i < buttonSprites.Count; i++)
		{
			buttonSprites[i].Texture = newTex;
		}
    }
}
