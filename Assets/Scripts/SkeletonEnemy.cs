using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEnemy : Enemy
{
    public float walkDistance;
    private float previousRotationDistance;
    public float currentRotationDistance;
    private float rotationSpeed;

    private const float caughtTimerConst = 1.5f;
    private float caughtTimer;
    private const float walkTimerConst = 5.0f;
    [SerializeField]
    private float walkTimer;

    [SerializeField]
    private Vector3 walkToLocation = Vector3.zero;

    void Start()
    {
        this.viewDistance = 15;
        maxHealth = 50;
        this.health = maxHealth;
        this.attackSpeed = 3.0f;
        this.damage = 15;
        this.speed = 5.0f;
        this.attackDistance = 3;
        this.enemyType = EnemyType.Skeleton;
        this.hitTimer = 1.5f;

        this.attackSound = SoundManager.Sound.SkeletonAttack;
        this.chasingSound = SoundManager.Sound.SkeletonChasing;
        this.deathSound = SoundManager.Sound.SkeletonDeath;
        this.idleSound = SoundManager.Sound.SkeletonSteps;

        this.rotationSpeed = 100.0f;
        walkDistance = Random.Range(5, 10);
        currentRotationDistance = Random.Range(45, 135);

        caughtTimer = caughtTimerConst;
        walkTimer = walkTimerConst;

        walkToLocation = transform.position; // + (transform.forward * walkDistance);

        InitEnemy();
    }

    void Rotate()
    {
        if (currentRotationDistance >= 350)
        {
            currentRotationDistance -= 350;
        }

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        //Debug.Log("ROTATING");

        if (transform.localEulerAngles.y >= currentRotationDistance && transform.localEulerAngles.y <= currentRotationDistance + 5)
        {
            //Debug.Log("Angle = " + transform.eulerAngles.y);
            rotationSpeed = 100.0f;
           // Debug.Log("ROTATION COMPLETE");
            walkDistance = Random.Range(5, 10);

            walkToLocation = transform.position + (transform.forward * walkDistance);
            previousRotationDistance = currentRotationDistance;
            currentRotationDistance += Random.Range(45, 135);
            walkTimer = walkTimerConst;
            //enemyNavMeshAgent.SetDestination(walkToLocation);

            if (currentRotationDistance >= 350)
            {
                currentRotationDistance -= 350;
            }

            if (Physics.Raycast(enemySight, out hitInfo, viewDistance))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    //Debug.Log("triggered!!!!");

                    SwitchState(State.Chasing);
                }
            }
        }
    }

    public override void Idle()
    {
        walkTimer -= Time.deltaTime;

        //Debug.LogError("LOCAL POSITION: " + transform.localPosition);
        //Debug.LogError("POSITION: " + transform.position);
        //Debug.LogError("WALK TO LOCATION: " + walkToLocation);

        if (walkToLocation.y != transform.position.y) walkToLocation.y = transform.position.y;

        enemyNavMeshAgent.SetDestination(walkToLocation);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDistance, Color.white);

        if (Vector3.Distance(transform.position, walkToLocation) <= 3f)
        {
            walkToLocation = transform.position;
            //Debug.Log("AT LOCATION");
            Rotate();
        }

        if (Physics.Raycast(enemySight, out hitInfo, viewDistance))
        {
            if (hitInfo.collider.tag == "Player")
            {
                //Debug.Log("triggered!!!!");
                
                SwitchState(State.Chasing);
            }
        }

        if (distanceFromPlayer <= attackDistance) { SwitchState(State.Attacking); }

        if (walkTimer <= 0.0f) walkToLocation = transform.position;
    }

    public override void Chasing()
    {

        if (caughtTimer > 0.0)
        {
            SwitchAnimation("Spotted");

            //viewDistance = viewDistance * 2;
            enemyNavMeshAgent.SetDestination(transform.position);
            transform.LookAt(playerLocation);

            caughtTimer -= Time.deltaTime;
        }

        if (caughtTimer <= 0)
        {
            enemyNavMeshAgent.speed = speed * 2;
            enemyNavMeshAgent.SetDestination(playerLocation);
            SwitchAnimation("Chasing");
        }

        if (distanceFromPlayer <= attackDistance)
        {
            enemyNavMeshAgent.speed = speed;
            walkToLocation = transform.position;
            SwitchState(State.Attacking);
        }

        if (distanceFromPlayer > viewDistance)
        {
            enemyNavMeshAgent.speed = speed;
            walkToLocation = transform.position;
            //caughtTimer = caughtTimerConst;
            SwitchState(State.Idle);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("WALL HIT");

        if (collision.transform.tag == "Player") Physics.IgnoreCollision(this.transform.GetComponent<BoxCollider>(), collision.collider);
        if (collision.transform.tag != "Player")
        {
            //"1" counts as how far skeleton will bounce back when colliding with wall
            enemyNavMeshAgent.Warp(transform.position - transform.forward * 1f);

            currentRotationDistance = previousRotationDistance += Random.Range(135, 180);
            rotationSpeed = 200.0f;
            walkToLocation = transform.position;
        }
    }
}
