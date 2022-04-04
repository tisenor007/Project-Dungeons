using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;

public class Enemy : CharacterStats
{
    [SerializeField]
    protected enum State
    {
        Idle,
        Chasing,
        Attacking,
        Stunned,
        Dying
    }

    protected State enemyState;
    protected string[] animationStates;
    protected NavMeshAgent enemyNavMeshAgent;
    protected PlayerStats playerStats;
    protected Animator animator;
    protected AnimationClip clip;
    protected string currentAnimationState;

    protected Material[] enemyModel;

    protected float attackAnimTime;
    protected float stunnedAnimTime;
    protected float dyingAnimTime;

    protected float viewDistance;
    protected float hearingDistance;
    protected float attackDistance;
    protected float speed;
    protected string audioGroup;

    protected float distanceFromPlayer;
    protected float hitTimer;
    protected float stunnedTimer;
    protected float stunnedHitDuration;
    protected float dyingTimer;

    protected Vector3 playerLocation;
    protected Vector3 enemyLocation;
    protected Ray enemySight;
    protected RaycastHit hitInfo;

    [SerializeField] private Image healthColour;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject[] availableDrops;
    [SerializeField] private int itemDropChance;

    protected SoundManager.Sound attackSound;
    protected SoundManager.Sound chasingSound;
    protected SoundManager.Sound deathSound;
    protected SoundManager.Sound idleSound;

    public void Update()
    {
        UpdateHealth();
        transform.GetChild(0).transform.LookAt(transform.GetChild(0).transform.position + cam.forward);

        enemySight = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        enemyLocation = this.enemyNavMeshAgent.transform.position;
        playerLocation = playerStats.gameObject.transform.position;
        distanceFromPlayer = Vector3.Distance(playerLocation, enemyLocation);

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDistance, Color.white);

        switch (enemyState)
        {
            case State.Idle:
                PlayAudio(this);
                SwitchAnimation("Idle");
                Idle();
                break;

            case State.Chasing:
                PlayAudio(this);
                SwitchAnimation("Chasing");
                Chasing();
                break;

            case State.Attacking:
                //PlayAudio(this);
                //SwitchAnimation("Attacking Idle");
                Attacking();
                break;

            case State.Stunned:
                //PlayAudio(this);
                //SwitchAnimation("Stunned");
                Stunned();
                break;

            case State.Dying:
                Dying();
                break;
        }

