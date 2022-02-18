using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// game manager
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System;

// disappearing text
using UnityEngine.UI;
using TMPro;

public enum GameState
{ 
    TITLEMENU,
    GAMEPLAY,
    WIN,
    LOSE,
    PAUSE,
    OPTIONS,
    CREDITS,
    CHARACTERSELECTION
}

public class GameManager : MonoBehaviour
{
    public static GameManager manager; //singleton inst
    public LevelManager levelManager;
    public UIManager uiManager;
    public GameObject playerAndCamera;
    public PlayerStats playerStats;
    public CharacterSelection characterSelection;
    [HideInInspector]public GameState gameState;

    [Space]
    [SerializeField] private TextMeshProUGUI saveText;
    [SerializeField] private TextMeshProUGUI loadText;
    [SerializeField] private GameObject popUpPrefab;
    private GameState savedScreenState;
    // title acts as default state
    private bool gameplay;
    private bool paused;

    private bool fadeSave;
    private bool fadeLoad;
    private float textFadeWaitTime = 1.5f;

    //Create a new object within the player, one will be male, the other female
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject malePlayer;
    [SerializeField] private GameObject femalePlayer;

    void Awake()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(this.gameObject);
            manager = this; // setting this object to be THE singleton
        }
        else if (manager != this) // already exist's? DESTROY
        {
            Destroy(this.gameObject);
        }

        // make fading text invisible at start
        saveText.CrossFadeAlpha(0, .1f, true);
        loadText.CrossFadeAlpha(0, .1f, true);

        gameState = GameState.TITLEMENU;
    }

    void Update()
    {
        Controls();

        FadeText();

        levelManager.LoadButtonFade(File.Exists(Application.persistentDataPath + "/savedInfo.dat"));
        //Debug.Log(gameState);

        switch (gameState)
        {
            case GameState.TITLEMENU:
                {
                    if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
                    {
                        SceneManager.LoadScene(0, LoadSceneMode.Single);
                        SaveScreenState();
                    }
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    uiManager.LoadTitleMenu();
                    characterSelection.HideModels();

                    playerAndCamera.SetActive(false);
                    return;
                }
            case GameState.GAMEPLAY:
                {
                    if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(1))
                    {
                        SceneManager.LoadScene(1, LoadSceneMode.Single);
                        SaveScreenState();
                    }
                    if (Time.timeScale == 0) { Time.timeScale = 1; }
                    uiManager.LoadGameplay();
                    characterSelection.HideModels();

                    //if (Input.GetKey(KeyCode.Return) == true)
                    //{
                    //    Save();
                    //}

                    playerAndCamera.SetActive(true);
                    return;
                }
            case GameState.WIN:
                {
                    uiManager.LoadWinScreen();
                    characterSelection.HideModels();
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    return;
                }
            case GameState.LOSE:
                {
                    uiManager.LoadLoseScreen();
                    characterSelection.HideModels();
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    return;
                }
            case GameState.PAUSE:
                {
                    if (Time.timeScale == 1) { Time.timeScale = 0; }

                    uiManager.LoadPauseScreen();
                    characterSelection.HideModels();
                    return;
                }
            case GameState.OPTIONS:
                {
                    uiManager.LoadOptions();
                    characterSelection.HideModels();
                    return;
                }
            case GameState.CREDITS:
                {
                    if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(2))
                    {
                        SceneManager.LoadScene(2, LoadSceneMode.Single);
                        SaveScreenState();
                    }
                    if (Time.timeScale == 0) { Time.timeScale = 1; }
                    uiManager.LoadCredits();
                    characterSelection.HideModels();

                    playerAndCamera.SetActive(false);
                    return;
                }
            case GameState.CHARACTERSELECTION:
                {
                    if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(3))
                    {
                        SceneManager.LoadScene(3, LoadSceneMode.Single);
                        SaveScreenState();
                    }
                    uiManager.LoadCharacterSelection();
                    characterSelection.ShowModels();

                    playerAndCamera.SetActive(false);
                    return;
                }
        }
    }

    public void CreatePopUp(string message, Vector3 popUpPos, Color color)
    {
        levelManager.CreatePopUp(message, popUpPos, popUpPrefab, color);
    }

    public void ChangeState(GameState targetState)
    {
        gameState = targetState;
    }

    public void SaveScreenState()
    {
        savedScreenState = gameState;
    }

    public void ReturnToPreviousState()
    {
        gameState = savedScreenState;
    }

    private void Controls() // Global Controls
    {
        // quick save/load
        /*if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }*/

        if (gameState != GameState.TITLEMENU)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                levelManager.ChangeGameStateToPause();
            }
        }
    }

    public void Save() // canned file save method
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedInfo.dat");

        SaveInfo savedInfo = new SaveInfo();
        savedInfo.scene = SceneManager.GetActiveScene().buildIndex;
        savedInfo.activeScreen = levelManager.activeScreen;
        savedInfo.gameState = gameState;
        savedInfo.health = playerStats.health;
        savedInfo.genderStatus = characterSelection.isMale;

        saveText.CrossFadeAlpha(1, .1f, true);
        StartCoroutine(WaitToFadeText("save"));

        bf.Serialize(file, savedInfo);
        file.Close();
        Debug.Log("SAVED");
    }

    public void Load() // canned file load method
    {
        if (File.Exists(Application.persistentDataPath + "/savedInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedInfo.dat", FileMode.Open);
            SaveInfo loadedInfo = (SaveInfo)bf.Deserialize(file);
            file.Close();

            //temp Q&D
            playerStats.ResetStats();
            //
            SceneManager.LoadScene(loadedInfo.scene);
            levelManager.activeScreen = loadedInfo.activeScreen;
            gameState = loadedInfo.gameState;
            playerStats.health = loadedInfo.health;
            characterSelection.isMale = loadedInfo.genderStatus;
            characterSelection.SetPlayerModel();

            loadText.CrossFadeAlpha(1, .1f, true);
            StartCoroutine(WaitToFadeText("load"));
        }
    }

    public void FadeText()
    {
        if (Time.timeScale == 0)
        {
            saveText.CrossFadeAlpha(0, 0, true); fadeSave = false;
            loadText.CrossFadeAlpha(0, 0, true); fadeLoad = false;
        }
        if (fadeSave)
        {
            saveText.CrossFadeAlpha(0, 3, false); fadeSave = false;
        }
        if (fadeLoad)
        {
            loadText.CrossFadeAlpha(0, 3, false); fadeLoad = false;
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    IEnumerator WaitToFadeText(string fade)
    {
        yield return new WaitForSeconds(textFadeWaitTime);
        if (fade == "save")
            fadeSave = true;
        else if (fade == "load")
            fadeLoad = true;
    }

    public void SwitchPlayer(bool isMale)
    {
        malePlayer.SetActive(isMale);
        femalePlayer.SetActive(!isMale);

        if (isMale)
        {
            playerController.animator = malePlayer.GetComponent<Animator>();
        } else
        {
            playerController.animator = femalePlayer.GetComponent<Animator>();
        }
        PlayerStats playerStats = playerController.GetComponent<PlayerStats>();
        playerStats.SetGender(isMale);
    }
}

[Serializable]
class SaveInfo
{
    public int activeScreen;
    public GameState gameState;
    public int scene;
    public int health;
    public bool genderStatus;
}

