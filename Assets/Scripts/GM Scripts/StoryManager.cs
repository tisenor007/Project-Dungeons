using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    //err check last message being read

    public NoteItem[] storyNotes;
    [Tooltip("Read randomly after Story Notes are exhausted.")] public NoteItem[] fillerNotes;

    //progression
    [SerializeField] private NoteItem currentNote;
    private int currentProgress;
    private int fillerValue;
    [SerializeField] private bool storyFinished;

    private void Start()
    {
        SetupNoteProgression();
    }

    //private methods
    private void SetupNoteProgression()
    {
        if (storyNotes == null) { Debug.LogWarning("NO STORY LOADED"); return; }

        currentProgress = 0;
        currentNote = storyNotes[currentProgress];
    }

    private void ApplyStoryNote(Interactable note)
    {
        if (currentNote == null) { Debug.LogWarning($"NO MESSAGE IN CURRENT NOTE"); return; }

        note.ItemType = currentNote;
    }

    private void IncreaseProgression()
    {
        if (currentProgress >= storyNotes.Length - 1) { storyFinished = true; return; }

        currentProgress++;
        currentNote = storyNotes[currentProgress];

    }

    private void PickRandomNoteValueWithinRange()
    {
        int valueToChangeInto = (int)Random.Range(0, fillerNotes.Length-1); //-1 #IE: .Length starts counting at 1 however it's going to be read into an array which starts at 0 ;)

        if (valueToChangeInto == fillerValue || fillerNotes[valueToChangeInto] == currentNote )
        { PickRandomNoteValueWithinRange(); }
        else
        { fillerValue = valueToChangeInto; }
    }

    private void ApplyRandomNote(Interactable note)
    {
        if (fillerNotes == null) { Debug.LogError("NO FILLER NOTES FOR AFTER STORY ENDS"); }

        // choose a random SO note to inject
        PickRandomNoteValueWithinRange();

        note.ItemType = fillerNotes[fillerValue];
    }

    //public methods
    public void ProgressStory(Interactable note) // should be used in a level manager method inside a note method to call story progression if the note bottle is set to be a StoryNote
    {

        if (!storyFinished)
        {
            ApplyStoryNote(note);
        }
        else
        { 
            ApplyRandomNote(note);
        }

        IncreaseProgression();
    }
}
