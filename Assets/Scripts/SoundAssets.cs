using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAssets : MonoBehaviour
{
    private static SoundAssets _i;

    public static SoundAssets i
    {
        get
        {
            if (_i == null)
            {
                _i = (Instantiate(Resources.Load("SoundAssets")) as GameObject).GetComponent<SoundAssets>();
            }
            return _i;
        }
    }

    public SoundAudioClip[] soundAudioClipArray;

    void Awake()
    {
        if (_i == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _i = this; // setting this object to be THE singleton
        }
        else if (_i != this) // already exist's? DESTROY
        {
            Destroy(this.gameObject);
        }
    }
    /*private void Start()
    {
       
        foreach (SoundAudioClip soundAudioClip in soundAudioClipArray)
        {

            //soundAudioClip.soundLength = soundAudioClip.audioClipArray[0].length;
            //soundTimerDictionary[sound] = soundAudioClip.soundDelayTimer;

            //Debug.LogError(soundAudioClip.soundLength);

        }
    }*/

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip[] audioClipArray;
        //public float soundLength;
        //public float lastTimePlayed = 0;
    }

}
