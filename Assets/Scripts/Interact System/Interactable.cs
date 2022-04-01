using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShineColour
{ 
    NULL,
    HEAL,
    WEAPON,
    FIRE,
    TREASUREorNOTE,
}

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(BoxCollider))]
public class Interactable : MonoBehaviour
{
    // the interactable class allows for easy script to player interaction

    // LevelManager method create interactable allows creation at runtime

    // TYPES
    // possibly requireing insertion of new types below, 
    // item type allows for Items to be picked up by interaction
    // station type allows for menu interaction

    // all types can be added to, for instance, 
    // items could pick up into an inventory and interact furter with an inventory system instead

    public bool isFloatingObject = false;

    private float feedback__Intensity = 3;
    private Light feedbackLight; //feedback to show object is interactable
    private bool feedbackEnabled;
    private bool interactableEnabled = true;
    private float fadeInSpeed = 20f;
    private float fadeOutSpeed = 10f;
    private GameObject interactableObject;
    private float rotationSpeed = 50f;
    private float bounceSpeed = .7f;
    private float bounceRange = .3f;
    private float bounceSavedPos;
    private Vector3 floatingDirection;

    //types of interactables
    [SerializeField] private Item itemType;
    [SerializeField] private Station stationType;
    public bool FeedbackEnabled { get { return feedbackEnabled; } }
    public bool InteractableEnabled { get { return interactableEnabled; } }
    public Item ItemType { set { itemType = value; } }

    void Start()
    {
        interactableObject = this.gameObject.transform.GetChild(0).gameObject;
        feedbackLight = this.gameObject.GetComponent<Light>();

        interactableEnabled = true;
        feedbackEnabled = false;
        feedbackLight.intensity = 0;
        bounceSavedPos = transform.position.y;
        floatingDirection = Vector3.up;

        DisableFeedback();

        if (itemType != null) { SetShineColour(feedbackLight, itemType.ShineColour); }
        else if (stationType != null) { SetShineColour(feedbackLight, stationType.ShineColour); }
        else { RemoveGameObjectWFeedback(); }

        if (isFloatingObject && IsInGround())
        {
            bounceSavedPos += 5;
        }
    }

    void Update()
    {
        UpdateFeedback();
    }

    public void UpdateFeedback()
    {
        if (isFloatingObject)
        {
            this.gameObject.transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime, Space.World);

            if (this.transform.localPosition.y > bounceSavedPos + bounceRange)
            { floatingDirection = Vector3.down; }
            else if (this.transform.localPosition.y < bounceSavedPos - bounceRange)
            { floatingDirection = Vector3.up; }

            FloatObject();
        }

        if (feedbackEnabled == false && feedbackLight.intensity > 0)
        {
            feedbackLight.intensity -= Time.deltaTime * fadeOutSpeed;
        }
        else if (feedbackEnabled == true && feedbackLight.intensity < feedback__Intensity)
        {
            feedbackLight.intensity += Time.deltaTime * fadeInSpeed;
        }
        else if (interactableEnabled == false && feedbackLight.intensity <= 0)
        {
            Destroy(gameObject);
        }
    }

    public bool IsInGround()
    {
        bool inGround = false;
        LayerMask interactableLayer = LayerMask.NameToLayer("Interactable");

        if (Physics.Raycast(gameObject.transform.position + Vector3.up, Vector3.down,
            5f, interactableLayer, QueryTriggerInteraction.Ignore))
        { // detecting if interactable intersecs on a plane
            inGround = true;
        }

        return inGround;
    }

    public void FloatObject()
    {
        if (floatingDirection == Vector3.up)
        {
            gameObject.transform.position
                = new Vector3(transform.position.x,
                transform.position.y + bounceSpeed * Time.deltaTime,
                transform.position.z);
        }
        else if (floatingDirection == Vector3.down)
        {
            gameObject.transform.position
                = new Vector3(transform.position.x,
                transform.position.y - bounceSpeed * Time.deltaTime,
                transform.position.z);
        }
    }

    public void Interact(GameObject interactor)
    {
        if (itemType != null) { itemType.OnPickup(this.gameObject, interactor); }
        else if (stationType != null) { stationType.OnTriggerStation(this.gameObject, interactor); }
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
        //interactableObject.GetComponent<MeshRenderer>().enabled = false;
        //interactableObject.GetComponent<Collider>().enabled = false;
        DisableFeedback();
    }

    public void SetShineColour(Light light, ShineColour setColour)
    {
        switch (setColour)
        {
            case ShineColour.NULL: {
                    light.color = Color.white;
                    return;}

            case ShineColour.HEAL: {
                    light.color = Color.green;
                    return;}

            case ShineColour.WEAPON: {
                    light.color = new Color32(145, 56, 49, 255);
                    return;}

            case ShineColour.FIRE: {
                    light.color = new Color32(255, 165, 0, 255); // orange
                    return;}

            case ShineColour.TREASUREorNOTE: {
                    light.color = new Color32(255, 223, 0, 255); // "gold yellow"
                    return;}
        }
    }
}