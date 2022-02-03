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
    CREDITS
}

public class GameManager : MonoBehaviour
{
    public static GameManager manager; //singleton inst
    public LevelManager levelManager;
    public UIManager uiManager;
    public TextMeshProUGUI saveText;
    public TextMeshProUGUI loadText;
    public GameObject playerAndCamera;
    public GameObject popUpPrefab;
    private GameState gameState;
    private GameState savedScreenState;
    // title acts as default state
    private bool gameplay;
    private bool paused;

    private bool fadeSave;
    private bool fadeLoad;
    private float textFadeWaitTime = 1.5f;

    void Awake()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(playerAndCamera);
            manager = this; // setting this object to be THE singleton
        }
        else if (manager != this) // already exist's? DESTROY
        {
            Destroy(this.gameObject);
            Destroy(playerAndCamera);
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
                    if (Time.timeScale == 0) {Time.timeScale = 1;}
                    uiManager.LoadGameplay();

                    playerAndCamera.SetActive(true);
                    return;
                }
            case GameState.WIN:
                {
                    uiManager.LoadWinScreen();
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    return;
                }
            case GameState.LOSE:
                {
                    uiManager.LoadLoseScreen();
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    return;
                }
            case GameState.PAUSE:
                {
                    if (Time.timeScale == 1) { Time.timeScale = 0; }

                    uiManager.LoadPauseScreen();
                    return;
                }
            case GameState.OPTIONS:
                {
                    uiManager.LoadOptions();
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

        saveText.CrossFadeAlpha(1, .1f, true);
        StartCoroutine(WaitToFadeText("save"));

        bf.Serialize(file, savedInfo);
        file.Close();
    }
    
    public void Load() // canned file load method
    {
        if (File.Exists(Application.persistentDataPath + "/savedInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedInfo.dat", FileMode.Open);
            SaveInfo loadedInfo = (SaveInfo)bf.Deserialize(file);
            file.Close();

            SceneManager.LoadScene(loadedInfo.scene);
            levelManager.activeScreen = loadedInfo.activeScreen;
            gameState = loadedInfo.gameState;

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
}

[Serializable]
class SaveInfo
{
    public int activeScreen;
    public GameState gameState;
    public int scene;
}

