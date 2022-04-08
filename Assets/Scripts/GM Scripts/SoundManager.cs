using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    public static GameManager gameManager;
    public static GameObject soundManager;

    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;
    public static GameObject oneShotMusicGameObject;
    private static AudioSource oneShotMusicAudioSource;

    private static Sound currentMusic;
    private static Sound previousMusic;

    private static Dictionary<Sound, float> soundTimeDictionary;
    public static Dictionary<Sound, float> soundLengthDictionary;
    private static Dictionary<Sound, float> soundVolumeDictionary;
    //public static Dictionary<Sound, float> musicCutOffDictionary;
    private static SoundAssets.SoundAudioClip currentSoundAudioClip;
    private static float currentArrayAudioClipLength;

    [HideInInspector] public static bool canMusicPlay = true;

    private void Start()
    {
        oneShotMusicGameObject = new GameObject("One Shot Music Sound");
        oneShotMusicAudioSource = oneShotMusicGameObject.AddComponent<AudioSource>();
        oneShotMusicGameObject.transform.parent = GameObject.Find("SoundManager").transform;
        //if (oneShotMusicGameObject != null) { oneShotAudioSource = oneShotMusicGameObject.GetComponent<AudioSource>(); }
    }

    public enum Sound
    {
        CannonShot,
        CaveAmbience,
        Clang,
        CreepyCaveNoise,
        GameplayMusic,
        GhostAttack, 
        GhostChasing,
        GhostDeath,
        GhostIdle,
        MenuMusic,
        MetalClang,
        Punches,
        SkeletonAttack,
        SkeletonChasing,
        SkeletonDeath,
        SkeletonSteps,
        SwordClash,
        WaterDripping,
        ZombieAttack,
        ZombieChasing,
        ZombieDeath,
        ZombieIdle,
        PauseMusic,
        CharacterSelectionMusic,
        CreditMusic,
        Cork,
        PaperAway,
        FloodTrapMusic,
        SpikeWoodBreak,
        BossMusic,
        RockCollapsing,
        WinMusic,
        LossMusic,
        EatingSFX,
        BossIdleRest,
        BossChasing,
        BossAttack,
        BossDeath
    }

    public static void InitializeDictionary()
    {
        soundTimeDictionary = new Dictionary<Sound, float>();
        soundLengthDictionary = new Dictionary<Sound, float>();
        soundVolumeDictionary = new Dictionary<Sound, float>();
        //musicCutOffDictionary = new Dictionary<Sound, float>();
        SetDirectory(Sound.CannonShot, 0, 0, 1);
        SetDirectory(Sound.CaveAmbience, 0, 0, 0.5f);
        SetDirectory(Sound.Clang, 0, 0, 1);
        SetDirectory(Sound.Cork, 0, 0, 1);
        SetDirectory(Sound.CreditMusic, 0, 0, 1);
        SetDirectory(Sound.CharacterSelectionMusic, 0, 0, 1);
        SetDirectory(Sound.GameplayMusic, 0, 0, 1f);
        SetDirectory(Sound.GhostAttack, 0, 0, 0.5f);
        SetDirectory(Sound.GhostChasing, 0, 0, 0.5f);
        SetDirectory(Sound.GhostDeath, 0, 0, 0.5f);
        SetDirectory(Sound.GhostIdle, 0, 0, 0.5f);
        SetDirectory(Sound.MenuMusic, 0, 0, 0.5f);
        SetDirectory(Sound.MetalClang, 0, 0, 1);
        SetDirectory(Sound.PauseMusic, 0, 0, 1);
        SetDirectory(Sound.PaperAway, 0, 0, 1);
        SetDirectory(Sound.Punches, 0, 0, 1);
        SetDirectory(Sound.SkeletonAttack, 0, 0, 0.5f);
        SetDirectory(Sound.SkeletonChasing, 0, 0, 0.5f);
        SetDirectory(Sound.SkeletonDeath, 0, 0, 0.5f);
        SetDirectory(Sound.SkeletonSteps, 0, 0, 0.5f);
        SetDirectory(Sound.WaterDripping, 0, 0, 1);
        SetDirectory(Sound.ZombieAttack, 0, 0, 0.5f);
        SetDirectory(Sound.ZombieChasing, 0, 0, 0.5f);
        SetDirectory(Sound.ZombieDeath, 0, 0, 0.5f);
        SetDirectory(Sound.ZombieIdle, 0, 0, 0.5f);
        SetDirectory(Sound.FloodTrapMusic, 0, 0, 1f);
        SetDirectory(Sound.SpikeWoodBreak, 0, 0, 1f);
        SetDirectory(Sound.BossMusic, 0, 0, 1f);
        SetDirectory(Sound.RockCollapsing, 0, 0, 3f);
        SetDirectory(Sound.WinMusic, 0, 0, 1f);
        SetDirectory(Sound.LossMusic, 0, 0, 1f);
        SetDirectory(Sound.EatingSFX, 0, 0, 1f);
        SetDirectory(Sound.BossIdleRest, 0, 0, 1f);
        SetDirectory(Sound.BossChasing, 0, 0, 1f);
        SetDirectory(Sound.BossAttack, 0, 0, 1f);
        SetDirectory(Sound.BossDeath, 0, 0, 1f);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private static void SetDirectory(Sound sound, float length, float delay, float volume)
    {
        soundTimeDictionary[sound] = delay;
        soundLengthDictionary[sound] = length;
        soundVolumeDictionary[sound] = volume;
    }

    /*private static void SetDirectory(Sound sound, float length, float delay, float volume, float cutOff)
    {
        soundTimeDictionary[sound] = delay;
        soundLengthDictionary[sound] = length;
        soundVolumeDictionary[sound] = volume;
        musicCutOffDictionary[sound] = cutOff;
    }*/

    /*public static void SetMusicClipCuttOff()
    {

        musicCutOffDictionary[previousMusic] = oneShotMusicAudioSource.time;
        Debug.LogError("cut off set: " + musicCutOffDictionary[previousMusic]);
    }*/

    public static void PlaySound(Sound sound, Vector3 position)
    {
        //GetAudioClip(sound);

        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.volume = soundVolumeDictionary[sound];
            audioSource.clip = GetAudioClip(sound);
            audioSource.Play();

            //Debug.Log("CLIP PLAYED: " + audioSource.clip.name);
            Object.Destroy(soundGameObject, audioSource.clip.length);
        } 
    }

    public static void PlaySound(Sound sound)
    {
        //Debug.Log("SOUND DELAY TIMER: " + soundTimerDictionary[sound]);
        //

        if (CanPlaySound(sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                oneShotAudioSource.PlayOneShot(GetAudioClip(sound), soundVolumeDictionary[sound]);
            }
            else
            {
                if (oneShotAudioSource != null)
                {
                    oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                    oneShotAudioSource.PlayOneShot(GetAudioClip(sound), soundVolumeDictionary[sound]);
                }
            }
        }
    }

    public static void PlayMusic(Sound sound)
    {
        GetAudioClip(sound);
        if (CanPlaySound(sound))
        {

            //if (oneShotMusicGameObject == null)
            //{
            //    oneShotMusicGameObject = new GameObject("One Shot Music Sound");
            //    oneShotMusicAudioSource = oneShotMusicGameObject.AddComponent<AudioSource>();
            //    oneShotMusicAudioSource.PlayOneShot(GetAudioClip(sound), soundVolumeDictionary[sound]);
            //    previousMusic = sound;

            //}
            //else
            //{
                oneShotMusicAudioSource.Stop();
                //soundLengthDictionary[previousMusic] = 0;
                if (sound != previousMusic)
                { soundLengthDictionary[previousMusic] = 0; previousMusic = sound; }
                oneShotMusicAudioSource.PlayOneShot(GetAudioClip(sound), soundVolumeDictionary[sound]);
                //oneShotMusicAudioSource.time = musicCutOffDictionary[sound];
            //}
        }
    }

    public static void StopOneShotSound(Sound sound)
    {
        oneShotAudioSource.clip = GetAudioClip(sound);
        oneShotAudioSource.Stop();
    }
    public static bool IsSoundPlaying(Sound sound)
    {
        oneShotAudioSource.clip = GetAudioClip(sound);
        if (oneShotAudioSource.isPlaying) { return true; }
        return false;
    }

    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;

            case Sound.CaveAmbience:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength + Random.Range(10.0f, 20.0f);
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.CharacterSelectionMusic:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.CHARACTERSELECTION) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.Clang:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    return true;
                }
                else { return true; }

            case Sound.Cork:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    return true;
                }
                else { return true; }

            case Sound.CreditMusic:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.CREDITS) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.GameplayMusic:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.GhostAttack:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = 0;
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.GhostChasing:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength;
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.GhostDeath:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = Time.time;
                        soundTimeDictionary[sound] = Time.time;
                        return false;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.GhostIdle:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength + Random.Range(5.0f, 8.0f);
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.MenuMusic:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.TITLEMENU) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.MetalClang:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = 0;
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.PauseMusic:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.PAUSE) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.PaperAway:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    return true;
                }
                else { return true; }

            case Sound.SkeletonAttack:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = 0;
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.SkeletonChasing:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength / 5; // 5 times faster
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.SkeletonDeath:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = Time.time;
                        soundTimeDictionary[sound] = Time.time;
                        return false;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.SkeletonSteps:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength / 3; // 3 times faster
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.WaterDripping:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength + Random.Range(15.0f, 25.0f);
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.ZombieAttack:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = 0;
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.ZombieChasing:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength;
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.ZombieDeath:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = Time.time;
                        soundTimeDictionary[sound] = Time.time;
                        return false;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.ZombieIdle:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength + Random.Range(4.0f, 8.0f);
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.FloodTrapMusic:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.SpikeWoodBreak:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.BossMusic:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.RockCollapsing:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = 0;
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.WinMusic:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.WIN) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.LossMusic:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.LOSE) { return false; }
                    if (!oneShotMusicAudioSource.isPlaying) { canMusicPlay = true; }
                    if (canMusicPlay)
                    {
                        canMusicPlay = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.EatingSFX:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    return true;
                }
                else { return true; }

            case Sound.BossAttack:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = 0;
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.BossChasing:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength;
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.BossDeath:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = Time.time;
                        soundTimeDictionary[sound] = Time.time;
                        return false;
                    }
                    else { return false; }
                }
                else { return true; }

            case Sound.BossIdleRest:
                if (soundTimeDictionary.ContainsKey(sound))
                {
                    if (gameManager.gameState != GameState.GAMEPLAY) { return false; }
                    float lastTimePlayed = soundTimeDictionary[sound];
                    float delayTimerMax = soundLengthDictionary[sound];
                    if (lastTimePlayed + delayTimerMax < Time.time)
                    {
                        soundLengthDictionary[sound] = currentArrayAudioClipLength + Random.Range(5.0f, 8.0f);
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
                    else { return false; }
                }
                else { return true; }
        }
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAssets.SoundAudioClip soundAudioClip in SoundAssets.i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                currentSoundAudioClip = soundAudioClip;
                int i = Mathf.FloorToInt(Random.Range(0, soundAudioClip.audioClipArray.Length));
                //Debug.Log("currentArrayAudioClip: " + i);
                currentArrayAudioClipLength = soundAudioClip.audioClipArray[i].length;

                return soundAudioClip.audioClipArray[i];
            }
        }
        return null;
    }
}


