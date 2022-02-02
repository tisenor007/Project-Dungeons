using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [HideInInspector]
    public int activeScreen; // starts at 1 to count properly to screenCount
    [SerializeField]
    private OUI optionUI;
    [SerializeField]
    private GUI gameUI;

    private static float creditsBottomPos = 2800;
    private static float creditsTopPos = -1425;
    private static float creditsBeginPos = -500;
    private float creditsYPos = creditsBeginPos;
    private float creditsScrollRate = 1;

    public void Start()
    {
        
    }

    public void Update()
    {
        if (GameManager.manager.uiManager.credits.enabled == false && creditsYPos != creditsBeginPos)
        {
            creditsYPos = creditsBeginPos;
        }
        else if (GameManager.manager.uiManager.credits.enabled == true)
        {
            ScrollCredits();
        }
    }

    #region UIButtons

    public void ButtonStartNewGame()
    {
        ChangeGameStateToGamePlay();
        Debug.LogWarning("SetupGUI");
        //game set up goes here

    }

    public void ChangeGameStateToTitleMenu()
    {
        GameManager.manager.ChangeState(GameState.TITLEMENU);
    }
    
    public void ChangeGameStateToGamePlay()
    {
        GameManager.manager.playerRef.transform.GetChild(0).GetComponent<PlayerStats>().ResetStats();
        GameManager.manager.ChangeState(GameState.GAMEPLAY);
    }
    
    public void ChangeGameStateToWin()
    {
        GameManager.manager.ChangeState(GameState.WIN);
    }
    
    public void ChangeGameStateToLose()
    {
        GameManager.manager.ChangeState(GameState.LOSE);
    }

    public void ChangeGameStateToPause()
    {
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

    #region Options
    public void BrightnessSlider()
    {
        // sets the alpha of an image to the value of a slider
        Color newAlpha = optionUI.brightnessImage.color;
        newAlpha.a = optionUI.brightnessSlider.value / 100;
        optionUI.brightnessImage.color = newAlpha;

        Debug.LogError($"newAlpha: {newAlpha.a}, Image colour: {optionUI.brightnessImage.color}");
    }
    #endregion

    private void ScrollCredits()
    {
        int childOrderNum = 0;
        GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position = new Vector3(GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.x, creditsYPos, GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.z);

        if (GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.y >= creditsBottomPos) { creditsYPos = creditsTopPos; }
        else if (GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.y <= creditsBottomPos && GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.y >= creditsTopPos)
        {
            creditsYPos = creditsYPos += Time.deltaTime * creditsScrollRate;
        }
    }
}

[System.Serializable]
class GUI
{
    [Tooltip("any button that calls load")]
    public Button[] AllLoadButtons;
}

class OUI // Option User Interface
{
    public Slider brightnessSlider;
    public Image brightnessImage;
}