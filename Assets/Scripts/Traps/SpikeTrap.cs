using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Trap
{
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject planks;
    private bool soundPlayed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (trapTriggered)
        {
            floor.SetActive(false);
            planks.SetActive(true);
            if (soundPlayed == false)
            { SoundManager.PlaySound(SoundManager.Sound.SpikeWoodBreak); soundPlayed = true; }
        }
        else if (!trapTriggered)
        {
            floor.SetActive(true);
            planks.SetActive(false);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            trapTriggered = true;
        }
    }
}
