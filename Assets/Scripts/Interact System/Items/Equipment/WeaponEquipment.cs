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
        playerStats.damage = damage;

        // set attack speed

        /// make gameobject trigger
        
        
        /// set hitarea as current playerstats hit area and vice versa
    }

    public HitArea FindHitArea(GameObject equipment)
    {
        HitArea hitArea = equipment.GetComponent<HitArea>();

        return hitArea;
    }
}
