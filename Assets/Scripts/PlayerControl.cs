using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public enum MovementMode
    {
        Idle,
        Jog,
        Run
    }
    public Transform playerTransform;
    public MovementMode movement;
    public Animator animator;
    public float maxSpeed;
    private Rigidbody rb;
    private float velocity = 0.0f;
    private float acceleration = 4.0f;
    private float deceleration = 6.0f;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal") * maxSpeed;
        float zAxis = Input.GetAxis("Vertical") * maxSpeed;
        Vector3 movePos = transform.right * xAxis + transform.forward * zAxis;
        Vector3 newMovePos = new Vector3(movePos.x, rb.velocity.y, movePos.z);

        rb.velocity = newMovePos;
        animator.SetFloat("Velocity", velocity);

        if (Input.GetKey(KeyCode.LeftShift)) { movement = MovementMode.Run; }
        else { movement = MovementMode.Jog; }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) 
        {
            if (movement != MovementMode.Run){movement = MovementMode.Jog;}
        }
        if (Input.anyKey == false){movement = MovementMode.Idle;}

        switch (movement)
        {
            case MovementMode.Idle:
                if (velocity >= 0.0f) { velocity -= Time.deltaTime * deceleration; }
                break;
            case MovementMode.Jog:
                maxSpeed = 5;
                Move();
                break;
            case MovementMode.Run:
                maxSpeed = 10;
                Move();
                break;
        }
    }
    public void Move()
    {
        if (velocity < maxSpeed) { velocity += Time.deltaTime * acceleration; }
        else
        {
            velocity -= Time.deltaTime * deceleration;
        }
    }
}
