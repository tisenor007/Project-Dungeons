using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public GameObject gameCamera;
    public LayerMask body;
    public LayerMask interactable;
    public int runSpeed = 5;
    public int sprintSpeed = 10;

    public Transform cam;
    public enum MovementDirection
    {
        Forward,
        Backward,
        Left,
        Right
    }

    public enum MovementMode
    {
        Idle,
        Running,
        Sprinting,
        Jumping,
        Falling
    }

    private enum BlendState
    {
        Idle_Running_Sprinting,
        Jumping,
        Falling
    }

    private enum PlayerWeaponIndex
    {
        Knuckles,
        Dagger,
        Cutlass,
        Club
    }

    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 7f;

    private Joystick joystick;
    private float maxInensity;
    private Vector3 input;
    private MovementMode movementMode;
    private Rigidbody rb;
    private float moveIntensity = 0.0f;
    private float moveForce = 55.0f;
    private float velocityAcceleration = 80.0f;
    private float velocityDeceleration = 80.0f;
    private float playerRotationSpeed = 4000;
    private float jumpHeight = 4f;
    private KeyCode jumpInput = KeyCode.Space;
    private float actionBlend;
    private float actionBlendAcceleration = 10.0f;
    private float actionBlendDeceleration = 3.5f;
    private float animationAttackTiming;
    private Vector3 moveDirection;
    private float jumpTimeDuration = 1.34f;
    private float jumpTimer;
    private float rayRange = 1f;
    private RaycastHit rayHit;
    private PlayerStats playerStats;
    Collider[] hitColInteraction;
    private bool canMove;
    private Vector3 cameraDestination;
    private float cameraCatchUpSpeed = 0.0525f;
    [SerializeField] private float attackTimer;
    private float attackAnimDuration;
    private static int attackButton = 0;
    private static int blockButton = 1;

    public bool CanMove { set{ canMove = value; } } 

    void Start()
    {
        playerStats = transform.GetComponent<PlayerStats>();
        rb = this.GetComponent<Rigidbody>();
        movementMode = MovementMode.Idle;
        joystick = GameObject.FindWithTag("joystick").GetComponent<FixedJoystick>();
        canMove = true;
    }

    private void Update()
    {
        //interact with object

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EnableInteractionFeedbackWithinRange();

        Vector3 nullVelcocity = new Vector3(0, 0, 0);

        //When Player is not moving
        if (joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            //player rotation
            Vector3 subtractX = new Vector3(0, rb.velocity.y, 0);

            Quaternion toRotation = Quaternion.LookRotation(rb.velocity - subtractX, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, playerRotationSpeed * Time.deltaTime);

        }

        //Cam movement/placement
        gameCamera.transform.localEulerAngles = new Vector3(65, -45, 0);
        cameraDestination = new Vector3(transform.position.x + 7, transform.position.y + 18, transform.position.z - 7);
        gameCamera.transform.position = Vector3.Lerp(gameCamera.transform.position, cameraDestination, cameraCatchUpSpeed);

        //attack countdown
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime; 
        }

        //animation movement controller
        {
            if (GameManager.manager.gameState == GameState.GAMEPLAY) { UpdatePlayerInput(); }
            if (Time.time > jumpTimer && isGrounded() == false) { movementMode = MovementMode.Falling; }
            animator.SetFloat("Velocity", moveIntensity);
            animator.SetLayerWeight(1, actionBlend);
            UpdateWeaponAnimStates(playerStats.CurrentWeaponType.nameOfItem);

            switch (movementMode)
            {
                case MovementMode.Idle:
                    animator.SetFloat("AnimState", (int)BlendState.Idle_Running_Sprinting);
                    UpdateMoveIntensity(movementMode);
                    break;
                case MovementMode.Running:
                    animator.SetFloat("AnimState", (int)BlendState.Idle_Running_Sprinting);
                    maxInensity = runSpeed;
                    UpdateMoveIntensity(movementMode);
                    break;
                case MovementMode.Sprinting:
                    animator.SetFloat("AnimState", (int)BlendState.Idle_Running_Sprinting);
                    maxInensity = sprintSpeed;
                    UpdateMoveIntensity(movementMode);
                    break;
                case MovementMode.Jumping:
                    animator.SetFloat("AnimState", (int)BlendState.Jumping);
                    break;
                case MovementMode.Falling:
                    animator.SetFloat("AnimState", (int)BlendState.Falling);
                    break;
            }
        }
    }

    public void ToggleInteraction()
    {
        if (canMove)
        {
            Interact();
        }

        else if (!canMove)
        {
            canMove = true;
            GameManager.manager.levelManager.StopReadingNote();
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

    public void FixStats(PlayerStats newplayerStats)
    {
        playerStats = newplayerStats;
    }

    private void UpdatePlayerInput()
    {
        Move();

        //blocking
        if (!IsBlocking()) { StopBlocking(); }

        //attacking
        if (!IsAttacking()) { StopAttacking(); }

        //movemonet/sprinting

        //jumping
        if (Input.GetKey(jumpInput)) { Jump(); }

        //checking to be idle
        else if (IsMoving() == false) { movementMode = MovementMode.Idle; }
    }

    private void UpdateWeaponAnimStates(string weaponName)
    {
        //tried to use switch statement here, was not allow due to emun.ToString() for some reason...... VVVV
                                                                                                    /// strings arent inherently constant switch statements require that cases are
        if (weaponName == PlayerWeaponIndex.Knuckles.ToString())
        { animator.SetFloat("WeaponAnimState", (int)PlayerWeaponIndex.Knuckles); attackAnimDuration = 0.34f; return; }
        else if (weaponName == PlayerWeaponIndex.Dagger.ToString())
        { animator.SetFloat("WeaponAnimState", (int)PlayerWeaponIndex.Dagger); attackAnimDuration = 0.34f; return; }
        else if (weaponName == PlayerWeaponIndex.Cutlass.ToString())
        { animator.SetFloat("WeaponAnimState", (int)PlayerWeaponIndex.Cutlass); attackAnimDuration = 0.34f; return; }
        else if (weaponName == PlayerWeaponIndex.Club.ToString())
        { animator.SetFloat("WeaponAnimState", (int)PlayerWeaponIndex.Club); attackAnimDuration = 0.8f; return; }
        animator.SetFloat("WeaponAnimState", (int)PlayerWeaponIndex.Dagger);
        attackAnimDuration = 0.34f;
    }

    public void Interact()
    {
        //Debug.LogWarning("call Interact");

        if (hitColInteraction.Length == 0) return;

        //Debug.LogError($"trying Interaction with {hitColInteraction.Length} object(s)");

        foreach (Collider col in hitColInteraction)
        {
            Interactable interactable = FindClosestInteractableObject().GetComponentInParent<Interactable>();
            //Debug.LogWarning($"trying Interaction with {interactable.gameObject.name}");

            if (interactable.InteractableEnabled == true)
            {
                interactable.Interact(this.gameObject);
                //Debug.LogWarning($"{gameObject.name} interacting with {interactable.gameObject.name}");
            }
        }

    }

    public GameObject FindClosestInteractableObject()
    {
        if (hitColInteraction.Length == 0) { Debug.LogError("no objects in hitColInteraction"); return null; }

        GameObject interactableObject = hitColInteraction[0].gameObject;
        float shortestDistanceFromPlayer = Vector3.Distance(gameObject.transform.position, hitColInteraction[0].gameObject.transform.position);

        foreach (Collider col in hitColInteraction)
        { 
            float calcDistance = Vector3.Distance(gameObject.transform.position, col.gameObject.transform.position);

            if (calcDistance < shortestDistanceFromPlayer)
            {
                shortestDistanceFromPlayer = calcDistance;
                interactableObject = col.gameObject;
            }
        }

        return interactableObject;
    }

    private void EnableInteractionFeedbackWithinRange()
    {
        ///[errcontrol] make sure interactable LayerMask is set in PlayerController inspector Interactable
        hitColInteraction = Physics.OverlapSphere(transform.position, // setting detection range
                interactionRadius, interactable.value, QueryTriggerInteraction.Ignore);

        //Only follow through if interactable is hit
        if (hitColInteraction.Length == 0) { return; }
        
        float r = interactionRadius - .1f; // MN#: -.1 due to radius encompasing hitColInteraction enough to not miss turning off the light

        foreach (Collider col in hitColInteraction)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            //Debug.LogError($"hit {col.gameObject.name}");

            if (col.gameObject.GetComponent<Interactable>() != null)
            {
                Interactable interactable = col.gameObject.GetComponent<Interactable>();

                if (distance >= r && interactable.FeedbackEnabled)
                {
                    interactable.DisableFeedback();
                }
                else if (distance <= r && !interactable.FeedbackEnabled)
                {
                    Physics.IgnoreCollision(this.transform.GetComponent<CapsuleCollider>(), col, true);

                    interactable.EnableFeedback();
                }
            }
            else { Debug.LogError("INTERACTABLE LAYER BEING USED BY NON-INTERACTABLE, CHECK \"Debug.LogError(hit { col.gameObject.name} );\""); }
        }
    }

    private void UpdateMoveIntensity(MovementMode movementMode)
    {
        if (movementMode == MovementMode.Idle)
        {
            if (moveIntensity > 0.0f) { moveIntensity -= Time.deltaTime * velocityDeceleration; }
        }
        else if (movementMode != MovementMode.Idle)
        {
            if (moveIntensity < maxInensity) { moveIntensity += Time.deltaTime * velocityAcceleration; }
            else { moveIntensity -= Time.deltaTime * velocityDeceleration; }
        }
    }

    private void Move()
    {
        if (canMove == false) { return; }

        //--------------------------Mobile Movement----------------------------------

        // Input based on camera position
        input = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        /*rb.velocity += (camForward * input.z + camRight * input.x) * speed * Time.deltaTime * moveForce;*/
        Vector3 movement = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * 
        new Vector3(input.x, input.y, input.z) * moveIntensity * moveForce * Time.deltaTime;

        //Lock Vertual Joystick
        if (joystick.Horizontal > 0.5) rb.velocity = movement;
        if (joystick.Horizontal < -0.5) rb.velocity = movement;
        if (joystick.Vertical > 0.5) rb.velocity = movement;
        if (joystick.Vertical < -0.5) rb.velocity = movement;

        rb.AddForce(0, -velocityAcceleration, 0);
        //----------------------------------------------------------------------------

        if (movementMode == MovementMode.Sprinting) { return; }
        if (movementMode == MovementMode.Jumping) { return; }
        if (isGrounded() == false) { return; }

        movementMode = MovementMode.Running;
    }

    private bool IsMoving()
    {
        if (joystick.Vertical > 0.5) { return true; }
        if (joystick.Vertical < -0.5) { return true; }
        if (joystick.Horizontal > 0.5) { return true; }
        if (joystick.Horizontal < -0.5) { return true; }
        if (movementMode == MovementMode.Jumping) { return true; }
        if (isGrounded() == false) { return true; }

        return false;
    }

    public void Sprint()
    {
        if (!IsMoving()) { return; }
        if (movementMode == MovementMode.Jumping) { return; }
        if (isGrounded() == false) { return; }
        if (playerStats.Health <= playerStats.MaxHealth / 4) { movementMode = MovementMode.Running; return; }
        if (IsBlocking()) { movementMode = MovementMode.Running; return; }
        if (playerStats.inWater) { movementMode = MovementMode.Running; return; }

        Debug.Log("Sprinting");
        movementMode = MovementMode.Sprinting;
    }

    public void StopSprint()
    {
        movementMode = MovementMode.Running;
    }

    private void Jump()
    {
        if (isGrounded() == false) { return; }
        if (Time.time <= jumpTimer) { return; }

        movementMode = MovementMode.Jumping;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + jumpHeight, rb.velocity.z);
        jumpTimer = Time.time + jumpTimeDuration;
    }

    public void ActivateAttack()
    {
        if (actionBlend >= 1) { return; }
        if (Time.time <= animationAttackTiming) { return; }
        if (movementMode == MovementMode.Sprinting) { return; }
        if (movementMode == MovementMode.Falling) { return; }
        if (movementMode == MovementMode.Jumping) { return; }
        if (IsBlocking()) { return; }
        if (attackTimer > 0) { return; }

        attackTimer = playerStats.AttackSpeed;
        actionBlend = 1;
        animationAttackTiming = Time.time + attackAnimDuration; // fix animations ?
        Attack();
    }

    public void Attack() 
    {
        if (IsBlocking()) { return; }
        if (playerStats.weaponHitAreaCollider.enabled == true) { return; } 
        playerStats.weaponHitAreaCollider.enabled = true;
    }

    public void StopAttacking() 
    {
        if (playerStats.weaponHitAreaCollider == null) { return; }
        if (playerStats.weaponHitAreaCollider.enabled == false) { return; }
        actionBlend -= Time.deltaTime * actionBlendDeceleration;
        playerStats.weaponHitAreaCollider.enabled = false;
    }

    public bool IsAttacking()
    {
        if (Time.time <= animationAttackTiming) { return true; }
        //if (attackBlend > 0) { return true; }

        return false;
    }

    public void ActivateBlock()
    {
        if (IsAttacking()) { return; }
        actionBlend = 1;
        animator.SetBool("Blocking", true);
    }

    public void StopBlocking()
    {
        if (IsAttacking()) { return; }
        actionBlend -= Time.deltaTime * actionBlendDeceleration;
        animator.SetBool("Blocking", false);
    }
    
    public bool IsBlocking()
    {
        if (animator.GetBool("Blocking") == true)
        {
            return true;
        }

        return false;
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
