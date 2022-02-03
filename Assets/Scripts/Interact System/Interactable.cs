using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    public float interactableFeedback__Intensity = 10;

    private Light feedbackLight; //feedback to show object is interactable
    private bool feedbackEnabled;
    private bool interactableEnabled = true;

    //types of interactables
    public Item itemType;

    public bool InteractableEnabled { get { return interactableEnabled; } }

    void Start()
    {
        interactableEnabled = true;
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
        else if (interactableEnabled == false && feedbackLight.intensity <= 0)
        { 
            this.gameObject.SetActive(false);
        }
    }

    public void Interact(GameObject interactor)
    {
        Debug.LogError("Interacted");
        
        if (itemType != null) { itemType.OnPickup(interactor); }
        
        RemoveGameObjectWFeedback();
    }

    public void EnableFeedback()
    {
        if (feedbackEnabled != true && interactableEnabled) //quickfix: only enable feedback if object isnt being deleted
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
        interactableEnabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        DisableFeedback();
    }
}
