using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public enum MovementMode
    {
        Idle,
        Jog,
        Run,
        Jump
    }
    public Animator animator;
    public GameObject camera;
    public float mouseSensitivity;
    private float maxSpeed;
    private MovementMode movement;
    private Rigidbody rb;
    private float velocity = 0.0f;
    private float acceleration = 4.0f;
    private float deceleration = 10.0f;
    private float mouseX = 0;
    private float mouseY = 0;
    private float playerRotationSpeed = 500;
    private float jumpHeight = 4f;
    private bool canJump;
    private KeyCode forwardInput = KeyCode.W;
    private KeyCode backwardInput = KeyCode.S;
    private KeyCode leftInput = KeyCode.A;
    private KeyCode rightInput = KeyCode.D;
    private KeyCode jumpInput = KeyCode.Space;
    private KeyCode sprintInput = KeyCode.LeftShift;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        movement = MovementMode.Idle;
        canJump = true;
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
        camera.transform.localEulerAngles = new Vector3(50, -45, 0);
        camera.transform.position = new Vector3(transform.position.x + 8, transform.position.y +15, transform.position.z -8);

        //animation movement controller
        animator.SetFloat("Velocity", velocity);
        if (Input.GetKey(sprintInput) &&  movement != MovementMode.Jump && canJump == true) { movement = MovementMode.Run; }
        if (Input.GetKey(forwardInput) && Input.GetKey(sprintInput) == false &&  movement != MovementMode.Jump && canJump == true || Input.GetKey(leftInput) && Input.GetKey(sprintInput) == false &&  movement != MovementMode.Jump && canJump == true || Input.GetKey(backwardInput) && Input.GetKey(sprintInput) == false &&  movement != MovementMode.Jump && canJump == true || Input.GetKey(rightInput) && Input.GetKey(sprintInput) == false &&  movement != MovementMode.Jump && canJump == true) 
        {
            movement = MovementMode.Jog;
        }
        if (Input.GetKeyDown(jumpInput) && canJump == true) { movement = MovementMode.Jump; rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + jumpHeight, rb.velocity.z);}
        else if (Input.GetKey(forwardInput) == false && Input.GetKey(leftInput) == false && Input.GetKey(backwardInput) == false && Input.GetKey(rightInput) == false &&  movement != MovementMode.Jump && canJump == true)
        { 
            movement = MovementMode.Idle;
        }

        switch (movement)
        {
            case MovementMode.Idle:
                animator.SetFloat("AnimState", 0);
                if (velocity > 0.0f) { velocity -= Time.deltaTime * deceleration; }
                break;
            case MovementMode.Jog:
                animator.SetFloat("AnimState", 0);
                maxSpeed = 5;
                Move();
                break;
            case MovementMode.Run:
                animator.SetFloat("AnimState", 0);
                maxSpeed = 10;
                Move();
                break;
            case MovementMode.Jump:
                animator.SetFloat("AnimState", 1);
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground") { canJump = true; }
        movement = MovementMode.Idle;
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground") { canJump = false; }
    }
}
