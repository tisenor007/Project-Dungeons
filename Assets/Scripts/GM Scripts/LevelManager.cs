using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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
    private float creditsScrollRate = 100;
    private Canvas notePlain;
    private Text noteWriting;

    private void Start()
    {
        notePlain = GameManager.manager.uiManager.notePlain;
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
        GameManager.manager.ChangeState(GameState.GAMEPLAY);
    }
    public void ChangeGameStateToNewGame()
    {
        GameManager.manager.ResetScene();
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
    public void ChangeGameStateToCharacterSelection()
    {
        GameManager.manager.ChangeState(GameState.CHARACTERSELECTION);
    }

    //feedback
    public void CreatePopUp(string message, Vector3 popUpPos, GameObject prefab, Color color)
    {
        PopUp popUp;
        popUp = Instantiate(prefab, new Vector3(popUpPos.x, popUpPos.y + 5, popUpPos.z), Quaternion.identity).GetComponent<PopUp>();
        popUp.SetUp(message, color);
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
            foreach (Button button in GameManager.manager.uiManager.allLoadButtons)
                button.interactable = false;
        }
        else
        {
            foreach (Button button in GameManager.manager.uiManager.allLoadButtons)
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

    public void CreateInteractable(GameObject objectBecomingInteractable, Vector3 newPosition, bool floating, Color lightColour, Item itemType = null)
    {
        GameObject interactableObject = new GameObject($"Interactable {objectBecomingInteractable.name}");
        LayerMask interactableLayer = LayerMask.NameToLayer("Interactable");

        interactableObject.AddComponent(typeof(BoxCollider));

        Light lightSetup = interactableObject.AddComponent(typeof(Light)) as Light;
        lightSetup.color = lightColour;
        lightSetup.renderingLayerMask = interactableLayer;

        Interactable interactableSetup = interactableObject.AddComponent(typeof(Interactable)) as Interactable;
        interactableSetup.floatingObject = floating;
        if (itemType != null) { interactableSetup.ItemType = itemType; }

        objectBecomingInteractable.transform.localPosition = Vector3.zero;
        objectBecomingInteractable.transform.localEulerAngles = Vector3.zero;
        objectBecomingInteractable.transform.SetParent(interactableObject.transform);

        interactableObject.transform.position = newPosition;
    }
    #endregion

    #region Options
    public void BrightnessSlider()
    {
        // sets the alpha of an image to the value of a slider
        Color newAlpha = GameManager.manager.uiManager.brightnessImage.color;
        newAlpha.a = GameManager.manager.uiManager.brightnessSlider.value / 100;
        GameManager.manager.uiManager.brightnessImage.color = newAlpha;

        Debug.Log($"newAlpha: {newAlpha.a}, Image colour: {GameManager.manager.uiManager.brightnessImage.color}");
    }
    #endregion
}

