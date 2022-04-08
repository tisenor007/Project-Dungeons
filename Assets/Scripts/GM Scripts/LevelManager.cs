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
    private static float creditsBottomPos = 2825;
    private static float creditsTopPos = -2125;
    private static float creditsBeginPos = -750;
    private float creditsYPos = creditsBeginPos;
    private float creditsScrollRate = 100;
    private Canvas notePlain;
    private Text noteWriting;
    private Color weaponInfoControl = new Color();
    private Color alphaValueColour;
    [SerializeField] private float weaponDisplayTimer = 3;

    private StoryManager story;

    public StoryManager Story { get { return story; } }
    public float WeaponDisplayTimer { set { weaponDisplayTimer = value; } }

    private void Start()
    {
        //setting up Notes/story
        notePlain = GameManager.manager.uiManager.notePlain;
        noteWriting = notePlain.transform.GetChild(0).GetComponentInChildren<Text>();
        story = transform.parent.GetComponentInChildren<StoryManager>();
    }

    public void Update()
    {
        //ui
        weaponInfoControl = GameManager.manager.uiManager.weaponInfo.transform.GetChild(0).GetComponent<Image>().color;

        if (weaponInfoControl.a >= 1) // 1 is the max value of alpha
        {
            weaponDisplayTimer -= 1 * Time.deltaTime;
        }
        
        if (weaponDisplayTimer <= 0)
        { 
            JumpCanvasAlphaTo(0, GameManager.manager.uiManager.weaponInfo);
        }


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
        SoundManager.PlaySound(SoundManager.Sound.CannonShot);
        ChangeGameStateToGamePlay();
        Debug.LogWarning("SetupGUI");
        //game set up goes here

    }

    public void ChangeGameStateToTitleMenu()
    {
        GameManager.manager.gameStarting = false;
        GameManager.manager.uiManager.startButton.interactable = true;
        GameManager.manager.uiManager.playButton.interactable = true;
        GameManager.manager.ChangeState(GameState.TITLEMENU);
    }
    
    public void ChangeGameStateToGamePlay()
    {
        GameManager.manager.ChangeState(GameState.GAMEPLAY);
    }

    public void ChangeGameStateToNewGame()
    {
        StartCoroutine(LoadGameplay());
        
    }

    public void ProgressLevel()
    {
        GameManager.manager.ChangeState(GameState.LOADINGSCREEN);
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
        GameManager.manager.gameStarting = true;
        GameManager.manager.uiManager.startButton.interactable = false;
        GameManager.manager.uiManager.playButton.interactable = true;
        //GameManager.manager.loadButton.interactable = false;
        StartCoroutine(LoadCharacterSelectionScreen());
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
        GameManager.manager.ResetScene();

        foreach (GameObject level in GameManager.manager.levels)
        {
            if (level.GetComponent<DungeonGenerator>().dungeonGenerated) { level.GetComponent<DungeonGenerator>().ClearDungeon(); }
        }

        GameManager.manager.currentLevel = desiredLevel;
        GameManager.manager.levels[GameManager.manager.currentLevel].GetComponent<DungeonGenerator>().dungeonPreGenerating = true;
        GameManager.manager.levels[GameManager.manager.currentLevel].GetComponent<DungeonGenerator>().dungeonIsGenerating = true;
        GameManager.manager.playerStats.ResetStats();
        SoundManager.PlayMusic(SoundManager.Sound.GameplayMusic);
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

    public GameObject CreateInteractable(GameObject objectBecomingInteractable, Vector3 newPosition, bool floating, Color lightColour, Item itemType = null)
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
        
        /*// FIX PLAYER SCALE x2 reducing size to stop object doubling in size every unequip
        objectBecomingInteractable.transform.localScale = Vector3.one;*/

        interactableObject.transform.position = newPosition;

        return interactableObject;
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
        SoundManager.PlaySound(SoundManager.Sound.PaperAway);
    }

    //misc commands
    public void LoadButtonFade(bool fileExists)
    {
        if (!fileExists || GameManager.manager.gameStarting)
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
        int childOrderNum = 2;
        GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position = new Vector3(GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.x, creditsYPos, GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.z);

        if (GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.y >= creditsBottomPos) { creditsYPos = creditsTopPos; }
        else if (GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.y <= creditsBottomPos && GameManager.manager.uiManager.credits.transform.GetChild(childOrderNum).transform.position.y >= creditsTopPos)
        {
            creditsYPos = creditsYPos += Time.deltaTime * creditsScrollRate;
        }
    }

    // depricated
    /*public void FlashPlayerBleedingUI()
    {
        GameManager.manager.uiManager.playerBleeding.color = new Color(0, 0, 0, 100); // setting alpha to visible
        StartCoroutine("WaitAndDisablePlayerBleeding");
    }*/

    public void JumpCanvasAlphaTo(float value, Canvas inputCanvas)
    {
        Image[] canvasImages = inputCanvas.gameObject.GetComponentsInChildren<Image>();
        Text[] canvasText = inputCanvas.gameObject.GetComponentsInChildren<Text>();

        if (canvasImages.Length >= 1)
        {
            for (int i = 0; i < canvasImages.Length; i++)
            {
                Image image = inputCanvas.gameObject.GetComponentsInChildren<Image>()[i];
                alphaValueColour = new Color(image.color.r, image.color.g, image.color.b, value);
                inputCanvas.gameObject.GetComponentsInChildren<Image>()[i].color = alphaValueColour;
                Debug.LogWarning($"fading {image.gameObject.name} to {value}");
            }
        }

        if (canvasText.Length >= 1)
        {
            for (int i = 0; i < canvasText.Length; i++)
            {
                Text text = inputCanvas.gameObject.GetComponentsInChildren<Text>()[i];
                alphaValueColour = new Color(text.color.r, text.color.g, text.color.b, value);
                inputCanvas.gameObject.GetComponentsInChildren<Text>()[i].color = alphaValueColour;
                Debug.LogWarning($"fading {text.gameObject.name} to {value}");
            }
        }
    }

    IEnumerator LoadCharacterSelectionScreen()
    {
        SoundManager.PlaySound(SoundManager.Sound.CannonShot);
        //Debug.LogError("sound started");
        yield return new WaitForSecondsRealtime(4.0f);
        //Debug.LogError("sound over");
        GameManager.manager.ChangeState(GameState.CHARACTERSELECTION);
        //SoundManager.PlayMusic(SoundManager.Sound.CharacterSelectionMusic);
    }

    IEnumerator LoadGameplay()
    {
        SoundManager.PlaySound(SoundManager.Sound.CannonShot);
        //Debug.LogError("sound started");
        GameManager.manager.uiManager.playButton.interactable = false;
        yield return new WaitForSecondsRealtime(4.0f);
        //Debug.LogError("sound over");
        GameManager.manager.ChangeState(GameState.LOADINGSCREEN);
        SwitchLevel(0);
    }
    public float GetBrightnessSliderValue()
    {
        return GameManager.manager.uiManager.brightnessSlider.value;
    }

    #endregion

    #region Options
    public void SetBrightness(float brightnessValue)
    {
        // sets the alpha of an image to the value of a slider
        Color newAlpha = GameManager.manager.uiManager.brightnessImage.color;
        newAlpha.a = brightnessValue;
        GameManager.manager.uiManager.brightnessImage.color = newAlpha;

        //Debug.Log($"newAlpha: {newAlpha.a}, Image colour: {GameManager.manager.uiManager.brightnessImage.color}");
    }

    public void SetVolume(float volumeValue)
    {
        AudioListener.volume = volumeValue;
    }
    #endregion
}

