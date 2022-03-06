using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Interactable))] // all subclasses should also require interactable
[SerializeField]
public abstract class Item : ScriptableObject
{
    public string nameOfItem;
    public Color colorOfItemShine; // rude, make one with Enums to choose from

    public virtual void OnPickup(GameObject interactable, GameObject interactor)
    {
        Debug.Log($"{interactor.name} picked up {interactable.name}, {nameOfItem}.");
    }
}