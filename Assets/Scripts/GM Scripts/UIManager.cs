using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas titleMenu;
    public Canvas gameplay;
    public Canvas winDisplay;
    public Canvas loseDisplay;
    public Canvas pause;
    public Canvas options;
    public Canvas credits;
    public Canvas characterSelection;

    [Space(20)]

    [Tooltip("any button that calls load")]
    public Button[] allLoadButtons;

    public Canvas notePlain;
    public Image playerBleeding;

    [Header("Options")]
    [Tooltip("The Background Image in SystemContants")] 
    public Image brightnessImage;
    public Slider brightnessSlider;

    private void Awake()
    {
        LoadTitleMenu();
    }

    private void Update()
    {
        //Debug.Log(credits.enabled);
    }

    public void DisableAll()
    {
        titleMenu.enabled = false;
        options.enabled = false;
        credits.enabled = false;
        gameplay.enabled = false;
        winDisplay.enabled = false;
        loseDisplay.enabled = false;
        pause.enabled = false;
        characterSelection.enabled = false;
    }

    public void LoadTitleMenu()
    {
        DisableAll();
        titleMenu.enabled = true;
    }

    public void LoadGameplay()
    {
        DisableAll();
        gameplay.enabled = true;
    }

    public void LoadWinScreen()
    {
        winDisplay.enabled = true;
    }

    public void LoadLoseScreen()
    {
        loseDisplay.enabled = true;
    }

    public void LoadPauseScreen()
    {
        credits.enabled = false;
        options.enabled = false;
        pause.enabled = true;
    }

    public void LoadOptions()
    {
        options.enabled = true;
    }

    public void LoadCredits()
    {
        DisableAll();
        credits.enabled = true;
    }
    public void LoadCharacterSelection()
    {
        DisableAll();
        characterSelection.enabled = true;
    }
}

[System.Serializable]
public class GUI
{
    
}

