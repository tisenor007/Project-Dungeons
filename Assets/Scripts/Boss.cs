using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    private float stamina = 10;
    private float recoverTime = 6;
    private float staminaTimer;
    private float recoverTimer;
    // Start is called before the first frame update
  
    void Start()
    {
        this.viewDistance = 15;
        this.health = maxHealth; 
        this.attackSpeed = 4f;
        this.damage = 30;
        this.speed = 5.0f;
        this.attackDistance = 5;

        this.attackSound = SoundManager.Sound.ZombieAttack;
        this.chasingSound = SoundManager.Sound.ZombieChasing;
        this.deathSound = SoundManager.Sound.ZombieDeath;
        this.idleSound = SoundManager.Sound.ZombieIdle;

        InitEnemy();
    }

    public override void Idle()
    {
        staminaTimer = Time.time + stamina;
        if (Time.time >= recoverTimer)
        {
            SwitchState(State.Chasing);
        }
    }

    public override void Chasing()
    {
        recoverTimer = Time.time + recoverTime;
        enemyNavMeshAgent.SetDestination(playerLocation);

        if (distanceFromPlayer <= attackDistance)
        {
            hitTimer = attackSpeed;
            SwitchState(State.Attacking);
        }

        if (distanceFromPlayer >= viewDistance)
        {
            SwitchState(State.Chasing);
        }

        if (Time.time >= staminaTimer)
        {
            staminaTimer = Time.time + stamina;
            SwitchState(State.Idle);
        }
    }


    public override void TakeDamage(int damage, Transform character)
    {
        base.TakeDamage(damage, character);
    }
}
