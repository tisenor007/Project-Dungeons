using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void Update()
    {
        blocking = Input.GetMouseButton(1);

        if (blocking)
        {
            shield.SetActive(true);
        }
        if (blocking == false)
        {
            shield.SetActive(false);

            if (Input.GetMouseButtonDown(0))
            {
                attacking = true;
            }
            if (attacking)
            {
                hitArea.SetActive(true);

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitArea.SetActive(false);
                    hitTimer = 0.5f;
                    attacking = false;
                }

            }
        }
    }

    protected override void Death()
    {
        base.Death();
        // ENTER CODE FOR DEATH ANIMATIONS, ETC
    }

    public float GetHealthDividedMaxHealth()
    {
        return (health / maxHealth);
    }
}
