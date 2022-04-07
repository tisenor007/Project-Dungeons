using UnityEngine;
public abstract class CharacterStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected bool isAlive = true;
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int damage;
    [SerializeField] protected float attackSpeed = 0.5f;

    [Header("Equipment")]
    [SerializeField] protected Weapon currentWeaponType;
    [SerializeField] protected Weapon defaultWeapon;

    public int Health { get => health;}
    public int MaxHealth { get { return maxHealth; } }
    public bool IsAlive { get => isAlive; }
    public int Damage { get => damage;}
    public float AttackSpeed { get => attackSpeed; }
    public Weapon CurrentWeaponType { get => currentWeaponType; }
    public Weapon DefaultWeaponType { get => defaultWeapon; }

    private void Start()
    {
        defaultWeapon = currentWeaponType;
    }

    public virtual void ResetStats()
    {
        health = maxHealth; // bypass feedback by setting value without healw
        isAlive = true;
        ResetWeapon();
    }

    public void ResetWeapon()
    {
        currentWeaponType = defaultWeapon;
    }

    public virtual void TakeDamage(int damage, Transform character = null)
    {
        if (character.tag == "Enemy") { SoundManager.PlaySound(SoundManager.Sound.Punches, transform.position); }

        health -= damage;
        //if(health <= 0)
        //{
            //Death();
        //}
    }

    public void Heal(int healValue)
    {
        health += healValue;
        HealFeedback(this.transform, healValue.ToString(), Color.green);
        //Debug.Log($"{gameObject.name} healed {healValue}");
        if (health > maxHealth) health = maxHealth;
    }

    protected virtual void Death()
    {
        health = 0;
        isAlive = false;
    }

    protected virtual void DamageFeedback(Transform character, string message, Color color)
    {
        LevelManager lM = GameManager.manager.levelManager;

        lM.CreatePopUp(message, character.position, color);
    }

    protected void HealFeedback(Transform character, string message, Color color)
    {
        GameManager.manager.levelManager.CreatePopUp($"+{message}", character.transform.position, color);
    }
}
