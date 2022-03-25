using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// game manager
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEditor;
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
    CHARACTERSELECTION,
    SAVEOPTION,
    LOADINGSCREEN
}

public class GameManager : MonoBehaviour
{
    public static GameManager manager; //singleton inst
    public LevelManager levelManager;
    public UIManager uiManager;
    public GameObject playerAndCamera;
    public PlayerStats playerStats;
    public CharacterSelection characterSelection;
    public GameObject[] levels;
    public Weapon playerSavedWeapon;
    [HideInInspector] public GameState gameState;
    [HideInInspector] public int currentLevel = 0;
    [HideInInspector] public GameObject currentPlayerModel;
    [HideInInspector] public PlayerController playerController;
    private GameState savedScreenState;
    private string dataPathSaveLoadDIT = "/savedInfo.dat";
    
    // title acts as default state
    private bool gameplay;
    private bool paused;
    private bool fadeSave;
    private bool fadeLoad;
    private float textFadeWaitTime = 1.5f;

    [Space]
    [SerializeField] private TextMeshProUGUI saveText;
    [SerializeField] private TextMeshProUGUI loadText;
    
    //Create a new object within the player, one will be male, the other female
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

        //levelManager.ChangeGameStateToLoadingScreen();
        gameState = GameState.LOADINGSCREEN;

        
    }

    void Update()
    {
        Controls();

        FadeText();

        levelManager.LoadButtonFade(File.Exists(Application.persistentDataPath + dataPathSaveLoadDIT));
        levelManager.NextLevelButtonFade(currentLevel);
        levelManager.UpdateDungeon();
        AudioListener.volume = 0.25f;

        switch (gameState)
        {
            case GameState.TITLEMENU:
                {
                    SoundManager.PlaySound(SoundManager.Sound.MenuMusic);
                    if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
                    {
                        SceneManager.LoadScene(0, LoadSceneMode.Single);
                        SaveScreenState();
                    }
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    uiManager.LoadTitleMenu();
                    characterSelection.HideModels();
                    Cursor.visible = true;

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
                    playerAndCamera.SetActive(true);
                    UpdatePlayerVitalStatusAppearance(characterSelection.isMale, playerStats.IsAlive);
                    playerStats.UpdateWeaponHitArea(characterSelection.isMale);
                    Cursor.visible = false;

                    SoundManager.PlaySound(SoundManager.Sound.CaveAmbience);
                    SoundManager.PlaySound(SoundManager.Sound.GameplayMusic);
                    SoundManager.PlaySound(SoundManager.Sound.WaterDripping);
                    //Debug.LogError("SOUND PLAYED");
                    return;
                }
            case GameState.WIN:
                {
                    uiManager.LoadWinScreen();
                    characterSelection.HideModels();
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    Cursor.visible = true;
                    return;
                }
            case GameState.LOSE:
                {
                    uiManager.LoadLoseScreen();
                    characterSelection.HideModels();
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    Cursor.visible = true;
                    return;
                }
            case GameState.PAUSE:
                {
                    if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(1))
                    {
                        SceneManager.LoadScene(1, LoadSceneMode.Single);
                        SaveScreenState();
                    }
                    playerAndCamera.SetActive(true);
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    Cursor.visible = true;

                    uiManager.LoadPauseScreen();
                    characterSelection.HideModels();
                    return;
                }
            case GameState.OPTIONS:
                {
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    uiManager.LoadOptions();
                    characterSelection.HideModels();
                    Cursor.visible = true;
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
                    Cursor.visible = true;
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
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    uiManager.LoadCharacterSelection();
                    characterSelection.ShowModels();
                    Cursor.visible = true;
                    playerAndCamera.SetActive(false);
                    return;
                }
            case GameState.SAVEOPTION:
                {
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    uiManager.LoadSaveOption();
                    characterSelection.HideModels();
                    Cursor.visible = true;
                    return;
                }
            case GameState.LOADINGSCREEN:
                {
                    if (Time.timeScale == 1) { Time.timeScale = 0; }
                    playerAndCamera.SetActive(true);
                    Cursor.visible = false;
                    if (levels[currentLevel].GetComponent<DungeonGenerator>().dungeonGenerated == false)
                    {
                        levels[currentLevel].GetComponent<DungeonGenerator>().dungeonIsGenerating = true;
                    }
                    uiManager.LoadLoadingScreen();
                    characterSelection.HideModels();
                    if (levels[currentLevel].activeSelf == true && levels[currentLevel].GetComponent<DungeonGenerator>().dungeonGenerated == true) 
                    {
                        levelManager.ChangeGameStateToTitleMenu();
                    }
                    return;
                }
        }
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

        if (gameState == GameState.TITLEMENU) { return; }
        if (gameState == GameState.CREDITS) { return; }
        if (gameState == GameState.LOADINGSCREEN) { return; }
        if (gameState == GameState.LOSE) { return; }
        if (gameState == GameState.WIN) { return; }
        if (gameState == GameState.SAVEOPTION) { return; }
        if (gameState == GameState.OPTIONS) { return; }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            levelManager.ChangeGameStateToPause();
        }
        
    }

    public void Save() // canned file save method
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + dataPathSaveLoadDIT);

        SaveInfo savedInfo = new SaveInfo();
        savedInfo.scene = SceneManager.GetActiveScene().buildIndex;
        savedInfo.activeScreen = levelManager.activeScreen;
        savedInfo.gameState = gameState;
        savedInfo.health = playerStats.Health;
        savedInfo.JsonWeapon = JsonUtility.ToJson(playerStats.CurrentWeaponType);
        savedInfo.genderStatus = characterSelection.isMale;
        savedInfo.playerSpawnPosX = playerAndCamera.transform.GetChild(0).gameObject.transform.position.x;
        savedInfo.playerSpawnPosY = playerAndCamera.transform.GetChild(0).gameObject.transform.position.y;
        savedInfo.playerSpawnPosZ = playerAndCamera.transform.GetChild(0).gameObject.transform.position.z;
        savedInfo.SaveDungeon(levels[currentLevel].GetComponent<DungeonGenerator>().structures);

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
            FileStream file = File.Open(Application.persistentDataPath + dataPathSaveLoadDIT, FileMode.Open);
            SaveInfo loadedInfo = (SaveInfo)bf.Deserialize(file);
            file.Close();

            //temp Q&D
            playerStats.ResetStats();
            //
            SceneManager.LoadScene(loadedInfo.scene);
            loadedInfo.LoadSavedDungeon(ref currentLevel, levels);
            levelManager.activeScreen = loadedInfo.activeScreen;
            gameState = loadedInfo.gameState;
            playerStats.TakeDamage(playerStats.MaxHealth - loadedInfo.health, playerStats.transform);
            characterSelection.isMale = loadedInfo.genderStatus;
            playerStats.RespawnPos = new Vector3(loadedInfo.playerSpawnPosX, loadedInfo.playerSpawnPosY, loadedInfo.playerSpawnPosZ);
            playerAndCamera.transform.GetChild(0).position = playerStats.RespawnPos;
            characterSelection.SetPlayerModel();
            JsonUtility.FromJsonOverwrite(loadedInfo.JsonWeapon, playerSavedWeapon);
            playerSavedWeapon.Equip(playerSavedWeapon.prefab, playerAndCamera.transform.GetChild(0).gameObject, false);
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
            currentPlayerModel = malePlayer;
            playerController.animator = malePlayer.GetComponent<Animator>();
        } else
        {
            currentPlayerModel = femalePlayer;
            playerController.animator = femalePlayer.GetComponent<Animator>();
        }

        playerStats.UpdateWeaponHitArea(characterSelection.isMale);
    }

    public void UpdatePlayerVitalStatusAppearance(bool isMale, bool isAlive)
    {
        if (isMale) { currentPlayerModel = malePlayer; }
        else if (!isMale) { currentPlayerModel = femalePlayer; }
        if (isAlive) { currentPlayerModel.SetActive(true); }
        else if (!isAlive) { currentPlayerModel.SetActive(true); }
    }
}

