using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public NoteItem[] notes;

    [SerializeField] private static NoteItem currentNoteProgression;

    private void Start()
    {
        SetupNoteProgression();
    }

    private void SetupNoteProgression()
    {
        if (notes == null) return;

        currentNoteProgression = notes[0];
    }

    private void ApplyNote()
    { 
        //applies note to bottle
    }

    private void IncreaseProgression()
    { 
        //increase current progression by 1
    }

    private void ApplyRandomNote()
    { 
        // choose a random SO note to inject
    }

    public void ProgressStory() // should be used in a level manager method inside a note method to call story progression if the note bottle is set to be a StoryNote
    {
        //apply note to current Bottle
        ApplyNote();
        //increase current progression

        //if all story notes have been applied // apply random note
    }
}
