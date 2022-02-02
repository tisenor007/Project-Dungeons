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
    //public Player player;

    //public Animator animator;
    public NavMeshAgent enemy;

    public PlayerStats player;
    //public Transform[] points;

    //public TextMeshProUGUI currentStateTxt;

    //public int patrolDestinationPoint;
    //public int patrolDestinationAmount;
    public int viewDistance = 20;
    public int hearingDistance = 20;
    public int attackDistance = 3;
    //public int attackDamage = 10;
    public float distance;
    public float chasingTime = 4.0f;
    public float attackDuration = 0.5f;
    public float stunnedHitDuration = 3.0f;
    private float hitTime = 3.0f;


    private Vector3 playerLocation;
    private Vector3 enemyLocation;

    public Ray enemySight;
    public RaycastHit hitInfo;

    public bool hitable;

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
    }

    void Update()
    {
        if (player == null) { player = GameObject.Find("IsometricCharacterController").transform.GetChild(0).GetComponent<PlayerStats>(); }

        UpdateHealth();
        transform.GetChild(0).transform.LookAt(transform.GetChild(0).transform.position + cam.forward);

        enemySight = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        enemyLocation = enemy.transform.position;
        playerLocation = player.gameObject.transform.position;
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
        playerLocation = player.gameObject.transform.position;

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
            hitTime = attackDuration;
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
            if (player.blocking)
            {
                player.TakeDamage((int)(damage / 4), player.GetComponent<Transform>());
                hitTime = stunnedHitDuration; // <----- will be replaced by a possible stunned state
            }
            else
            {
                player.TakeDamage(damage, player.GetComponent<Transform>());
                hitTime = attackDuration;
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

    protected override void Death()
    {
        base.Death();

        // ENTER CODE FOR DEATH ANIMATIONS, ETC
        this.gameObject.SetActive(false);
    }

}