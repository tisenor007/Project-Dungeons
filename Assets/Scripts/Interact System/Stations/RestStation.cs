using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))] // all subclasses should also require interactable
[CreateAssetMenu]
public class RestStation : Station
{
    public override void OnTriggerStation(GameObject interactable, GameObject interactee)
    {
        base.OnTriggerStation(interactable, interactee);

        //interactable.GetComponent<Interactable>().RemoveGameObjectWFeedback();
        GameManager.manager.levelManager.ChangeGameStateToSaveOption();
    }
}
