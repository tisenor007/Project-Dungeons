using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System;

public class Enemy : GameCharacter
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking
    }

    public State enemyState;
    public NavMeshAgent enemyNavMeshAgent;
    public PlayerStats playerStats;

    public int viewDistance;
    public int hearingDistance;
    public int attackDistance;
    public float speed;

    public float distanceFromPlayer;
    public float hitTimer;
    public float hitDuration;
    public float stunnedHitDuration;


    public Vector3 playerLocation;
    private Vector3 enemyLocation;
    public Ray enemySight;
    public RaycastHit hitInfo;

    [SerializeField] private Image healthColour;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Transform cam;

    void Start()
    {
        
    }

    public void Update()
    {
        UpdateHealth();
        transform.GetChild(0).transform.LookAt(transform.GetChild(0).transform.position + cam.forward);

        enemySight = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        enemyLocation = this.enemyNavMeshAgent.transform.position;
        playerLocation = playerStats.gameObject.transform.position;
        distanceFromPlayer = Vector3.Distance(playerLocation, enemyLocation);

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

        Debug.Log("enemy ai running");
    }

    public virtual void Chasing()
    {
        enemyNavMeshAgent.SetDestination(playerLocation);

        if (distanceFromPlayer <= attackDistance)
        {
            hitTimer = hitDuration;
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
                hitTimer = stunnedHitDuration; // <----- will be replaced by a possible stunned state
            }
            else
            {
                playerStats.TakeDamage(damage, playerStats.GetComponent<Transform>());
                hitTimer = hitDuration;
            }
        }   

        if (distanceFromPlayer > attackDistance)
        {
            SwitchState(State.Chasing);
        }
    }

    public void SwitchState(State newState)
    {
        enemyState = newState;
    }

    public void UpdateHealth()
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

    public void InitEnemy()
    {
        SwitchState(State.Idle);
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        healthColour = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        healthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();

        healthBar.maxValue = maxHealth;
        healthColour.color = new Color32(74, 227, 14, 255);
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
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