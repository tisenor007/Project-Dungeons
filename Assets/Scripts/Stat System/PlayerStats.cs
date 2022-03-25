using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    //equiment
    public GameObject weaponObject;
    public GameObject shield;
    public Collider weaponHitArea;

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

    private int damageValue = 0;
    private int savedDamageValue;
    private int[] damageThresholds = { 75, 50, 25, 15 }; // limits at which GUI reacts

    public Vector3 RespawnPos { get { return respawnPos; } set { respawnPos = value; } }
    public GameObject WeaponObject { get { return weaponObject; } }

    private void Start()
    {
        //set up weapon
        if (currentWeaponType != null) currentWeaponType = defaultWeapon;

        damage = currentWeaponType.damage;
        attackSpeed = currentWeaponType.attackSpeed;
    }

    private void Update()
    {
        UpdateHud();

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
    public void EquipWeapon(Weapon weaponType, bool discardWeapon)
    {
        //gets
        Transform playerHand = weaponObject.transform.parent;
        HitArea hitArea = weaponObject.transform.GetChild(0).GetComponentInChildren<HitArea>();

        //drop an interteractable?
        if (discardWeapon)
        {
            DiscardWeapon();
        }
        else 
        {
            RemoveWeapon();
        }

        //create Weapon in Players hand
        weaponObject = Instantiate(weaponType.prefab, playerHand);

        //resize weapon accronding to size and magic numbers?
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

        //set player hit area
        hitArea.PlayerStats = this;
        hitArea.PlayerController = gameObject.GetComponent<PlayerController>();
        weaponHitArea = hitArea.gameObject.GetComponent<Collider>();

        // set player stats
        // damage
        damage = weaponType.damage;

        // attack speed
        attackSpeed = weaponType.attackSpeed;

        // item type
        currentWeaponType = weaponType;


        Debug.Log($"Set {gameObject.name}, damage = {damage}, speed = {attackSpeed}, type = {currentWeaponType}.");
    }

    private void RemoveWeapon()
    {
        if (currentWeaponType == null) return;

        Destroy(weaponObject);
    }

    public void DiscardWeapon() // should be replaced by unequip if inventory is established
    {
        if (currentWeaponType == null) return;

        GameManager.manager.levelManager.CreateInteractable(weaponObject,
            transform.position, true, Color.red, CurrentWeaponType);
    }
    #endregion

    #region tools
    public void UpdateHud()
    {
        //gets
        LevelManager lM = GameManager.manager.levelManager;
        UIManager uM = GameManager.manager.uiManager;

        //update health
        healthBar.value = health;
        healthText.text = "" + health;

        //meta feedback
        if (damageValue == savedDamageValue) return;

        //calc
        if (health > damageThresholds[0]) damageValue = 0;
        else if (health < damageThresholds[1]) damageValue = 25;
        else if (health < damageThresholds[2]) damageValue = 50;
        else if (health < damageThresholds[3]) damageValue = 75;
        else if (health < damageThresholds[4]) damageValue = 100;

        lM.JumpCanvasAlphaTo(damageValue, uM.playerBlood);
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
        }
        else if (!isMale)
        {
            activePlayerHand = femaleHand;
        }

        if (activePlayerHand == null) { return; }
        weaponHitArea = activePlayerHand.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collider>();

        if (weaponHitArea == null) { return; }
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
