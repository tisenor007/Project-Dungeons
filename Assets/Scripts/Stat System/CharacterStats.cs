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
    [SerializeField] protected Weapon currentWeapon;
    [SerializeField] protected Weapon defaultWeapon;

    public int Health { get => health; set => health = value; }
    public int MaxHealth { get { return maxHealth; } }
    public bool IsAlive { get => isAlive; }
    public int Damage { get => damage; set => damage = value; }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public Weapon CurrentWeapon { get => currentWeapon; set => currentWeapon = value; }
    public Weapon DefaultWeapon { get => defaultWeapon; set => defaultWeapon = value; }

    private void Start()
    {
        defaultWeapon = currentWeapon;
    }

    public virtual void ResetStats()
    {
        health = maxHealth; // bypass feedback by setting value without healw
        isAlive = true;
        ResetWeapon();
    }

    public void ResetWeapon()
    {
        currentWeapon = defaultWeapon;
    }

    public virtual void TakeDamage(int damage, Transform character)
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
        GameManager.manager.levelManager.CreatePopUp(message, character.transform.position, color);
    }

    protected void HealFeedback(Transform character, string message, Color color)
    {
        GameManager.manager.levelManager.CreatePopUp($"+{message}", character.transform.position, color);
    }
}
