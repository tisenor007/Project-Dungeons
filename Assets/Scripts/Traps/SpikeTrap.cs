using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Trap
{
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject planks;
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
                floor.SetActive(false);
                planks.SetActive(true);
                break;
            case false:
                floor.SetActive(true);
                planks.SetActive(false);
                break;
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
