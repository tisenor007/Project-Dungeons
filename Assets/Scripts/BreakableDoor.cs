using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableDoor : MonoBehaviour
{
    [HideInInspector] public bool doorBroken = false;
    private bool playerInRange;
    private int doorHealth = 400;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E)) { doorHealth = doorHealth - 10; GameManager.manager.levelManager.CreatePopUp("KEEP DIGGING!", transform.position, Color.green); }
        if (doorHealth <= 0) { doorBroken = true; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
