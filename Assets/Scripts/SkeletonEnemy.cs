using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEnemy : Enemy
{
    private float walkDistance;
    public float rotationDistance;
    private float rotationSpeed;

    [SerializeField]
    private Vector3 walkToLocation = Vector3.zero;

    void Start()
    {
        this.viewDistance = 5;
        this.health = 100;
        this.maxHealth = 100;
        this.hitDuration = 3.0f;
        this.damage = 25;
        this.speed = 5.0f;
        this.attackDistance = 4;

        this.rotationSpeed = 20.0f;
        walkDistance = Random.Range(1, 3);
        rotationDistance = Random.Range(45, 135);

        walkToLocation = transform.position + transform.forward;

        InitEnemy();
    }

    void Rotate()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        Debug.Log("ROTATING");

        if (transform.localEulerAngles.y >= rotationDistance)
        {
            Debug.Log("ROTATION COMPLETE");
            walkDistance = Random.Range(4, 9);
            //walkToLocation.x = transform.position.x + walkDistance;
            walkToLocation = transform.position + transform.forward;
            rotationDistance = rotationDistance = Random.Range(45, 135);
        }
    }

    public override void Idle()
    {
        enemyNavMeshAgent.SetDestination(walkToLocation);

        if (Vector3.Distance(transform.position, walkToLocation) < 0.5f)
        {
            Debug.Log("AT LOCATION");
            Rotate();
        }
    }



}
