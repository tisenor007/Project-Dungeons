using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEnemy : Enemy
{
    public float walkDistance;
    private float previousRotationDistance;
    public float currentRotationDistance;
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

        this.rotationSpeed = 100.0f;
        walkDistance = Random.Range(1, 3);
        currentRotationDistance = Random.Range(45, 135);

        walkToLocation = transform.position + (transform.forward * walkDistance);

        InitEnemy();
    }

    void Rotate()
    {
        if (currentRotationDistance >= 350)
        {
            currentRotationDistance -= 350;
        }

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        Debug.Log("ROTATING");

        if (transform.localEulerAngles.y >= currentRotationDistance && transform.localEulerAngles.y <= currentRotationDistance + 5)
        {
            Debug.Log("Angle = " + transform.eulerAngles.y);
            rotationSpeed = 100.0f;
            Debug.Log("ROTATION COMPLETE");
            walkDistance = Random.Range(4, 9);
            //walkToLocation.x = transform.position.x + walkDistance;
            walkToLocation = transform.position + (transform.forward * walkDistance);
            previousRotationDistance = currentRotationDistance;
            currentRotationDistance += Random.Range(45, 135);

            if (currentRotationDistance >= 350)
            {
                currentRotationDistance -= 350;
            }
        }
    }

    public override void Idle()
    {
       Debug.Log("WALKING");
        enemyNavMeshAgent.SetDestination(walkToLocation);

        if (Vector3.Distance(transform.position, walkToLocation) <= 0.5f)
        {
            walkToLocation = transform.position;
            Debug.Log("AT LOCATION");
            Rotate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("WALK HIT");
        enemyNavMeshAgent.Warp(transform.position - transform.forward * 1.5f);
        //transform.rotation = Quaternion.Euler(0, -transform.localRotation.y, 0);

        currentRotationDistance = previousRotationDistance += Random.Range(135, 180);
        rotationSpeed = 200.0f;
        walkToLocation = transform.position;

    }



}
