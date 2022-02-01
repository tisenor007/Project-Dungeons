using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float interactableFeedback__Intensity = 10;

    private Light feedbackLight; //feedback to show object is interactable
    private bool feedbackEnabled;
    private bool removeInteractableObject = false;

    private void Start()
    {
        feedbackLight = GetComponent<Light>();
        feedbackLight.intensity = 0;
        DisableFeedback();
    }

    private void Update()
    {
        if (feedbackEnabled == false && feedbackLight.intensity > 0)
        {
            feedbackLight.intensity -= Time.deltaTime * 10;
        }
        else if (feedbackEnabled == true && feedbackLight.intensity < interactableFeedback__Intensity)
        {
            feedbackLight.intensity += Time.deltaTime * 30;
        }
        else if (removeInteractableObject && feedbackLight.intensity <= 0)
        { 
            this.gameObject.SetActive(false);
        }
    }

    public void Interact()
    {
        Debug.LogError("Interacted");
        RemoveGameObjectWFeedback();
    }

    public void EnableFeedback()
    {
        if (feedbackEnabled != true && !removeInteractableObject) //quickfix: only enable feedback if object isnt being deleted
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
        removeInteractableObject = true;
        GetComponent<MeshRenderer>().enabled = false;
        DisableFeedback();
    }
}
