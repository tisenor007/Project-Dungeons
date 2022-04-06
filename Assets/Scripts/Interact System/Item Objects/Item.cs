using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Interactable))] // all subclasses should also require interactable
public abstract class Item : ScriptableObject
{
    public virtual void OnPickup(GameObject interactor)
    {
        Debug.Log($"{interactor.name} picked up {this.name}.");
    }
}