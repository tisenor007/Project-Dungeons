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
    public Animator animator;
    public GameObject camera;
    public float mouseSensitivity;
    private float maxSpeed;
    private MovementMode movement;
    private Rigidbody rb;
    private float velocity = 0.0f;
    private float acceleration = 4.0f;
    private float deceleration = 6.0f;
    private float mouseX = 0;
    private float mouseY = 0;
    private float playerRotationSpeed = 500;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //General Move
        float vertInput = Input.GetAxis("Horizontal") * velocity;
        float horzInput = Input.GetAxis("Vertical") * velocity;
        Vector3 movePos = new Vector3(vertInput, 0, horzInput);
        movePos.Normalize();
        transform.Translate(movePos * velocity * Time.deltaTime, Space.World);

        //player rotation
        if (movePos != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movePos, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, playerRotationSpeed * Time.deltaTime);
        }

        //Cam movement/placement
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, 0, 40);
        camera.transform.localEulerAngles = new Vector3(mouseY, -45, 0);
        camera.transform.position = new Vector3(transform.position.x + 8, transform.position.y +7, transform.position.z -8);

        //animation movement controller
        animator.SetFloat("Velocity", velocity);
        if (Input.GetKey(KeyCode.LeftShift)) { movement = MovementMode.Run; }
        else { movement = MovementMode.Jog; }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) 
        {
            if (movement != MovementMode.Run){movement = MovementMode.Jog;}
        }
        else if (Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.D) == false)
        { 
            movement = MovementMode.Idle;
        }

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
