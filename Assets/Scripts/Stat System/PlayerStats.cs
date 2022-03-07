using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    //equiment
    public Collider weaponHitArea;
    public GameObject shield;
    private GameObject weaponObject;

    //initialization
    [SerializeField] private GameObject maleHand;
    [SerializeField] private GameObject femaleHand;

    //HUD
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text healthText;
    [Space]
    private GameObject hitArea;
    private bool healing;
    [Space]
    [SerializeField] private Vector3 respawnPos = new Vector3(-56.0f, 5.11f, -63.0f);

    private float restStationHealDuration = 3;
    private float restStationHealAmount = 10;
    private float restStationHealTimer = 0;
    private GameObject activePlayerHand;

    public Vector3 RespawnPos { get { return respawnPos; } set { respawnPos = value; } }
    public GameObject WeaponObject { get { return weaponObject; } }

    private void Start()
    {
        currentWeapon.StartSetStats(this);
    }

    private void Update()
    {
        healthBar.value = Health;
        healthText.text = "" + Health;

        if (healing) { HealAtRestStation(); }
    }

    public void Attack() 
    { 
        if (shield.activeSelf == true) { return; }
        if (hitArea.activeSelf == true) { return; }
        hitArea.SetActive(true); 
    }

    public void StopAttacking() 
    {
        if (hitArea == null) { return; }
        if (hitArea.activeSelf == false) { return; }
        hitArea.SetActive(false);
    }

    public void Block() 
    { 
        if (shield.activeSelf == true) { return; }
        shield.SetActive(true);
    }

    public void StopBlocking() 
    { 
        if (shield.activeSelf == false) { return; }
        shield.SetActive(false);
    }

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
        healing = false;
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
    public void EquipWeapon(GameObject newWeaponObj, bool discardWeapon)
    {
        //get player hands
        Transform playerHands = weaponObject.transform.parent;

        if (discardWeapon)
        {
            DiscardWeapon();
        }
        else 
        {
            RemoveWeapon();
        }

        weaponObject = Instantiate(newWeaponObj, playerHands);

        if (!GameManager.manager.characterSelection.isMale) 
        {
            weaponObject.transform.localScale = new Vector3
            (15 / GameManager.manager.playerAndCamera.transform.GetChild(0).transform.GetChild(0).localScale.x,
            15 / GameManager.manager.playerAndCamera.transform.GetChild(0).transform.GetChild(0).localScale.y,
            15 / GameManager.manager.playerAndCamera.transform.GetChild(0).transform.GetChild(0).localScale.z);
        }
        else if (GameManager.manager.characterSelection.isMale)
        {
            weaponObject.transform.localScale = new Vector3
            (1 / GameManager.manager.playerAndCamera.transform.GetChild(0).transform.GetChild(0).localScale.x,
            1 / GameManager.manager.playerAndCamera.transform.GetChild(0).transform.GetChild(0).localScale.y,
            1 / GameManager.manager.playerAndCamera.transform.GetChild(0).transform.GetChild(0).localScale.z);
        }

        HitArea hitArea = weaponObject.transform.GetChild(0).GetComponentInChildren<HitArea>();

        hitArea.PlayerStats = this;
        hitArea.PlayerController = gameObject.GetComponent<PlayerController>();
        weaponHitArea = hitArea.gameObject.GetComponent<Collider>();

        weaponObject.transform.parent = activePlayerHand.transform;
    }

    private void RemoveWeapon()
    {
        if (currentWeapon == null) return;

        Destroy(weaponObject);
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

    public void UpdateWeaponHitArea(bool isMale)
    {  
        if (isMale)
        {
            activePlayerHand = maleHand;
            maleHand.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collider>().enabled = true;
            femaleHand.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collider>().enabled = false;
        }
        else if (!isMale)
        {
            activePlayerHand = femaleHand;
            maleHand.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collider>().enabled = false;
            femaleHand.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collider>().enabled = true;
        }

        if (activePlayerHand == null) { return; }
        weaponHitArea = activePlayerHand.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collider>();

        if (weaponHitArea == null) { return; }
        weaponObject = weaponHitArea.transform.parent.parent.gameObject; // parent.parent is to get: hitarea > handpos > weapon root
    }

    public void EquipDefaultWeapon(bool inGamePickup)
    {
        DefaultWeapon.Equip(DefaultWeapon.weaponObject, GameManager.manager.playerAndCamera.transform.GetChild(0).gameObject, inGamePickup);
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

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "RestStation") { healing = true; }
        if (other.tag == "DeathBox") { transform.position = respawnPos; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RestStation") { healing = false; }
    }
}
