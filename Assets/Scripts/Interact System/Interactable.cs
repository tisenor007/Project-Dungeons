using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void Interact()
    {
        this.gameObject.SetActive(false);
        Debug.LogError("Interacted");
    }
    
}
