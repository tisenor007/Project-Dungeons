using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : GameCharacter 
{
    public GameObject shield;
    [HideInInspector]public Vector3 respawnPos = Vector3.zero;
    [SerializeField] private GameObject maleHitArea;
    [SerializeField] private GameObject femaleHitArea;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text healthText;
    [Space]
    private GameObject hitArea;
    private bool healing;

    private float restStationHealDuration = 10;
    private float restStationHealAmount = 5;
    private float restStationHealTimer = 0;

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

    public float GetHealthDividedMaxHealth()
    {
        return (health / maxHealth);
    }

    public void SetGender(bool isMale)
    {
        if (isMale)
        {
            hitArea = maleHitArea;
        } else
        {
            hitArea = femaleHitArea;
        }
    }

    public override void ResetStats()
    {
        base.ResetStats();
        // transform.GetChild(0).gameObject.SetActive(true); Removed to prevent issues with character selection, since this is handled there to create the ability for multiple genders
        respawnPos = new Vector3(0, 2, 0);
        this.transform.position = respawnPos;
        transform.parent.localEulerAngles = Vector3.zero;
        StopAttacking();
        StopBlocking();
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
