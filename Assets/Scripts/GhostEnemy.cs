using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostEnemy : Enemy
{
    public float rotationSpeed;

    public float bobUp;
    public float bobDown;
    public float bobSpeed = 2;

    public bool movingDown = false;

    public GameObject ghostBody;
    void Start()
    {
        enemyNavMeshAgent = this.GetComponent<NavMeshAgent>();
        //ghostBody = transform.GetChild(0).position;

        this.viewDistance = 10;
        this.health = 100;
        this.hitDuration = 3.0f;
        this.damage = 5;
        this.speed = 5.0f;
        this.rotationSpeed = 0.1f;

        bobUp = transform.position.y + 3;
        bobDown = transform.position.y + 1;
        enemyNavMeshAgent.speed = speed;

        InitEnemy();
    }

    void Radius() // -90? -0 > 0 > 90 > 180 > -180 > -90 ||
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);

        if (transform.localEulerAngles.y > 45 && transform.localEulerAngles.y < 135  || (transform.localEulerAngles.y > 135 && transform.localEulerAngles.y < -135))
        {
            rotationSpeed = 10f;
            Debug.Log("BETWEEN 45 AND 135");
        }
        else
        {
            rotationSpeed = 20f;
        }

    }

    void Bobbing()
    {
        if (movingDown == false)
        {
            ghostBody.transform.Translate(Vector3.up * Time.deltaTime, Space.World);


            if (ghostBody.transform.position.y > bobUp)
            {
                movingDown = true;

            }
        }
        else
        {
            ghostBody.transform.Translate(Vector3.down * Time.deltaTime, Space.World);

            if (ghostBody.transform.position.y < bobDown)
            {
                movingDown = false;

            }
        }


    }

    public override void Idle()
    {
        Bobbing();
        playerLocation = playerStats.gameObject.transform.position;

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        
        Radius();



        if (distanceFromPlayer <= viewDistance)
        {
            //SwitchState(State.Chasing);
        }
    }
    public override void Chasing()
    {
        enemyNavMeshAgent.speed = 10;
        enemyNavMeshAgent.SetDestination(playerLocation);

        if (distanceFromPlayer <= attackDistance)
        {
            hitTimer = hitDuration;
            SwitchState(State.Attacking);
        }

        if (distanceFromPlayer >= viewDistance)
        {
            enemyNavMeshAgent.speed = speed;
            SwitchState(State.Idle);
        }
    }
}

    
