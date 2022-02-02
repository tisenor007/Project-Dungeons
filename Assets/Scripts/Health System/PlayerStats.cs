using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : GameCharacter 
{
    public bool attacking = false;
    public bool blocking;

    public float hitTimer = 0.5f;

    public GameObject shield;
    public GameObject hitArea;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text healthText;
    private Vector3 respawnPos = new Vector3(-56.0f, 5.11f, -63.0f);
    private void Awake()
    {
        
    }

    private void Update()
    {
        healthBar.value = Health;
        healthText.text = "" + Health;

        //blocking = Input.GetMouseButton(1);

        //if (blocking)
        //{
        //    shield.SetActive(true);
        //}
        //if (blocking == false)
        //{
        //    shield.SetActive(false);

        //    if (Input.GetMouseButtonDown(0) && attacking == false)
        //    {
        //        attacking = true;
        //    }
        //    if (attacking)
        //    {
        //        hitArea.SetActive(true);

        //        hitTimer -= Time.deltaTime;

        //        if (hitTimer < 0)
        //        {
        //            hitArea.SetActive(false);
        //            hitTimer = attackDuration;
        //            attacking = false;
        //        }

        //    }
        //}
    }
    public void Attack() { if (shield.activeSelf == false && hitArea.activeSelf == false) { hitArea.SetActive(true); } }
    public void StopAttacking() { if (hitArea.activeSelf == true) hitArea.SetActive(false);}

    public void Block() { if (shield.activeSelf == false) { shield.SetActive(true); } blocking = true; }
    public void StopBlocking() { if (shield.activeSelf == true) { shield.SetActive(false); blocking = false; } }

    public float GetHealthDividedMaxHealth()
    {
        return (health / maxHealth);
    }

    public override void ResetStats()
    {
        base.ResetStats();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.localPosition = respawnPos;
        transform.parent.localEulerAngles = Vector3.zero;
    }

    public override void TakeDamage(int damage, Transform character)
    {
        base.TakeDamage(damage, character);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WIN") { GameManager.manager.levelManager.ChangeGameStateToWin(); }
    }
}
