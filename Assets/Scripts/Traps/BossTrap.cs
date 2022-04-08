using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrap : Trap
{
    [SerializeField] private GameObject bossObj;

    // Update is called once per frame
    void Update()
    {
        switch (trapTriggered)
        {
            case true:
                thisStructure.trapPlayer = true;
                bossObj.SetActive(true);
                if (!soundPlaying)
                { GameManager.manager.ChangeGamePlayState(GamePlayState.BossRoom); soundPlaying = true; }
                break;
            case false:
                thisStructure.trapPlayer = false;
                //bossObj.SetActive(false);
                if (soundPlaying) 
                { GameManager.manager.ChangeGamePlayState(GamePlayState.Default); soundPlaying = false; }
                break;
        }
        if (bossObj.GetComponent<Boss>().IsAlive == false) { trapTriggered = false; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (bossObj.GetComponent<Boss>().IsAlive == true) { trapTriggered = true; }
        }
    }
}
