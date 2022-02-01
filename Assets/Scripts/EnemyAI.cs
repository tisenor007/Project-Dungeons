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
    public int hearingDistance = 10;
    public int attackDistance = 3;
    //public int attackDamage = 10;
    public float distance;
    public float chasingTime = 4.0f;
    public float hitTime = 3.0f;

    private Vector3 playerLocation;
    private Vector3 enemyLocation;

    public Ray enemySight;
    public RaycastHit hitInfo;

    public bool hitable;

    void Start()
    {
        health = 30;
        damage = 5;
        enemy = GetComponent<NavMeshAgent>();
        SwitchState(State.Idle);
    }

    void Update()
    {
        enemySight = new Ray(transform.position, transform.TransformDirection(Vector3.forward));

        //currentStateTxt.text = "Current State: " + enemyState;
        enemyLocation = enemy.transform.position;
        playerLocation = player.gameObject.transform.position;
        distance = Vector3.Distance(playerLocation, enemyLocation);

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDistance, Color.white);

        /*if (distance <= hearingDistance && player.running == true && distance > attackDistance)
        {
            SwitchState(State.Listening);
        }*/


        switch (enemyState)
        {
            case State.Idle:
                //Debug.Log("State: Idle");
                Idle();
                break;

            case State.Chasing:
                //Debug.Log("State: Chasing");
                Chasing();
                break;

            case State.Attacking:
                //Debug.Log("State: Attacking");
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
                Debug.Log("triggered!!!!");
                SwitchState(State.Chasing);
            }
        }

    }

    void Chasing()
    {
        enemy.SetDestination(playerLocation);

        if (distance <= attackDistance)
        {
            hitTime = 3.0f;
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
                hitTime = 5.0f; // <----- will be replaced by a possible stunned state
            }
            else
            {
                player.TakeDamage(damage, player.GetComponent<Transform>());
                hitTime = 3.0f;
            }

            Debug.Log("PLAYER HEALTH: " + player.health);
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

    



}