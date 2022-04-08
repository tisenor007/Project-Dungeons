using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEnemy : Enemy
{
    private float turnAroundAngle;
    

    void Start()
    {
        this.viewDistance = 10;
        maxHealth = 75;
        this.health = maxHealth;
        this.attackSpeed = 3.0f;
        this.damage = 25;
        this.speed = 3.5f;
        this.attackDistance = 4.0f;
        this.enemyType = EnemyType.Skeleton;

        this.attackSound = SoundManager.Sound.ZombieAttack;
        this.chasingSound = SoundManager.Sound.ZombieChasing;
        this.deathSound = SoundManager.Sound.ZombieDeath;
        this.idleSound = SoundManager.Sound.ZombieIdle;

        turnAroundAngle = transform.localEulerAngles.y;

        

        InitEnemy();
    }

    public override void Idle()
    {
        //PlayAudio(this);

        if (transform.localEulerAngles.y < turnAroundAngle || transform.localEulerAngles.y > turnAroundAngle + 5)
        {
            transform.Rotate(Vector3.up * (speed * 2) * Time.deltaTime, Space.Self);
        }

        viewDistance = 10;
        playerLocation = GameManager.manager.playerStats.gameObject.transform.position;
        enemyNavMeshAgent.speed = speed;

        if (distanceFromPlayer <= viewDistance)
        {
            SwitchState(State.Chasing);
        }
    }

    public override void Chasing()
    {
        //animator.SetBool("Chasing", true);

        viewDistance = 20;
        enemyNavMeshAgent.SetDestination(playerLocation);

        if (distanceFromPlayer <= attackDistance)
        {
            attackTimer = attackSpeed;
            enemyNavMeshAgent.speed = speed;
            SwitchState(State.Attacking);
        }

        if (distanceFromPlayer > viewDistance)
        {
            enemyNavMeshAgent.speed = speed;
            turnAroundAngle = transform.localEulerAngles.y + 180;
            enemyNavMeshAgent.ResetPath();
            SwitchState(State.Idle);
        }
    }
}
