using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    public int Health { get => health; }
    public bool IsAlive { get => isAlive; }
    [SerializeField] private int armorValue;
    [SerializeField] private int maxHealth;
    private int health;
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
        if(damage - armorValue > health)
        {
            health -= (damage - armorValue);
        } else
        {
            Death();
        }
    }
    public void Heal()
    {
        health = maxHealth;
    }
    protected virtual void Death()
    {
        health = 0;
        isAlive = false;
    }
    protected virtual void DamageFeedback()
    {
        
    }
}
