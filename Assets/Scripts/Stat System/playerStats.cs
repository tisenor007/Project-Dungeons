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
    private GameObject hitArea;
    private bool healing;
    [Space]
    [SerializeField] private Vector3 respawnPos = new Vector3(-56.0f, 5.11f, -63.0f);

    private float restStationHealDuration = 10;
    private float restStationHealAmount = 5;
    private float restStationHealTimer = 0;

    public Vector3 RespawnPos { get { return respawnPos; } set { respawnPos = value; } }

    private void Update()
    {
        healthBar.value = Health;
        healthText.text = "" + Health;

        if (healing) { HealAtRestStation(); }
    }

    public void Attack() { if (shield.activeSelf == false && hitArea.activeSelf == false) { hitArea.SetActive(true); } }

    public void StopAttacking() 
    {
        if (hitArea == null) { return; }
        if (hitArea.activeSelf == false) { return; }
        hitArea.SetActive(false);
    }

    public void Block() { if (shield.activeSelf == false) { shield.SetActive(true); }}

    public void StopBlocking() { if (shield.activeSelf == true) { shield.SetActive(false);}}

    #region Stat Modification
    public override void ResetStats()
    {
        base.ResetStats();

        PlayerController playerController = gameObject.GetComponent<PlayerController>();

        // transform.GetChild(0).gameObject.SetActive(true); Removed to prevent issues with character selection, since this is handled there to create the ability for multiple genders
        respawnPos = new Vector3(0, 2, 0);
        this.transform.position = respawnPos;
        transform.parent.localEulerAngles = Vector3.zero;

        playerController.StopAttacking();
        playerController.StopBlocking();
    }

    public override void TakeDamage(int damage, Transform character)
    {
        base.TakeDamage(damage, character);
        DamageFeedback(character, "-" + damage, Color.red);
        GameManager.manager.levelManager.FlashPlayerBleedingUI();
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

    #region Equip Modification
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
    #endregion

    private void HealAtRestStation()
    {
        if (Time.time > restStationHealTimer)
        {
            Heal((int)restStationHealAmount);
            restStationHealTimer = Time.time + restStationHealDuration;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RestStation") { healing = true; }
        if (other.tag == "WIN") { GameManager.manager.levelManager.ChangeGameStateToWin(); }
        if (other.tag == "DeathBox") { transform.position = respawnPos; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RestStation") { healing = false; }
    }
}
