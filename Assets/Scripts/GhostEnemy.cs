using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostEnemy : Enemy
{
    public float rotationSpeed;

    public float bobUp;
    public float bobDown;
    public float bobSpeed = 0.5f; // units per seconds

    // rotation speeds
    public float noramlSpeedSpin = 10.0f; // degrees per seconds
    public float fastSpeedSpin = 100.0f; // degrees per seconds

    public bool movingDown = false;

    public Transform ghostBody;
    void Start()
    {
        enemyNavMeshAgent = this.GetComponent<NavMeshAgent>();
        ghostBody = transform.GetChild(1);

        this.viewDistance = 10;
        maxHealth = 25; 
        this.health = maxHealth;
        this.attackSpeed = 1.0f;
        this.damage = 5;
        this.speed = 6.0f;
        this.rotationSpeed = 10f;
        this.attackDistance = 2;
        bobUp = transform.position.y + 4;
        bobDown = transform.position.y + 2;
        enemyNavMeshAgent.speed = speed;
        this.enemyType = EnemyType.Ghost;

        this.attackSound = SoundManager.Sound.GhostAttack;
        this.chasingSound = SoundManager.Sound.GhostChasing;
        this.deathSound = SoundManager.Sound.GhostDeath;
        this.idleSound = SoundManager.Sound.GhostIdle;

        InitEnemy();
    }

    void Rotate() // || -90 > -0 > 0 > 90 > 180 > -180 > -90 ||
    {
        // set rotation speed 
        if (transform.localEulerAngles.y > 0 && transform.localEulerAngles.y < 45) // 0..45 degrees (45 degrees total)
        {
            //Debug.Log("45");
            rotationSpeed = noramlSpeedSpin;
        }
        else if (transform.localEulerAngles.y > 180 && transform.localEulerAngles.y < 225) // 180..225 degrees (45 degrees total)
        {
            //Debug.Log("180");
            rotationSpeed = fastSpeedSpin;
        }
        else
        {
            rotationSpeed = 100f;
        }

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }

    void Bobbing()
    {
        if (movingDown == false)
        {
            ghostBody.transform.Translate(Vector3.up * Time.deltaTime * bobSpeed, Space.World);


            if (ghostBody.transform.position.y > bobUp)
            {
                movingDown = true;

            }
        }
        else
        {
            ghostBody.transform.Translate(Vector3.down * Time.deltaTime * bobSpeed, Space.World);

            if (ghostBody.transform.position.y < bobDown)
            {
                movingDown = false;

            }
        }

    }

    public override void Idle()
    {
        Bobbing();
        playerLocation = GameManager.manager.playerStats.gameObject.transform.position;
        enemyNavMeshAgent.speed = speed;
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        Rotate();

        if (distanceFromPlayer <= viewDistance)
        {
            SwitchState(State.Chasing);
        }
    }
    public override void Chasing()
    {
        enemyNavMeshAgent.speed = 10;

        enemyNavMeshAgent.SetDestination(playerLocation);

        if (distanceFromPlayer <= attackDistance)
        {
            enemyNavMeshAgent.speed = speed;
            attackTimer = attackSpeed;
            SwitchState(State.Attacking);
        }

        if (distanceFromPlayer >= viewDistance)
        {
            enemyNavMeshAgent.ResetPath();
            enemyNavMeshAgent.speed = speed;
            SwitchState(State.Idle);
        }
    }
}

    
