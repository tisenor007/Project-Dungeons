using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum MovementMode
    {
        Idle,
        Running,
        Sprinting,
        Jumping,
        Falling
    }

    
    public Animator animator;
    public GameObject gameCamera;
    public LayerMask body;
    public LayerMask interactable;
    public float interactionRadius = 7f;

    private float maxInensity;
    private MovementMode movementMode;
    private Rigidbody rb;
    private float moveIntensity = 0.0f;
    private float velocityAcceleration = 4.0f;
    private float velocityDeceleration = 15.0f;
    private float playerRotationSpeed = 450;
    private float jumpHeight = 4f;
    private KeyCode forwardInput = KeyCode.W;
    private KeyCode backwardInput = KeyCode.S;
    private KeyCode leftInput = KeyCode.A;
    private KeyCode rightInput = KeyCode.D;
    private KeyCode jumpInput = KeyCode.Space;
    private KeyCode sprintInput = KeyCode.LeftShift;
    private float attackBlend;
    private float attackBlendAcceleration = 10.0f;
    private float attackBlendDeceleration = 3.5f;
    private float attackTimeDuration = 1.34f;
    private float attackTimer;
    private Vector3 moveDirection;
    private float jumpTimeDuration = 1.34f;
    private float jumpTimer;
    private float rayRange = 0.85f;
    private RaycastHit rayHit;
    [SerializeField]
    private bool canInteract;
    Collider[] hitColInteraction;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        movementMode = MovementMode.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        //General Move
        if (Input.GetKey(forwardInput) == true) { moveDirection += new Vector3(-1, 0, 1); }
        if (Input.GetKey(backwardInput) == true) { moveDirection += new Vector3(1, 0, -1); }
        if (Input.GetKey(rightInput) == true) { moveDirection += new Vector3(1, 0, 1); }
        if (Input.GetKey(leftInput) == true) { moveDirection += new Vector3(-1, 0, -1); }
        else if (Input.GetKey(forwardInput) == false && Input.GetKey(backwardInput) == false && Input.GetKey(rightInput) == false && Input.GetKey(leftInput) == false)
        {
            moveDirection = Vector3.zero;
        }
        moveDirection.Normalize();
        transform.Translate(moveDirection * moveIntensity * Time.deltaTime, Space.World);

        //player rotation
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, playerRotationSpeed * Time.deltaTime);
        }

        //Cam movement/placement
        gameCamera.transform.localEulerAngles = new Vector3(50, -45, 0);
        gameCamera.transform.position = new Vector3(transform.position.x + 8, transform.position.y + 15, transform.position.z - 8);

        //animation movement controller
        { 
            CheckPlayerInputandPerformPlayerAnims();
            if (Time.time > jumpTimer && isGrounded() == false) { movementMode = MovementMode.Falling; }
            animator.SetFloat("Velocity", moveIntensity);
            animator.SetLayerWeight(1, attackBlend);

            switch (movementMode)
            {
                case MovementMode.Idle:
                    animator.SetFloat("AnimState", 0);
                    if (moveIntensity > 0.0f) { moveIntensity -= Time.deltaTime * velocityDeceleration; }
                    break;
                case MovementMode.Running:
                    animator.SetFloat("AnimState", 0);
                    maxInensity = 5;
                    AdjustMoveIntensity();
                    break;
                case MovementMode.Sprinting:
                    animator.SetFloat("AnimState", 0);
                    maxInensity = 10;
                    AdjustMoveIntensity();
                    break;
                case MovementMode.Jumping:
                    animator.SetFloat("AnimState", 1);
                    break;
                case MovementMode.Falling:
                    animator.SetFloat("AnimState", 2);
                    break;
            }
        }

        //Interaction 
        {
            /// make sure interactable LayerMask is set in PlayerController inspector Interactable
            hitColInteraction = Physics.OverlapSphere(transform.position,
                    interactionRadius, interactable.value, QueryTriggerInteraction.Ignore);

            //highlight objects interactable to the player
            if (hitColInteraction.Length != 0)
            {
                canInteract = true;
                float r = interactionRadius - .1f; // MN#: -.5 due to radius encompasing hitColInteraction enough to not miss turning off the light
                Debug.LogError($"hitting {hitColInteraction.Length} object(s)");

                foreach (Collider col in hitColInteraction)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    Debug.LogError($"hit {col.gameObject.name}");

                    if (distance >= r)
                    {
                        col.gameObject.GetComponent<Interactable>().DisableFeedback();
                    }
                    else { col.gameObject.GetComponent<Interactable>().EnableFeedback(); }
                    
                }
            }

            //interact with object
            if (Input.GetKeyDown(KeyCode.E))
            {
                canInteract = false;
                Debug.LogWarning("call Interact");

                if (hitColInteraction.Length != 0)
                {
                    Debug.LogError  ($"trying Interaction with {hitColInteraction.Length} object(s)");
                    
                    foreach (Collider col in hitColInteraction)
                    {
                        Interactable interactable = col.gameObject.GetComponent<Interactable>();
                        Debug.LogWarning($"trying Interaction with {interactable.gameObject.name}");
                        
                        if (interactable.InteractableEnabled == true)
                        {
                            Debug.LogWarning($"Interacting with {interactable.gameObject.name}");
                            interactable.Interact(this.gameObject);
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //interaction range
        { 
            float r = interactionRadius;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, r);
        }
    }

    public void AdjustMoveIntensity()
    {
        if (moveIntensity < maxInensity) { moveIntensity += Time.deltaTime * velocityAcceleration; }
        else
        {
            moveIntensity -= Time.deltaTime * velocityDeceleration;
        }
    }

    private void CheckPlayerInputandPerformPlayerAnims()
    {
        if (Input.GetMouseButton(0) == true && attackBlend < 1 && movementMode != MovementMode.Sprinting && movementMode != MovementMode.Falling && Time.time > attackTimer) 
        {
            attackBlend = 1; attackTimer = Time.time + attackTimeDuration; 
        }
        if (Input.GetMouseButton(0) == false && attackBlend > 0 && Time.time > attackTimer) { attackBlend -= Time.deltaTime * attackBlendDeceleration; }
        if (Input.GetKey(sprintInput) && movementMode != MovementMode.Jumping && isGrounded() == true) { movementMode = MovementMode.Sprinting; }
        if (Input.GetKey(forwardInput) == true && Input.GetKey(sprintInput) == false && movementMode != MovementMode.Jumping && isGrounded() == true || Input.GetKey(leftInput) && Input.GetKey(sprintInput) == false && movementMode != MovementMode.Jumping && isGrounded() == true || Input.GetKey(backwardInput) && Input.GetKey(sprintInput) == false && movementMode != MovementMode.Jumping && isGrounded() == true || Input.GetKey(rightInput) && Input.GetKey(sprintInput) == false && movementMode != MovementMode.Jumping && isGrounded() == true)
        {
            movementMode = MovementMode.Running;
        }
        if (Input.GetKey(jumpInput) && isGrounded() == true && Time.time > jumpTimer) { movementMode = MovementMode.Jumping; rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + jumpHeight, rb.velocity.z); jumpTimer = Time.time + jumpTimeDuration;}
        else if (Input.GetKey(forwardInput) == false && Input.GetKey(leftInput) == false && Input.GetKey(backwardInput) == false && Input.GetKey(rightInput) == false && movementMode != MovementMode.Jumping && isGrounded() == true)
        {
            movementMode = MovementMode.Idle;
        }
    }

    private bool isGrounded()
    {
        /// Make sure body Layermask is set in PlayerController Inspector Body
        if (Physics.Raycast(this.transform.position, -this.transform.up, out rayHit, rayRange, ~body)) 
        {
            return true;
        }
        return false;
    }

    private void OnCollisionStay(Collision other)
    {
        if (isGrounded() == true && movementMode == MovementMode.Jumping || isGrounded() == true && movementMode == MovementMode.Falling) 
        { 
            movementMode = MovementMode.Idle; 
        }
    }
}
