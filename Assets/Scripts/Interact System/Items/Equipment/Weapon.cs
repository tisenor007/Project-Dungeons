using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Weapon : Equipment
{
    public int damage;
    public int attackSpeed;

    public override void Equip(GameObject equipment, GameObject interactor)
    {
        base.Equip(equipment, interactor);

        PlayerStats playerStats = FindPlayerStats(interactor);

        // discard old weapon
        // playerStats.DiscardWeapon();

        // set player stats
        // damage
        playerStats.Damage = damage;
        Debug.LogWarning($"Setting {playerStats.gameObject.name} damage to {damage}.");

        // attack speed
        playerStats.AttackSpeed = attackSpeed;
        Debug.LogWarning($"Setting {playerStats.gameObject.name} attack speed to {attackSpeed}");
        
        // item type
        playerStats.CurrentWeapon = this;
        Debug.LogWarning($"Setting Item Type");

        //equip 
        playerStats.EquipWeapon(equipment);


    }

}
