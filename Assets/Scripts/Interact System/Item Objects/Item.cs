using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public virtual void OnPickup(GameObject gameObject)
    {
        Debug.Log($"{this} was picked up.");
    }
}
