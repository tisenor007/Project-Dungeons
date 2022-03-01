using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEnemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        this.viewDistance = 10;
        this.health = 300;
        this.maxHealth = 300;
        this.hitDuration = 5.0f;
        this.damage = 25;
        this.speed = 3.5f;
        this.attackDistance = 2.5f;

        InitEnemy();
    }

    public override void Idle()
    {
        playerLocation = playerStats.gameObject.transform.position;
        enemyNavMeshAgent.speed = speed;
        //Debug.Log("LOCAL = " + transform.localEulerAngles.y);
        //Debug.Log("EULER = " + transform.eulerAngles.y);

        enemyNavMeshAgent.SetDestination(playerLocation);

        if (distanceFromPlayer <= viewDistance)
        {
            SwitchState(State.Chasing);
        }
    }
}
