using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    public bool rotates = false;

    private float feedback__Intensity = 3;
    private Light feedbackLight; //feedback to show object is interactable
    private bool feedbackEnabled;
    private bool interactableEnabled = true;
    private GameObject interactableObject;
    private float rotationSpeed = 40f;
    private float bounceSpeed = 30f;

    //types of interactables
    [SerializeField] private Item itemType;

    public bool FeedbackEnabled { get { return feedbackEnabled; } }
    public bool InteractableEnabled { get { return interactableEnabled; } }

    void Start()
    {
        interactableObject = this.gameObject.transform.GetChild(0).gameObject;
        feedbackLight = this.gameObject.GetComponent<Light>();

        interactableEnabled = true;
        feedbackEnabled = false;
        feedbackLight.intensity = 0;

        DisableFeedback();
    }

    void Update()
    {
        if (rotates)
        {
            this.gameObject.transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime, Space.World);
            if (this.transform.localPosition.y >= 10)
            { 
                
            
            } 
        }

        if (feedbackEnabled == false && feedbackLight.intensity > 0)
        {
            feedbackLight.intensity -= Time.deltaTime * 10;
        }
        else if (feedbackEnabled == true && feedbackLight.intensity < feedback__Intensity)
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
