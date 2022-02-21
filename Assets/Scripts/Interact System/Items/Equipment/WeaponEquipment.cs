using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : Equipment
{
    public int damage;
    public int attackSpeed;


    public override void Equip(GameObject equipment, GameObject interactor)
    {
        base.Equip(equipment, interactor);

        HitArea hitArea = FindHitArea(equipment);

        PlayerStats playerStats = FindPlayerStats(interactor);

        // set player stats
        playerStats.Damage = damage;
        Debug.LogWarning($"Setting {interactor.name} damage to {damage}.");

        // set attack speed
        playerStats.AttackSpeed = attackSpeed;
        Debug.LogWarning($"Setting {interactor.name} attack speed to {attackSpeed}");

        /// make gameobject trigger


        /// set hitarea as current playerstats hit area and vice versa
        

    }

    public HitArea FindHitArea(GameObject equipment)
    {
        HitArea hitArea = equipment.GetComponent<HitArea>();

        return hitArea;
    }
}
