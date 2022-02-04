using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitArea : MonoBehaviour
{
    [SerializeField] private PlayerStats player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyAI>().TakeDamage(player.damage, other.GetComponent<Transform>());
            Debug.Log("ENEMY HEALTH: " + other.GetComponent<EnemyAI>().health);
        }
    }
}
