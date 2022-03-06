using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    public static GameManager gameManager;

    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    private static Dictionary<Sound, float> soundTimeDictionary;
    private static Dictionary<Sound, float> soundLengthDictionary;
    private static SoundAssets.SoundAudioClip currentSoundAudioClip;
    private static float currentArrayAudioClipLength;
    public enum Sound
    {
        CannonShot,
        CaveAmbience,
        Clang,
        CreepyCaveNoise,
        GhostAttack, 
        GhostChasing,
        GhostDeath,
        GhostIdle,
        MetalClang,
        PirateCrew,
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
    }

    public static void InitializeDictionary()
    {
        soundTimeDictionary = new Dictionary<Sound, float>();
        soundLengthDictionary = new Dictionary<Sound, float>();
        SetDirectory(Sound.CaveAmbience, 0, 0);
        SetDirectory(Sound.GhostAttack, 0, 0);
        SetDirectory(Sound.GhostChasing, 0, 0);
        SetDirectory(Sound.GhostDeath, 0, 0);
        SetDirectory(Sound.GhostIdle, 0, 0);
        SetDirectory(Sound.PirateCrew, 0, 0);
        SetDirectory(Sound.Punches, 0, 0);
        SetDirectory(Sound.SkeletonAttack, 0, 0);
        SetDirectory(Sound.SkeletonChasing, 0, 0);
        SetDirectory(Sound.SkeletonDeath, 0, 0);
        SetDirectory(Sound.SkeletonSteps, 0, 0);
        SetDirectory(Sound.ZombieAttack, 0, 0);
        SetDirectory(Sound.ZombieChasing, 0, 0);
        SetDirectory(Sound.ZombieDeath, 0, 0);
        SetDirectory(Sound.ZombieIdle, 0, 0);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    private static void SetDirectory(Sound sound, float length, float delay)
    {
        soundTimeDictionary[sound] = delay;
        soundLengthDictionary[sound] = length;
    }
    public static void PlaySound(Sound sound, Vector3 position)
    {
        GetAudioClip(sound);

        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            audioSource.clip = GetAudioClip(sound);
            audioSource.Play();

            Debug.LogError("CLIP LENGTH: " + audioSource.clip.length);
            //Object.Destroy(soundGameObject, audioSource.clip.length);
        } 
    }

    public static void PlaySound(Sound sound)
    {
        //Debug.LogError("SOUND DELAY TIMER: " + soundTimerDictionary[sound]);
        //

        if (CanPlaySound(sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();

            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
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
                //Debug.LogError("currentArrayAudioClip: " + i);
                currentArrayAudioClipLength = soundAudioClip.audioClipArray[i].length;

                return soundAudioClip.audioClipArray[i];
            }
        }
        return null;
    }
}


