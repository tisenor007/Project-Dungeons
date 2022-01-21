using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public enum MovementMode
    {
        Normal_Idle,
        Normal_Walk,
        Normal_Run,
        Normal_Jump,
        Fight_Idle,
        Fight_Walk,
        Fight_Punch
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
    private bool isGrounded;
    private KeyCode forwardInput = KeyCode.W;
    private KeyCode backwardInput = KeyCode.S;
    private KeyCode leftInput = KeyCode.A;
    private KeyCode rightInput = KeyCode.D;
    private KeyCode jumpInput = KeyCode.Space;
    private KeyCode sprintInput = KeyCode.LeftShift;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        movement = MovementMode.Normal_Idle;
        isGrounded = true;
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
        if (Input.GetKey(sprintInput) &&  movement != MovementMode.Normal_Jump && isGrounded == true) { movement = MovementMode.Normal_Run; }
        if (Input.GetKey(forwardInput) && Input.GetKey(sprintInput) == false &&  movement != MovementMode.Normal_Jump && isGrounded == true || Input.GetKey(leftInput) && Input.GetKey(sprintInput) == false &&  movement != MovementMode.Normal_Jump && isGrounded == true || Input.GetKey(backwardInput) && Input.GetKey(sprintInput) == false &&  movement != MovementMode.Normal_Jump && isGrounded == true || Input.GetKey(rightInput) && Input.GetKey(sprintInput) == false &&  movement != MovementMode.Normal_Jump && isGrounded == true) 
        {
            movement = MovementMode.Normal_Walk;
        }
        if (Input.GetKeyDown(jumpInput) && isGrounded == true) { movement = MovementMode.Normal_Jump; rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + jumpHeight, rb.velocity.z);}
        else if (Input.GetKey(forwardInput) == false && Input.GetKey(leftInput) == false && Input.GetKey(backwardInput) == false && Input.GetKey(rightInput) == false &&  movement != MovementMode.Normal_Jump && isGrounded == true)
        { 
            movement = MovementMode.Normal_Idle;
        }

        switch (movement)
        {
            case MovementMode.Normal_Idle:
                animator.SetFloat("AnimState", 0);
                if (velocity > 0.0f) { velocity -= Time.deltaTime * deceleration; }
                break;
            case MovementMode.Normal_Walk:
                animator.SetFloat("AnimState", 0);
                maxSpeed = 5;
                Move();
                break;
            case MovementMode.Normal_Run:
                animator.SetFloat("AnimState", 0);
                maxSpeed = 10;
                Move();
                break;
            case MovementMode.Normal_Jump:
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
        if (other.gameObject.tag == "Ground") { isGrounded = true;}
        //This line fixes jump movement, but only here, other methods failed....
        if (movement == MovementMode.Normal_Jump) { movement = MovementMode.Normal_Idle; }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground") { isGrounded = false;}
    }
}
