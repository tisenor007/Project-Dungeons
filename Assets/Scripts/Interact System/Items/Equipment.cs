using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public abstract class Equipment : Item
{
    GameObject equipmentObject;
    // basic equip type method
    // will be equiped on pickup, 
    // should eject overidden equipment!

    public override void OnPickup(GameObject interactable, GameObject interactor)
    {
        base.OnPickup(interactable, interactor);

        interactable.GetComponent<Interactable>().RemoveGameObjectWFeedback();
        GameObject equipment = interactable.transform.GetChild(0).gameObject; // breaking off the interactable shell/GameObject

        Equip(equipment, interactor);

    }

    public virtual void Equip(GameObject equipment, GameObject interactor)
    {
        FindEquipmentObject(equipment);
        Debug.Log($"Equiping {nameOfItem}");
    }

    public virtual void FindEquipmentObject(GameObject equipmentGameObject)
    {
        equipmentObject = equipmentGameObject;
    }

    public PlayerStats FindPlayerStats(GameObject interactor)
    {
        PlayerStats playerStats = interactor.GetComponent<PlayerStats>();

        return playerStats;
    }
}
