using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : Item
{
    // basic equipment will be equiped on pickup, 
    
    //needs polish to eject old equipment
    public override void OnPickup(GameObject interactable, GameObject interactor)
    {
        GameObject equipment = interactable.transform.GetChild(0).gameObject; // breaking off the interactable shell/GameObject

        base.OnPickup(interactable, interactor);
        Equip(equipment, interactor);
    }

    public virtual void Equip(GameObject equipment, GameObject interactor)
    {
        Debug.LogWarning($"Equiping {nameOfItem} Item");
    }

    public PlayerStats FindPlayerStats(GameObject interactor)
    {
        PlayerStats playerStats = interactor.GetComponent<PlayerStats>();

        return playerStats;
    }
}
