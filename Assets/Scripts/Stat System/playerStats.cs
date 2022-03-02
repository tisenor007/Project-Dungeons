using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats 
{
    //initialization
    [SerializeField] private GameObject maleHitArea;
    [SerializeField] private GameObject femaleHitArea;

    //equipment
    private GameObject weaponObject;
    public Collider weaponHitArea;
    public GameObject shield;

    //HUD
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text healthText;
    [Space]
    [SerializeField] private Vector3 respawnPos = new Vector3(-56.0f, 5.11f, -63.0f);

    private void Update()
    {
        UpdateHud();
    }

    #region Stat Modification
    public override void ResetStats()
    {
        base.ResetStats();

        PlayerController playerController = gameObject.GetComponent<PlayerController>();

        // transform.GetChild(0).gameObject.SetActive(true); Removed to prevent issues with character selection, since this is handled there to create the ability for multiple genders
        transform.localPosition = respawnPos;
        transform.parent.localEulerAngles = Vector3.zero;

        playerController.StopAttacking();
        playerController.StopBlocking();
    }

    public override void TakeDamage(int damage, Transform character)
    {
        base.TakeDamage(damage, character);
        DamageFeedback(character, "-" + damage, Color.red);
        if (health <= 0)
        {
            GameManager.manager.levelManager.ChangeGameStateToLose();
            Death();
        }
    }

    protected override void Death()
    {
        base.Death();
        transform.GetChild(0).gameObject.SetActive(false);
        // ENTER CODE FOR DEATH ANIMATIONS, ETC
    }
    #endregion

    #region tools
    public void UpdateHud()
    {
        healthBar.value = Health;
        healthText.text = "" + Health;
    }

    public float GetHealthDividedMaxHealth()
    {
        return (health / maxHealth);
    }

    public void SetGender(bool isMale)
    {
        if (isMale)
        {
            weaponHitArea = maleHitArea.GetComponent<Collider>();
        }
        else
        {
            weaponHitArea = femaleHitArea.GetComponent<Collider>();
        }

        weaponObject = weaponHitArea.transform.parent.parent.gameObject; // parent.parent is to get: hitarea > handpos > weapon root
    }

    public void EquipWeapon(GameObject newWeaponObject)
    {
        //get player hands
        Transform playerHands = weaponObject.transform.parent;
        
        DiscardWeapon();
        //Destroy(weaponObject);

        weaponObject = Instantiate(newWeaponObject, playerHands);

        HitArea hitArea = weaponObject.transform.GetChild(0).GetComponentInChildren<HitArea>();

        hitArea.PlayerStats = this;
        hitArea.PlayerController = gameObject.GetComponent<PlayerController>();
        weaponHitArea = hitArea.gameObject.GetComponent<Collider>();
    }

    public void DiscardWeapon() // should be replaced by unequip if inventory is established
    {
        if (currentWeapon == null) return;

        GameManager.manager.levelManager.CreateInteractable(weaponObject,
            transform.position, true, Color.red, CurrentWeapon);
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WIN") { GameManager.manager.levelManager.ChangeGameStateToWin(); }
        if (other.tag == "DeathBox") { transform.position = respawnPos; }
    }
}
