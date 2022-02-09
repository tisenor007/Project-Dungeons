using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [HideInInspector]
    public int activeScreen;

    private static float creditsBottomPos = 2800;
    private static float creditsTopPos = -1425;
    private static float creditsBeginPos = -500;
    private float creditsYPos = creditsBeginPos;
    private float creditsScrollRate = 1;
    private Canvas notePlain;
    private Text noteWriting;

    private void Start()
    {
        notePlain = GameManager.manager.uiManager.GameUI.notePlain;
        noteWriting = notePlain.transform.GetChild(0).GetComponentInChildren<Text>();
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

    #region UIControl

    //buttons
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
        GameManager.manager.playerAndCamera.transform.GetChild(0).GetComponent<PlayerStats>().ResetStats();
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

    //messages
    public void DisplayPlainNote(string message)
    {
        notePlain.enabled = true;
        noteWriting.text = message;
        Debug.Log($"displaying Note {message}");
    }

    public void StopReadingNote()
    {
        notePlain.enabled = false;
        noteWriting.text = null;
    }

    //misc commands
    public void LoadButtonFade(bool fileExists)
    {
        if (!fileExists)
        {
            foreach (Button button in GameManager.manager.uiManager.GameUI.allLoadButtons)
                button.interactable = false;
        }
        else
        {
            foreach (Button button in GameManager.manager.uiManager.GameUI.allLoadButtons)
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


    #endregion

    #region Options
    public void BrightnessSlider()
    {
        // sets the alpha of an image to the value of a slider
        Color newAlpha = GameManager.manager.uiManager.OptionUI.brightnessImage.color;
        newAlpha.a = GameManager.manager.uiManager.OptionUI.brightnessSlider.value / 100;
        GameManager.manager.uiManager.OptionUI.brightnessImage.color = newAlpha;

        Debug.LogError($"newAlpha: {newAlpha.a}, Image colour: {GameManager.manager.uiManager.OptionUI.brightnessImage.color}");
    }
    #endregion
    
}

