using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class Weapon : Equipment
{
    public int damage;
    public float attackSpeed;

    public override void Equip(GameObject equipment, GameObject interactor, bool inGameEquip)
    {
        LevelManager lM = GameManager.manager.levelManager;

        base.Equip(equipment, interactor, inGameEquip);

        PlayerStats playerStats = FindPlayerStats(interactor);

        //equip weapon
        playerStats.EquipWeapon(this, inGameEquip);

        if (inGameEquip == false) { return; }
        //display equip feedback ?

        lM.CreatePopUp($"<=Equiped=-", interactor.transform.position, Color.white);
        DisplayFeedback(lM);
    }

    public void DisplayFeedback(LevelManager lM)
    { 
        UIManager uM = GameManager.manager.uiManager;

        //uM.weaponInfo.transform.GetChild(0).GetComponentInChildren<Image>().sprite = this.sprite;
        uM.weaponInfo.transform.GetChild(0).GetComponentInChildren<Text>().text = $"{nameOfItem}:\nDMG = {damage}\nSPD = {attackSpeed}";
        
        lM.WeaponDisplayTimer = 3;
        lM.JumpCanvasAlphaTo(10, uM.weaponInfo);
    }
}
