using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrap : Trap
{
    [SerializeField] private GameObject bossObj;

    // Update is called once per frame
    void Update()
    {
        if (bossObj.GetComponent<Boss>().IsAlive == false) { thisStructure.trapPlayer = false; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (bossObj.GetComponent<Boss>().IsAlive == true) { thisStructure.trapPlayer = true; bossObj.SetActive(true); }
        }
    }
}
