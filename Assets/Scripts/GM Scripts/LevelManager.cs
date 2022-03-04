using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [HideInInspector]
    public int activeScreen;

    [SerializeField] private GameObject popUpPrefab;
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
        GameManager.manager.uiManager.playerBleeding.color = new Color(100, 0, 0, 0);
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
        SwitchLevel(0);     
    }

    public void ProgressLevel()
    {
        if (GameManager.manager.currentLevel != GameManager.manager.levels.Length - 1)
        {
            SwitchLevel(GameManager.manager.currentLevel + 1);
        }
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

    public void ChangeGameStateToSaveOption()
    {
        GameManager.manager.ChangeState(GameState.SAVEOPTION);
    }

    public void ChangeGameStateToLoadingScreen()
    {
        GameManager.manager.ChangeState(GameState.LOADINGSCREEN);
    }

    public void SwitchLevel(int desiredLevel)
    {
        GameManager.manager.ChangeState(GameState.GAMEPLAY);
        GameManager.manager.ResetScene();

        foreach (GameObject level in GameManager.manager.levels)
        {
            if (level.GetComponent<DungeonGenerator>().dungeonGenerated) { level.GetComponent<DungeonGenerator>().ClearDungeon(); }
        }

        GameManager.manager.currentLevel = desiredLevel;
        GameManager.manager.levels[GameManager.manager.currentLevel].GetComponent<DungeonGenerator>().dungeonIsGenerating = true;
        GameManager.manager.playerAndCamera.transform.GetChild(0).GetComponent<PlayerStats>().ResetStats();
    }

    public void UpdateDungeon()
    {
        foreach (GameObject level in GameManager.manager.levels)
        {
            if (level.GetComponent<DungeonGenerator>().dungeonGenerated == false && level.GetComponent<DungeonGenerator>().dungeonIsGenerating == false)
            {
                level.SetActive(false);
            }
            else if (level.GetComponent<DungeonGenerator>().dungeonGenerated == true || level.GetComponent<DungeonGenerator>().dungeonIsGenerating == true)
            {
                if (GameManager.manager.gameState == GameState.TITLEMENU) { level.SetActive(false); }
                else if (GameManager.manager.gameState == GameState.CHARACTERSELECTION) { level.SetActive(false); }
                else if (GameManager.manager.gameState == GameState.CREDITS) { level.SetActive(false); }
                else if (GameManager.manager.gameState == GameState.OPTIONS) { level.SetActive(false); }
                else { level.SetActive(true); }
            }
        }
    }

    //feedback
    public void CreatePopUp(string message, Vector3 popUpPos, Color color)
    {
        InfoPopUp popUp;
        popUp = Instantiate(popUpPrefab, new Vector3(popUpPos.x, popUpPos.y + 5, popUpPos.z), Quaternion.identity).GetComponent<InfoPopUp>();
        popUp.SetUp(message, color);
    }

    public void CreateInteractable(GameObject objectBecomingInteractable, Vector3 newPosition, bool floating, Color lightColour, Item itemType = null)
    {
        GameObject interactableObject = new GameObject($"Interactable {objectBecomingInteractable.name}");
        Interactable interactableSetup = interactableObject.AddComponent(typeof(Interactable)) as Interactable;
        LayerMask interactableLayer = LayerMask.NameToLayer("Interactable");
        Light lightSetup = interactableObject.GetComponent<Light>();

        // light
        lightSetup.color = lightColour;
        lightSetup.renderingLayerMask = interactableLayer;

        // basic setup
        interactableSetup.isFloatingObject = floating;
        interactableObject.layer = interactableLayer;
        if (itemType != null) { interactableSetup.ItemType = itemType; }

        // object insertion
        objectBecomingInteractable.transform.SetPositionAndRotation(Vector3.zero, Quaternion.LookRotation(Vector3.forward, Vector3.up));
        objectBecomingInteractable.transform.SetParent(interactableObject.transform);

        // FIX PLAYER SCALE x2 reducing size to stop object doubling in size every unequip
        objectBecomingInteractable.transform.localScale = Vector3.one;

        interactableObject.transform.position = newPosition;
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

    public void NextLevelButtonFade(int currentLevel)
    {
        if (currentLevel >= GameManager.manager.levels.Length-1)
        {
            foreach (Button button in GameManager.manager.uiManager.allNextLevelButtons)
            { button.interactable = false;}
        }
        else
        {
            foreach(Button button in GameManager.manager.uiManager.allNextLevelButtons)
            { button.interactable = true; }
        }
    }

    public void Save()
    {
        ChangeGameStateToGamePlay();
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

    public void FlashPlayerBleedingUI()
    {
        GameManager.manager.uiManager.playerBleeding.color = new Color(100, 0, 0, 100);
        StartCoroutine("WaitAndDisablePlayerBleeding");
    }

    IEnumerator WaitAndDisablePlayerBleeding()
    {
        yield return new WaitForSeconds(.3f);
        GameManager.manager.uiManager.playerBleeding.color = new Color (100,0,0,0);
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

