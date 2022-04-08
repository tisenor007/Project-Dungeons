using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodTrap : Trap
{
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject breakableDoors;
    private float waterRiseRate = 0.1f;
    private float waterLevel = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (trapTriggered)
        {
            case true:
                water.SetActive(true);
                waterLevel = waterLevel += Time.deltaTime * waterRiseRate;
                water.transform.position = new Vector3(water.transform.position.x, waterLevel, water.transform.position.z);
                ReplaceOpenDoors();
                if (water.transform.position.y >= 5)
                { GameManager.manager.playerStats.TakeDamage(GameManager.manager.playerStats.MaxHealth, GameManager.manager.playerStats.transform); }
                GameManager.manager.playerStats.inWater = true;
                if (!soundPlaying)
                { GameManager.manager.ChangeGamePlayState(GamePlayState.FloodRoom); SoundManager.PlaySound(SoundManager.Sound.RockCollapsing); soundPlaying = true; }
                break;
            case false:
                water.SetActive(false);
                OpenAllBreakableDoors();
                GameManager.manager.playerStats.inWater = false;
                if (soundPlaying)
                { GameManager.manager.ChangeGamePlayState(GamePlayState.Default); soundPlaying = false; }
                break;
        }

        if (anyDoorBroken()){ trapTriggered = false; }
        
    }

    private void ReplaceOpenDoors()
    {
        for (int i = 0; i < thisStructure.doors.transform.childCount; i++)
        {
            if (thisStructure.doors.transform.GetChild(i).gameObject.activeSelf == false) 
            { breakableDoors.transform.GetChild(i).gameObject.SetActive(true); }
        }
    }

    private bool anyDoorBroken()
    {
        foreach (Transform child in breakableDoors.transform)
        {
            if (child.GetComponent<BreakableDoor>().doorBroken == true) { return true; }
        }
        return false;
    }

    private void OpenAllBreakableDoors()
    {
        foreach (Transform child in breakableDoors.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!anyDoorBroken()) { trapTriggered = true; 
                GameManager.manager.levelManager.CreatePopUp("DIG DIRT TO ESCAPE!", GameManager.manager.playerStats.transform.position, Color.red); }
        }
    }
}
