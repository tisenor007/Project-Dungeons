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
        this.attackDistance = 3;

        this.enemyType = EnemyType.Boss;
        this.attackSound = SoundManager.Sound.BossAttack;
        this.chasingSound = SoundManager.Sound.BossChasing;
        this.deathSound = SoundManager.Sound.BossDeath;
        this.idleSound = SoundManager.Sound.BossIdleRest;

        InitEnemy();
    }

    public override void Idle()
    {
        staminaTimer = Time.time + stamina;
        if (Time.time >= recoverTimer && distanceFromPlayer <= attackDistance)
        {
            attackTimer = attackSpeed;
            SwitchState(State.Attacking);
        }
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
            attackTimer = attackSpeed;
            SwitchState(State.Attacking);
        }

        if (distanceFromPlayer >= attackDistance)
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
