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

    private void Start()
    {
        damage = 10;
        //health = 100;
    }

    private void Update()
    {
        healthBar.value = Health;
        healthText.text = "" + Health;

        if (IsAlive == false)
        {
            GameManager.manager.ChangeState(GameState.LOSE);
            //Health = maxHealth;
        }

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

    protected override void Death()
    {
        base.Death();
        // ENTER CODE FOR DEATH ANIMATIONS, ETC
    }

    public float GetHealthDividedMaxHealth()
    {
        return (health / maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WIN") { GameManager.manager.ChangeState(GameState.WIN); }
    }
}
