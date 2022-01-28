using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]


public class Player : GameCharacter
{
    //Urg that's ugly, maybe find a better way
    public static Player Instance { get; protected set; }

    //public Camera MainCamera;


    //public Transform CameraPosition;


    //this is only use at start, allow to grant ammo in the inspector. m_AmmoInventory is used during gameplay
 

    [Header("Control Settings")]
    public float MouseSensitivity = 100.0f;
    public float PlayerSpeed = 5.0f;
    public float RunningSpeed = 7.0f;
    public float CrouchingSpeed = 2.0f;
    public float JumpSpeed = 5.0f;

    public bool attacking = false;
    //public GameCharacter nearestEnemy;

    public bool running;
    public bool blocking;
    public GameObject shield;
    public float hitTimer = 0.5f;
    public GameObject hitArea;

    float m_VerticalSpeed = 0.0f;
    bool m_IsPaused = false;


    float m_VerticalAngle, m_HorizontalAngle;
    public float Speed { get; private set; } = 0.0f;

    public bool LockControl { get; set; }
    public bool CanPause { get; set; } = true;

    public bool Grounded => m_Grounded;

    CharacterController m_CharacterController;

    bool m_Grounded;
    float m_GroundedTimer;
    float m_SpeedAtJump = 0.0f;

    

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_IsPaused = false;
        m_Grounded = true;

        //MainCamera.transform.SetParent(CameraPosition, false);
        //MainCamera.transform.localPosition = Vector3.zero;
        //MainCamera.transform.localRotation = Quaternion.identity;
        m_CharacterController = GetComponent<CharacterController>();



        m_VerticalAngle = 0.0f;
        m_HorizontalAngle = transform.localEulerAngles.y;

        damage = 10;
        
    }

    void Update()
    {
        //Debug.Log("HEALTH: " + health);

        bool wasGrounded = m_Grounded;
        bool loosedGrounding = false;

        //attacking = Input.GetKeyUp(KeyCode.E);

        if (Input.GetKeyUp(KeyCode.E))
        {
            attacking = true;
        }
        if (attacking)
        {
            hitArea.SetActive(true);

            hitTimer -= Time.deltaTime;

            if (hitTimer < 0)
            {
                hitArea.SetActive(false);
                hitTimer = 0.5f;
                attacking = false;
            }
            
        }

        if (!m_CharacterController.isGrounded)
        {
            if (m_Grounded)
            {
                m_GroundedTimer += Time.deltaTime;
                if (m_GroundedTimer >= 0.5f)
                {
                    loosedGrounding = true;
                    m_Grounded = false;
                }
            }
        }
        else
        {
            m_GroundedTimer = 0.0f;
            m_Grounded = true;
        }

        Speed = 0;
        Vector3 move = Vector3.zero;
        if (!m_IsPaused && !LockControl)
        {
            // Jump (we do it first as 
            if (m_Grounded && Input.GetButtonDown("Jump"))
            {
                m_VerticalSpeed = JumpSpeed;
                m_Grounded = false;
                loosedGrounding = true;                
            }

            running = Input.GetKey(KeyCode.LeftShift);
            float actualSpeed = running ? RunningSpeed : PlayerSpeed;

            blocking = Input.GetKey(KeyCode.RightShift);
            if (blocking)
            { 
                actualSpeed = blocking ? CrouchingSpeed : PlayerSpeed;
                shield.SetActive(true);
            }
            if (blocking == false)
            {
                shield.SetActive(false);
            }

            if (loosedGrounding)
            {
                m_SpeedAtJump = actualSpeed;
            }

            // Move around with WASD
            move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            if (move.sqrMagnitude > 1.0f)
                move.Normalize();

            float usedSpeed = m_Grounded ? actualSpeed : m_SpeedAtJump;

            move = move * usedSpeed * Time.deltaTime;

            move = transform.TransformDirection(move);
            m_CharacterController.Move(move);

            // Turn player
            float turnPlayer = Input.GetAxis("Mouse X") * MouseSensitivity;
            m_HorizontalAngle = m_HorizontalAngle + turnPlayer;

            if (m_HorizontalAngle > 360) m_HorizontalAngle -= 360.0f;
            if (m_HorizontalAngle < 0) m_HorizontalAngle += 360.0f;

            Vector3 currentAngles = transform.localEulerAngles;
            currentAngles.y = m_HorizontalAngle;
            transform.localEulerAngles = currentAngles;

            // Camera look up/down
            var turnCam = -Input.GetAxis("Mouse Y");
            turnCam = turnCam * MouseSensitivity;
            m_VerticalAngle = Mathf.Clamp(turnCam + m_VerticalAngle, -89.0f, 89.0f);
            //currentAngles = CameraPosition.transform.localEulerAngles;
            currentAngles.x = m_VerticalAngle;
            //CameraPosition.transform.localEulerAngles = currentAngles;



            Speed = move.magnitude / (PlayerSpeed * Time.deltaTime);



            // Fall down / gravity
            m_VerticalSpeed = m_VerticalSpeed - 10.0f * Time.deltaTime;
            if (m_VerticalSpeed < -10.0f)
                m_VerticalSpeed = -10.0f; // max fall speed
            var verticalMove = new Vector3(0, m_VerticalSpeed * Time.deltaTime, 0);
            var flag = m_CharacterController.Move(verticalMove);
            if ((flag & CollisionFlags.Below) != 0)
                m_VerticalSpeed = 0;

            if (!wasGrounded && m_Grounded)
            {
                //play a sound after being in air and landing
            }
        }

    }

    public void Attack()
    {
        //nearestEnemy.TakeDamage(damage);
        //Debug.Log("ENEMY HEALTH: " + nearestEnemy.health);
    }
}
