using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System;

<<<<<<< HEAD:Assets/Scripts/Enemy.cs
public class Enemy : GameCharacter
=======
public class EnemyAI : CharacterStats
>>>>>>> Development-Shrimps:Assets/Scripts/Char Controllers/EnemyAI.cs
{
    protected enum State
    {
        Idle,
        Chasing,
        Attacking
    }
<<<<<<< HEAD:Assets/Scripts/Enemy.cs
=======
    private State enemyState;
    [SerializeField] private NavMeshAgent enemy;
    private PlayerStats playerStats;
    [SerializeField] private int viewDistance = 20;
    [SerializeField] private int hearingDistance = 20;
    [SerializeField] private int attackDistance = 3;
    private float distance;
    private float chasingTime = 4.0f;
    [Tooltip("The time added till next attack, when stunned.")][SerializeField] private float stunnedHitDurationAddition = 3.0f;
    private float hitTime = 3.0f; // Time until next attack
    private Vector3 playerLocation;
    private Vector3 enemyLocation;
    private Ray enemySight;
    private RaycastHit hitInfo;
    [SerializeField] private bool hitable;
    private Image healthColour;
    private Slider healthBar;
    private Transform cam;

    void Start()
    {
        health = 30;
        damage = 5;
        enemy = GetComponent<NavMeshAgent>();
        SwitchState(State.Idle);
>>>>>>> Development-Shrimps:Assets/Scripts/Char Controllers/EnemyAI.cs

    protected State enemyState;
    protected NavMeshAgent enemyNavMeshAgent;
    protected PlayerStats playerStats;

    protected float viewDistance;
    protected float hearingDistance;
    protected float attackDistance;
    protected float speed;

    protected float distanceFromPlayer;
    protected float hitTimer;
    protected float hitDuration;
    protected float stunnedHitDuration;

    protected Vector3 playerLocation;
    protected Vector3 enemyLocation;
    protected Ray enemySight;
    protected RaycastHit hitInfo;

    [SerializeField] private Image healthColour;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Transform cam;

    public void Update()
    {
        UpdateHealth();
        transform.GetChild(0).transform.LookAt(transform.GetChild(0).transform.position + cam.forward);

        enemySight = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        enemyLocation = this.enemyNavMeshAgent.transform.position;
        playerLocation = playerStats.gameObject.transform.position;
        distanceFromPlayer = Vector3.Distance(playerLocation, enemyLocation);

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDistance, Color.white);

        switch (enemyState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Chasing:
                Chasing();
                break;

            case State.Attacking:
                Attacking();
                break;
        }

        //Debug.Log(enemyState);
    }

    public virtual void Idle()
    {
        playerLocation = playerStats.gameObject.transform.position;

        if (Physics.Raycast(enemySight, out hitInfo, viewDistance))
        {
            if (hitInfo.collider.tag == "Player")
            {
                SwitchState(State.Chasing);
            }
        }

        //Debug.Log("enemy ai running");
    }

    public virtual void Chasing()
    {
        enemyNavMeshAgent.SetDestination(playerLocation);

        if (distanceFromPlayer <= attackDistance)
        {
<<<<<<< HEAD:Assets/Scripts/Enemy.cs
            hitTimer = hitDuration;
=======
            hitTime = attackSpeed;
>>>>>>> Development-Shrimps:Assets/Scripts/Char Controllers/EnemyAI.cs
            SwitchState(State.Attacking);
        }

        if (distanceFromPlayer >= viewDistance)
        {
            SwitchState(State.Idle);
        }
    }

    void Attacking()
    {
        enemyNavMeshAgent.SetDestination(enemyLocation);

        hitTimer -= Time.deltaTime;

        if (hitTimer <= 0.0f)
        {
            if (playerStats.shield.activeSelf == true)
            {
                playerStats.TakeDamage((int)(damage / 4), playerStats.GetComponent<Transform>());
<<<<<<< HEAD:Assets/Scripts/Enemy.cs
                hitTimer = stunnedHitDuration; // <----- will be replaced by a possible stunned state
=======
                hitTime = stunnedHitDurationAddition; // <----- will be replaced by a possible stunned state
>>>>>>> Development-Shrimps:Assets/Scripts/Char Controllers/EnemyAI.cs
            }
            else
            {
                playerStats.TakeDamage(damage, playerStats.GetComponent<Transform>());
<<<<<<< HEAD:Assets/Scripts/Enemy.cs
                hitTimer = hitDuration;
=======
                hitTime = attackSpeed;
>>>>>>> Development-Shrimps:Assets/Scripts/Char Controllers/EnemyAI.cs
            }
        }   

        if (distanceFromPlayer > attackDistance)
        {
            SwitchState(State.Chasing);
        }
    }

    protected void SwitchState(State newState)
    {
        enemyState = newState;
    }

    public void UpdateHealth()
    {
        healthBar.value = Health;
        healthBar.maxValue = maxHealth;

        if (Health < maxHealth * 0.8 && Health > maxHealth * 0.6)
            healthColour.color = new Color32(167, 227, 16, 255);

        if (Health < maxHealth * 0.6 && Health > maxHealth * 0.4)
            healthColour.color = new Color32(227, 176, 9, 255);

        if (Health < maxHealth * 0.4 && Health > maxHealth * 0.2)
            healthColour.color = new Color32(240, 86, 48, 255);

        if (Health < maxHealth * 0.2)
            healthColour.color = new Color32(204, 40, 0, 255);
    }

    public void InitEnemy()
    {
        SwitchState(State.Idle);

        healthColour = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        healthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        healthColour.color = new Color32(74, 227, 14, 255);

        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();

        maxHealth = Health;
        healthBar.maxValue = maxHealth;
        stunnedHitDuration = hitDuration * 1.5f;
    }

    public override void TakeDamage(int damage, Transform character)
    {
        base.TakeDamage(damage, character);
<<<<<<< HEAD:Assets/Scripts/Enemy.cs
        DamageFeedback(character, "-" + damage, new Color32(255, 69, 0, 255));
        if (Health <= 0)
=======
        DamageFeedback(character, "-" + damage, Color.yellow);
        if (health <= 0)
>>>>>>> Development-Shrimps:Assets/Scripts/Char Controllers/EnemyAI.cs
        {
            Death();
        }
    }

    protected override void Death()
    {
        base.Death();

        // ENTER CODE FOR DEATH ANIMATIONS, ETC
        this.gameObject.SetActive(false);
    }
}