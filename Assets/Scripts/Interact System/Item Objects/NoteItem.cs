using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu]
public class NoteItem : Item
{

    [SerializeField] private string message;

    public override void OnPickup(GameObject interactable, GameObject interactor)
    {
        base.OnPickup(interactable, interactor);

        interactor.GetComponent<PlayerController>().CanMove = false;
        GameManager.manager.levelManager.DisplayPlainNote(message);
    }
}