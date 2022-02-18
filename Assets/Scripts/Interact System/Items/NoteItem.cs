using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Interactable))] 
[CreateAssetMenu]
public class NoteItem : Item
{

    [SerializeField] [TextArea(30,1)] private string message;

    public override void OnPickup(GameObject interactable, GameObject interactor)
    {
        base.OnPickup(interactable, interactor);

        interactor.GetComponent<PlayerController>().CanMove = false;
        GameManager.manager.levelManager.DisplayPlainNote(message);
    }
}