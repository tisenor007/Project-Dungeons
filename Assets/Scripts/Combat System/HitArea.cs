using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerController playerController;

    public PlayerStats PlayerStats { set { playerStats = value; } }
    public PlayerController PlayerController { set { playerController = value; } }

    private void Update()
    {
        this.GetComponent<Collider>().enabled = CanDealDamage();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && CanDealDamage())
        {
            other.GetComponent<Enemy>().TakeDamage(playerStats.Damage, other.GetComponent<Transform>());
        }
    }

    private bool CanDealDamage()
    {
        if (playerController == null) { return false; }
        if (playerStats == null) { return false; }

        return playerController.IsAttacking();
    }
}
