using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //Manages a singular level aspect

    public static LevelManager control;

    public Slider brightnessSlider;
    public Image brightnessImage;
    
    [HideInInspector]
    public int activeScreen; // starts at 1 to count properly to screenCount
    [SerializeField]
    private GUI gameUI;

    #region Gameplay
    public void ManageGameplay()
    {
        // catches traveling past the finish line
        if (activeScreen >= gameUI.screens.Length) 
        {
            activeScreen = gameUI.screens.Length-1; // #MN: -1 removes 
        }

        // changes gameScreen to active screen when the activeScreen changes
        if (!gameUI.screens[activeScreen].enabled)
        {
            EnableGameButtons();
            DisableGameScreens();

            gameUI.screens[activeScreen].enabled = true;

            //check win condition
            if (activeScreen == gameUI.screens.Length - 1)
            {
                ChangeGameStateToWin();
            }

            //lose condition
            for (int i = 0; i < gameUI.losingScreens.Length; i++)
            {
                if (activeScreen == gameUI.losingScreens[i])
                {
                    ChangeGameStateToLose();
                }
            }
        }
    }

    //private
    private void DisableGameScreens()
    {
        for (int i = 0; i < gameUI.screens.Length; i++)
        {
            gameUI.screens[i].enabled = false;
        }
    }

    private void DisableGameButtons()
    {
        foreach (Button button in gameUI.buttons)
        {
            button.interactable = false;
        }
    }

    private void EnableGameButtons()
    {
        foreach (Button button in gameUI.buttons)
        {
            button.interactable = true;
        }
    }

    //public 
    public void SetupGameplayUI()
    {
        Debug.LogWarning("SetupGUI");
        DisableGameScreens();
        EnableGameButtons();
        //active screen will auto trigger enabling the first scene
        activeScreen = 0;
    }

    //buttons
    public void AddOneToScreenCount()
    {
        activeScreen += 1;
    }

    public void AddTwoToScreenCount()
    {
        activeScreen += 2;
    }
    #endregion

    #region Options
    public void BrightnessSlider()
    {
        // sets the alpha of an image to the value of a slider
        Color newAlpha = brightnessImage.color;
        newAlpha.a = brightnessSlider.value/100;
        brightnessImage.color = newAlpha;

        Debug.LogError($"newAlpha: {newAlpha.a}, Image colour: {brightnessImage.color}");
    }
    #endregion

    #region UIButtons
    public void ChangeGameStateToTitleMenu()
    {
        GameManager.manager.ChangeState(GameState.TITLEMENU);
    }
    
    public void ChangeGameStateToGamePlay()
    {
        SetupGameplayUI();
        GameManager.manager.ChangeState(GameState.GAMEPLAY);
    }
    
    public void ChangeGameStateToWin()
    {
        DisableGameButtons();
        GameManager.manager.ChangeState(GameState.WIN);
    }
    
    public void ChangeGameStateToLose()
    {
        DisableGameButtons();
        GameManager.manager.ChangeState(GameState.LOSE);
    }

    public void ChangeGameStateToPause()
    {
        DisableGameButtons();
        GameManager.manager.ChangeState(GameState.PAUSE);
    }

    public void ChangeGameStateToOptions()
    {
        GameManager.manager.ChangeState(GameState.OPTIONS);
    }

    public void ChangeGameStateToCredits()
    {
        GameManager.manager.ChangeState(GameState.CREDITS);
    }

    public void ChangeGameStateToPrevious()
    {
        GameManager.manager.ReturnToPreviousState();
        EnableGameButtons();
    }

    public void LoadButtonFade(bool fileExists)
    {
        if (!fileExists)
        {
            foreach (Button button in gameUI.AllLoadButtons)
                button.interactable = false;
        }
        else
        {
            foreach (Button button in gameUI.AllLoadButtons)
                button.interactable = true;
        }
    }

    public void Save()
    {
        GameManager.manager.Save();
    }

    public void Load()
    {
        GameManager.manager.Load();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}

[System.Serializable]
class GUI
{
    public Canvas[] screens;
    public Button[] buttons;
    public Button[] AllLoadButtons;
    [Tooltip("the screen numbers that cause a game over, the final screen is the win screen")]
    public int[] losingScreens;
}