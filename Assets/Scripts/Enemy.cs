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
        Hit,
        Dying,
        Swining
    }

    protected enum EnemyType
    {
        Ghost,
        Zombie,
        Skeleton,
        Boss
    }

    protected EnemyType enemyType;
    protected State enemyState;
    protected string[] animationStates;
    protected NavMeshAgent enemyNavMeshAgent;

    protected PlayerStats playerStats;
    protected Animator animator;
    protected AnimationClip clip;
    protected string currentAnimationState;

    protected Material[] enemyModel;

    protected float viewDistance;
    protected float hearingDistance;
    protected float attackDistance;
    protected float speed;

    protected float distanceFromPlayer;
    protected float attackTimer;
    protected float stunnedTimer;
    protected float hitTimer;
    protected float stunnedHitDuration;
    protected float dyingTimer;
    protected float swingingTimer;

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

        enemySight = new Ray(new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), transform.TransformDirection(Vector3.forward));
        enemyLocation = this.enemyNavMeshAgent.transform.position;
        playerLocation = GameManager.manager.playerStats.gameObject.transform.position;
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
                Attacking();
                break;

            case State.Swining:
                Swinging();
                break;

            case State.Stunned:
                Stunned();
                break;

            case State.Hit:
                Hit();
                break;

            case State.Dying:
                Dying();
                break;
        }

        //Debug.Log(enemyState);
    }

    public virtual void Idle()
    {
        playerLocation = GameManager.manager.playerStats.gameObject.transform.position;

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
            attackTimer = attackSpeed;
            SwitchState(State.Attacking);
        }

        if (distanceFromPlayer >= viewDistance)
        {
            SwitchState(State.Idle);
        }
    }

    void Attacking()
    {
        //if (this.audioGroup == "Zombie") Debug.LogError("Zombie attack time: " + attackTimer);

        if (distanceFromPlayer > attackDistance)
        {
            SwitchState(State.Chasing);
        }

        this.transform.LookAt(new Vector3(GameManager.manager.playerStats.gameObject.transform.position.x,
            this.transform.position.y,
            GameManager.manager.playerStats.gameObject.transform.position.z));

        SwitchAnimation("Attacking Idle");

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0.0f)
        {
            //SwitchAnimation("Swinging");
            //animator.Play("Swinging");

            if (GameManager.manager.playerController.IsBlocking())
            {
                SwitchAnimation("Swinging");
                this.stunnedTimer = 1.5f;
                SwitchState(State.Stunned);
            }
            else // attack hits
            {
                //this.swingingTimer = 1.5f;
                this.attackTimer = this.attackSpeed;
                PlayAudio(this);
                SwitchState(State.Swining);

            }

        }

        
    }

    void Swinging()
    {
        SwitchAnimation("Swinging");
        swingingTimer -= Time.deltaTime;

        if (this.swingingTimer <= 0.0f)
        {
            if (distanceFromPlayer < attackDistance) GameManager.manager.playerStats.TakeDamage(damage, GameManager.manager.playerStats.GetComponent<Transform>());
            attackTimer = attackSpeed;
            SwitchState(State.Attacking);
        }
    }

    void Stunned()
    {
        SwitchAnimation("Stunned");
        stunnedTimer -= Time.deltaTime;

        if (this.stunnedTimer <= 0.0f)
        {
            if (distanceFromPlayer < attackDistance) {
                GameManager.manager.playerStats.TakeDamage((int)(damage / 4), GameManager.manager.playerStats.GetComponent<Transform>());
                    SoundManager.PlaySound(SoundManager.Sound.MetalClang, playerLocation);
            }

            attackTimer = attackSpeed;
            SwitchState(State.Attacking);
        }
    }

    void Hit()
    {
        if (animator.GetBool("Swinging") != true) SwitchAnimation("Hit");
        hitTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        if (this.hitTimer <= 0.0f)
        {
            //attackTimer = 0;
            SwitchState(State.Attacking);
        }
    }

    void Dying()
    {
        //Debug.LogWarning(this.dyingTimer);
        this.dyingTimer -= Time.deltaTime;

        if (this.dyingTimer <= 0.0f)
        {
            FadeOut();
        }
    }

    void FadeOut()
    {
        this.gameObject.SetActive(false);
        DropItemOnDeath();


    }

    protected void SwitchState(State newState)
    {
        if (currentAnimationState == "Dying") { return; }

        enemyState = newState;
        //if (this.enemyType == "Zombie") Debug.LogError("Zombie STATE: " + newState);
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
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();

        maxHealth = Health;
        healthBar.maxValue = maxHealth;
        stunnedHitDuration = attackSpeed * 1.5f;

        animator = transform.GetChild(1).GetComponent<Animator>();
    }

    public override void TakeDamage(int damage, Transform character)
    {
        base.TakeDamage(damage, character);
        DamageFeedback(character, "-" + damage, new Color32(255, 69, 0, 255));
        if (Health <= 0)
        {
            Death();
        }

        if (hitTimer < 0.0f) this.hitTimer = 1.5f;
        if (currentAnimationState != "Swinging") SwitchState(State.Hit);
    }

    protected override void Death()
    {
        SoundManager.PlaySound(this.deathSound, enemyLocation); 
        base.Death();

        //this is done because ghost has no death animation
        if (enemyType == EnemyType.Ghost) { DropItemOnDeath(); transform.gameObject.SetActive(false); }

        // ENTER CODE FOR DEATH ANIMATIONS, ETC
        //this.gameObject.SetActive(false);

        this.dyingTimer = 4.0f;
        SwitchState(State.Dying);
        Debug.LogWarning("animation dying");
        SwitchAnimation("Dying");

    }

    protected void DropItemOnDeath()
    {
        int xDropOffset = 0;
        int zDropOffset = 0;

        //if no drops are available will not drop
        if (availableDrops.Length <=0) { return; }

        //decides by chance if it will drop
        int dropDecision = ChooseNumbByChance(0, 1, itemDropChance);
        if (dropDecision == 1) { return; }

        //decideds direction the item will drop
        int dropDirection = UnityEngine.Random.Range(0, 3);
        if (dropDirection <= 0) { xDropOffset = 1; zDropOffset = 0; }
        else if(dropDirection == 1) { xDropOffset = -1; zDropOffset = 0; }
        else if (dropDirection == 2) { xDropOffset = 0; zDropOffset = 1; }
        else if (dropDirection >= 3) { xDropOffset = 0; zDropOffset = -1; }

        //selects item from index and drops it
        int selectedItem = UnityEngine.Random.Range(0, availableDrops.Length);
        Instantiate(availableDrops[selectedItem], new Vector3(transform.position.x + xDropOffset, transform.position.y + 1, transform.position.z + zDropOffset), Quaternion.identity);
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

    public void SwitchAnimation(string nextState)
    {
        //if (this.audioGroup == "Ghost") return;
        
        if (animator.GetBool(nextState) == true) return;

        foreach (AnimatorControllerParameter parameter in this.animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool) this.animator.SetBool(parameter.name, false);
        }

        animator.SetBool(nextState, true);
        currentAnimationState = nextState;
        //if (this.audioGroup == "Zombie") Debug.LogError("Zombie ANIMATION STATE: " + nextState);
    }

    private int ChooseNumbByChance(int output1, int output2, int chanceNum)
    {
        int chance = UnityEngine.Random.Range(0, 101);
        if (chance < chanceNum) { return output1; }
        else if (chance > chanceNum) { return output2; }
        return 0;
    }
}