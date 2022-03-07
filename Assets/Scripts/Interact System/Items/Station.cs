using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public abstract class Station : ScriptableObject
{
    public virtual void OnTriggerStation(GameObject interactable, GameObject interactor)
    {
        Debug.Log($"{interactor.name} triggered {interactable.name}.");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
