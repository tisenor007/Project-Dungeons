using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System;

public class EnemyAI : GameCharacter
{
    enum State
    {
        Idle,
        Chasing,
        Attacking
    }
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

        healthColour = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        healthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthColour.GetComponent<Image>().color = new Color32(74, 227, 14, 255);
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    void Update()
    {
        UpdateHealth();
        transform.GetChild(0).transform.LookAt(transform.GetChild(0).transform.position + cam.forward);

        enemySight = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        enemyLocation = enemy.transform.position;
        playerLocation = playerStats.gameObject.transform.position;
        distance = Vector3.Distance(playerLocation, enemyLocation);

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDistance, Color.white);

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
    }

    void Idle()
    {
        playerLocation = playerStats.gameObject.transform.position;

        if (Physics.Raycast(enemySight, out hitInfo, viewDistance))
        {
            if (hitInfo.collider.tag == "Player")
            {
                SwitchState(State.Chasing);
            }
        }
    }

    void Chasing()
    {
        enemy.SetDestination(playerLocation);

        if (distance <= attackDistance)
        {
            hitTime = attackSpeed;
            SwitchState(State.Attacking);
        }

        if (distance >= viewDistance)
        {
            SwitchState(State.Idle);
        }
    }

    void Attacking()
    {
        enemy.SetDestination(enemyLocation);

        hitTime -= Time.deltaTime;

        if (hitTime <= 0.0f)
        {
            if (playerStats.shield.activeSelf == true)
            {
                playerStats.TakeDamage((int)(damage / 4), playerStats.GetComponent<Transform>());
                hitTime = stunnedHitDurationAddition; // <----- will be replaced by a possible stunned state
            }
            else
            {
                playerStats.TakeDamage(damage, playerStats.GetComponent<Transform>());
                hitTime = attackSpeed;
            }
        }   

        if (distance > attackDistance)
        {
            SwitchState(State.Chasing);
        }
    }

    void SwitchState(State newState)
    {
        enemyState = newState;
    }

    void UpdateHealth()
    {
        healthBar.value = Health;
        //miniHealthBar.GetComponent<Slider>().value = health;

        if (Health < maxHealth * 0.8 && Health > maxHealth * 0.6)
            healthColour.color = new Color32(167, 227, 16, 255);

        if (Health < maxHealth * 0.6 && Health > maxHealth * 0.4)
            healthColour.color = new Color32(227, 176, 9, 255);

        if (Health < maxHealth * 0.4 && Health > maxHealth * 0.2)
            healthColour.color = new Color32(240, 86, 48, 255);

        if (Health < maxHealth * 0.2)
            healthColour.color = new Color32(204, 40, 0, 255);
    }

    public override void TakeDamage(int damage, Transform character)
    {
        base.TakeDamage(damage, character);
        DamageFeedback(character, "-" + damage, new Color32(255, 69, 0, 255));
        if (health <= 0)
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