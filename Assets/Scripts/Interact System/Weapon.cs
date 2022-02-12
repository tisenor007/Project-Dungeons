using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : Item
{
    public override void OnPickup(GameObject interactable, GameObject interactor)
    {
        base.OnPickup(interactable, interactor);
        Equip(interactor);
    }

    public void Equip(GameObject interactor)
    {
        //apply equipment stats to interactor?
        Debug.LogWarning("Stats are not set up to Equip");
    }
}
