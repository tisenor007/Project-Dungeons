using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    public bool InteractableEnabled { get { return interactableEnabled; } }

    [SerializeField] private float interactableFeedback__Intensity = 10;

    private Light feedbackLight; //feedback to show object is interactable
    private bool feedbackEnabled;
    private bool interactableEnabled = true;
    private GameObject interactableObject;

    //types of interactables
    [SerializeField] private Item itemType;


    void Start()
    {
        interactableObject = GetComponentInChildren<GameObject>();
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
        if (itemType != null) { itemType.OnPickup(this.gameObject, interactor); }
        else { RemoveGameObjectWFeedback(); }
    }

    public void EnableFeedback()
    {
        if (feedbackEnabled != true && interactableEnabled) //quickfix: only enable feedback if object isnt being deleted
        { 
            feedbackEnabled = true;
            //Debug.Log("enabled feedback");
        }
    }

    public void DisableFeedback()
    {
        if (feedbackEnabled != false)
        { 
            feedbackEnabled = false;
            //Debug.Log("disabled feedback");
        }
    }

    public void RemoveGameObjectWFeedback()
    {
        interactableEnabled = false;
        interactableObject.GetComponent<MeshRenderer>().enabled = false;
        interactableObject.GetComponent<Collider>().enabled = false;
        DisableFeedback();
    }
}
