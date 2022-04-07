using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Interactable))] // all subclasses should also require interactable
[SerializeField]
public abstract class Item : ScriptableObject
{
    public string nameOfItem;
    public GameObject prefab;
    public Sprite sprite;

    [SerializeField]
    private ShineColour shineColour; 

    public ShineColour ShineColour { get { return shineColour; } }

    public virtual void OnPickup(GameObject interactable, GameObject interactor)
    {
        Debug.Log($"{interactor.name} picked up {interactable.name}, {nameOfItem}.");
    }
}