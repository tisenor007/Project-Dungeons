using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public abstract class Interactable : MonoBehaviour
{
    public float interactableFeedback__Intensity = 10;

    private Light feedbackLight; //feedback to show object is interactable
    private bool feedbackEnabled;
    private bool interactableEnabled = true;

    //types of interactables
    private Item item;

    public bool InteractableEnabled { get; }

    void Start()
    {
        if (TryGetComponent<Item>(out Item itemGet))
        {
            item = itemGet;
            Debug.Log($"{gameObject.name} is a {item} interactable, (ITEM) Type.");
        }
        feedbackLight = GetComponent<Light>();
        feedbackLight.intensity = 0;
        DisableFeedback();
    }

    void Update()
    {
        if (feedbackEnabled == false && feedbackLight.intensity > 0)
        {
            feedbackLight.intensity -= Time.deltaTime * 10;
        }
        else if (feedbackEnabled == true && feedbackLight.intensity < interactableFeedback__Intensity)
        {
            feedbackLight.intensity += Time.deltaTime * 30;
        }
        else if (interactableEnabled && feedbackLight.intensity <= 0)
        { 
            this.gameObject.SetActive(false);
        }
    }

    public void Interact(GameObject interactee)
    {
        Debug.LogError("Interacted");
        
        if (item != null) { item.OnPickup(interactee); }
        
        RemoveGameObjectWFeedback();
    }

    public void EnableFeedback()
    {
        if (feedbackEnabled != true && !interactableEnabled) //quickfix: only enable feedback if object isnt being deleted
        { 
            feedbackEnabled = true;
            Debug.Log("enabled feedback");
        }
    }

    public void DisableFeedback()
    {
        if (feedbackEnabled != false)
        { 
            feedbackEnabled = false;
            Debug.Log("disabled feedback");
        }
    }

    public void RemoveGameObjectWFeedback()
    {
        interactableEnabled = true;
        GetComponent<MeshRenderer>().enabled = false;
        DisableFeedback();
    }
}
