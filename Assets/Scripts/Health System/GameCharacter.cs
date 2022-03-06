using UnityEngine;
public class GameCharacter : MonoBehaviour
{
    public int Health { get => health; set => health = value; }
    public bool IsAlive { get => isAlive; }
    [SerializeField] public int maxHealth;
    public int health;
    public int damage;

    private bool isAlive = true;


    private void Awake()
    {
        ResetStats();
    }

    public virtual void ResetStats()
    {
        health = maxHealth; // bypass feedback by setting value without healw
        isAlive = true;
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
        //gameObject.SetActive(false);
    }

    protected virtual void DamageFeedback(Transform character, string message, Color color)
    {
        GameManager.manager.CreatePopUp(message, character.transform.position, color);
    }

    protected void HealFeedback(Transform character, string message, Color color)
    {
        GameManager.manager.CreatePopUp($"+{message}", character.transform.position, color);
    }
}