        //Debug.Log(enemyState);
    }

    public virtual void Idle()
    {
        playerLocation = playerStats.gameObject.transform.position;

        if (Physics.Raycast(enemySight, out hitInfo, viewDistance))
        {
            if (hitInfo.collider.tag == "Player")
            {
                SwitchState(State.Chasing);
            }
        }

        //Debug.Log("enemy ai running");
    }

    public virtual void Chasing()
    {
        enemyNavMeshAgent.SetDestination(playerLocation);

        if (distanceFromPlayer <= attackDistance)
        {
            hitTimer = attackSpeed;
            SwitchState(State.Attacking);
        }

        if (distanceFromPlayer >= viewDistance)
        {
            SwitchState(State.Idle);
        }
    }

    void Attacking()
    {
        //if (currentAnimationState != "Attacking Idle" && currentAnimationState != "Swinging" && currentAnimationState != "Stunned") { SwitchAnimation("Attacking Idle"); Debug.LogWarning("STATE CHANGED TO ATTACKIDLE"); }
        hitTimer -= Time.deltaTime;


        if (hitTimer <= 0.0f)
        {
            SwitchAnimation("Swinging");
            animator.Play("Swinging");

            if (playerStats.shield.activeSelf == true) // attack is blocked
            {
                stunnedTimer = 1.5f;
                playerStats.TakeDamage((int)(damage / 4), playerStats.GetComponent<Transform>());
                SwitchState(State.Stunned);
            }
            else // attack hits
            {
                
                SwitchAnimation("Attacking Idle");
                //animator.SetFloat("AttackAnim", 0);
                playerStats.TakeDamage(damage, playerStats.GetComponent<Transform>());
                hitTimer = attackSpeed;
                PlayAudio(this);
                //animator.SetFloat("AttackAnim", 0.0f);
            }

            SwitchAnimation("Attacking Idle");
        }

        if (distanceFromPlayer > attackDistance)
        {
            SwitchState(State.Chasing);
        }
    }

    void Stunned()
    {
        SwitchAnimation("Stunned");
        stunnedTimer -= Time.deltaTime;

        if (stunnedTimer <= 0.0f)
        {
            SwitchAnimation("Attacking Idle");
            hitTimer = attackSpeed;
            SwitchState(State.Attacking);
        }
    }

    void Dying()
    {
        Debug.LogWarning(this.dyingTimer);
        this.dyingTimer -= Time.deltaTime;

        if (this.dyingTimer <= 0.0f)
        {
            FadeOut();
        }
    }

    void FadeOut()
    {
        this.gameObject.SetActive(false);

    }

    protected void SwitchState(State newState)
    {
        if (currentAnimationState == "Dying") { return; }

        enemyState = newState;
        Debug.LogWarning("CURRENT STATE: " + newState);
    }

    public void UpdateHealth()
    {
        healthBar.value = Health;
        healthBar.maxValue = maxHealth;

        if (Health < maxHealth * 0.8 && Health > maxHealth * 0.6)
            healthColour.color = new Color32(167, 227, 16, 255);

        if (Health < maxHealth * 0.6 && Health > maxHealth * 0.4)
            healthColour.color = new Color32(227, 176, 9, 255);

        if (Health < maxHealth * 0.4 && Health > maxHealth * 0.2)
            healthColour.color = new Color32(240, 86, 48, 255);

        if (Health < maxHealth * 0.2)
            healthColour.color = new Color32(204, 40, 0, 255);
    }

    public void InitEnemy()
    {
        SwitchState(State.Idle);

        healthColour = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        healthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        healthColour.color = new Color32(74, 227, 14, 255);

        // references
        cam = GameManager.manager.playerAndCamera.transform.GetChild(1);
        playerStats = GameManager.manager.playerStats;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();

        maxHealth = Health;
        healthBar.maxValue = maxHealth;
        stunnedHitDuration = attackSpeed * 1.5f;

        animator = transform.GetChild(1).GetComponent<Animator>();
        int numOfMaterials = this.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().materials.Length;
        enemyModel = new Material[numOfMaterials];

        for (int i = 0; i > numOfMaterials; i++)
        {
            enemyModel[i] = this.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[i];
        }

        SetAnimations();
    }

    public override void TakeDamage(int damage, Transform character)
    {
        base.TakeDamage(damage, character);
        DamageFeedback(character, "-" + damage, new Color32(255, 69, 0, 255));
        if (Health <= 0)
        {
            Death();
        }
    }

    protected override void Death()
    {
        SoundManager.PlaySound(this.deathSound, enemyLocation); 
        base.Death();

        // ENTER CODE FOR DEATH ANIMATIONS, ETC
        //this.gameObject.SetActive(false);

        this.dyingTimer = this.dyingAnimTime;
        SwitchState(State.Dying);
        Debug.LogWarning("animation dying");
        SwitchAnimation("Dying");

        DropItemOnDeath();
    }

    protected void DropItemOnDeath()
    {
        if (availableDrops.Length <=0) { return; }
        int dropDecision = ChooseNumbByChance(0, 1, itemDropChance);
        if (dropDecision == 1) { return; }
        int selectedItem = UnityEngine.Random.Range(0, availableDrops.Length);
        Instantiate(availableDrops[selectedItem], transform.position, Quaternion.identity);
    }

    public void PlayAudio(Enemy enemy)
    {
        if (enemyState == State.Idle)
            SoundManager.PlaySound(enemy.idleSound, enemy.transform.position);
        if (enemyState == State.Chasing)
            SoundManager.PlaySound(enemy.chasingSound, enemy.transform.position);
        if (enemyState == State.Attacking)
            SoundManager.PlaySound(enemy.attackSound, enemy.transform.position);
    }

    public void SetAnimations()
    {
        animationStates = new string[8];

        animationStates[0] = "Idle";
        animationStates[1] = "Chasing";
        animationStates[2] = "Attacking Idle";
        animationStates[3] = "Stunned";
        animationStates[4] = "Hit";
        animationStates[5] = "Dying";
        animationStates[6] = "Swinging";
        animationStates[7] = "Dead";

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "attackAnim":
                    attackAnimTime = clip.length;
                    break;
                case "stunnedAnim":
                    stunnedAnimTime = clip.length;
                    break;
                case "deathAnim":
                    this.dyingAnimTime = clip.length;
                    this.dyingTimer = dyingAnimTime;
                    break;
            }
        }
    }

    public void SwitchAnimation(string nextState)
    {
        if (animator.GetBool(nextState) == true) return;

        foreach (string state in animationStates)
        {
            animator.SetBool(state, false);
        }

        animator.SetBool(nextState, true);
        currentAnimationState = nextState;
        Debug.LogWarning("NEW ANIMATION STATE: " + nextState);
    }

    private int ChooseNumbByChance(int output1, int output2, int chanceNum)
    {
        int chance = UnityEngine.Random.Range(0, 101);
        if (chance < chanceNum) { return output1; }
        else if (chance > chanceNum) { return output2; }
        return 0;
    }
}