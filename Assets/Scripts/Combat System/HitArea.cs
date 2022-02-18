using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    [SerializeField] private PlayerStats pS_Damage;

    public PlayerStats PlayerStats { get { return pS_Damage; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyAI>().TakeDamage(PlayerStats.damage, other.GetComponent<Transform>());
            Debug.Log("ENEMY HEALTH: " + other.GetComponent<EnemyAI>().health);
        }
    }
}
