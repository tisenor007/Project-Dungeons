using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Weapon : Equipment
{
    public GameObject weaponObject;
    public int damage;
    public float attackSpeed;

    public void StartSetStats(PlayerStats playerStats)
    {
        // damage
        playerStats.Damage = damage;

        // attack speed
        playerStats.AttackSpeed = attackSpeed;

        // item type
        playerStats.CurrentWeapon = this;
    }

    public override void Equip(GameObject equipment, GameObject interactor, bool inGameEquip)
    {
        base.Equip(equipment, interactor, inGameEquip);

        PlayerStats playerStats = FindPlayerStats(interactor);

        // discard old weapon
        // playerStats.DiscardWeapon();

        // set player stats
        // damage
        playerStats.Damage = damage;

        // attack speed
        playerStats.AttackSpeed = attackSpeed;
        
        //equip 
        playerStats.EquipWeapon(equipment, inGameEquip);

        // item type
        playerStats.CurrentWeapon = this;

        //display equip feedback
        if (inGameEquip == false) { return; }
        GameManager.manager.levelManager.CreatePopUp($"Equipped {this.nameOfItem}:{damage} ATK", interactor.transform.position, Color.white);
        Debug.LogWarning($"Set {playerStats.gameObject.name}, damage = {damage}, speed = {attackSpeed}, type = {playerStats.CurrentWeapon}.");
    }

}