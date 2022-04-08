using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Interactable))] 
[CreateAssetMenu]
public class NoteItem : Item
{
    [SerializeField] [Tooltip("IF TRUE DOES NOT LOAD THIS MESSAGE,\n" +
                              "loads the progressive story\n" +
                              "Found in GameManager>StoryManager")] public bool loadStory;

    [SerializeField] [TextArea(30,1)] private string message;

    public string Message { get { return message; } set { message = value; } }

    public override void OnPickup(GameObject interactable, GameObject interactor)
    {
        base.OnPickup(interactable, interactor);

        interactor.GetComponent<PlayerController>().CanMove = false;
        SoundManager.PlaySound(SoundManager.Sound.Cork);

        if (!loadStory)
        {
            GameManager.manager.levelManager.DisplayPlainNote(message);
        }
        else if (loadStory)
        {
            GameManager.manager.levelManager.Story.ProgressStory(interactable.GetComponent<Interactable>()); // pass through interactable and change note
            GameManager.manager.levelManager.DisplayPlainNote(message);
        }
    }

}