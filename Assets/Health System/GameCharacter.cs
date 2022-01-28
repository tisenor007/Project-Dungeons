using UnityEngine;
public class GameCharacter : MonoBehaviour
{
    public float Health { get => health; }
    public bool IsAlive { get => isAlive; }
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float health;
    private bool isAlive = true;
    private void Awake()
    {
        ResetStats();
    }
    public void ResetStats()
    {
        health = maxHealth;
        isAlive = true;
    }
    public void TakeDamage(int damage)
    {
        DamageFeedback();
        health -= damage;
        if(health <= 0)
        {
            Death();
        }
    }
    public void Heal(float heal)
    {
        health += heal;
        if (health > maxHealth) health = maxHealth;
    }
    protected virtual void Death()
    {
        health = 0;
        isAlive = false;
    }
    protected virtual void DamageFeedback()
    {
        // Insert Damage Feedback Code Here
        // Reminder: This method can be overridden. This should be the "universal" feedback for player and enemies
    }
}
