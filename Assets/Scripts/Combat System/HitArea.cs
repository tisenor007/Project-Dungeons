using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    [SerializeField] private ThePlayerStats playerStats;
    [SerializeField] private PlayerController playerController;

    public ThePlayerStats PlayerStats { set { playerStats = value; } }
    public PlayerController PlayerController { set { playerController = value; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && playerController.IsAttacking())
        {
            other.GetComponent<Enemy>().TakeDamage(playerStats.Damage, other.GetComponent<Transform>());
            Debug.Log("ENEMY HEALTH: " + other.GetComponent<Enemy>().Health);
        }
    }
}