[Serializable]
class SaveInfo
{
    public int activeScreen;
    public GameState gameState;
    public int scene;
    public int health;
    public string JsonWeapon;
    public bool genderStatus;
    public float playerSpawnPosX;
    public float playerSpawnPosY;
    public float playerSpawnPosZ;
    public int savedLevel;
    public List<DungeonGenerator.StructureType> savedStructureTypes = new List<DungeonGenerator.StructureType>();
    public List<int> savedStructureVariations = new List<int>();
    public List<float> savedStructsPosX = new List<float>();
    public List<float> savedStructsPosY = new List<float>();
    public List<float> savedStructsPosZ = new List<float>();
    public List<float> savedStructsRotX = new List<float>();
    public List<float> savedStructsRotY = new List<float>();
    public List<float> savedStructsRotZ = new List<float>();
    public int savedStructureAmount;

    public void SaveDungeon(List<GameObject> structures)
    {
        savedLevel = GameManager.manager.currentLevel;
        savedStructureTypes.Clear();
        savedStructureVariations.Clear();
        savedStructsPosX.Clear();
        savedStructsPosY.Clear();
        savedStructsPosZ.Clear();
        savedStructsRotX.Clear();
        savedStructsRotY.Clear();
        savedStructsRotZ.Clear();
        savedStructureAmount = structures.Count;

        for (int i = 0; i < savedStructureAmount; i++)
        {
            savedStructureTypes.Add(structures[i].GetComponent<StructureBehavior>().currentStructureType);
            savedStructureVariations.Add(structures[i].GetComponent<StructureBehavior>().currentVariation);

            savedStructsPosX.Add(structures[i].transform.position.x);
            savedStructsPosY.Add(structures[i].transform.position.y);
            savedStructsPosZ.Add(structures[i].transform.position.z);

            savedStructsRotX.Add(structures[i].transform.eulerAngles.x);
            savedStructsRotY.Add(structures[i].transform.eulerAngles.y);
            savedStructsRotZ.Add(structures[i].transform.eulerAngles.z);
        }
    }

    public void LoadSavedDungeon(ref int currentLevel, GameObject[] levels)
    {
        currentLevel = savedLevel;

        foreach (GameObject level in GameManager.manager.levels)
        {
            if (level.GetComponent<DungeonGenerator>().dungeonGenerated) { level.GetComponent<DungeonGenerator>().ClearDungeon(); }
        }

        for (int i = 0; i < savedStructureAmount; i++)
        {
            levels[currentLevel].GetComponent<DungeonGenerator>().GenerateSavedRoom(savedStructureTypes[i], savedStructureVariations[i], new Vector3(savedStructsPosX[i], savedStructsPosY[i], savedStructsPosZ[i]), new Vector3(savedStructsRotX[i], savedStructsRotY[i], savedStructsRotZ[i]));
        }

        levels[currentLevel].GetComponent<DungeonGenerator>().CompleteGeneration();
    }
}

