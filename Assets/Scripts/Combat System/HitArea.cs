using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private PlayerController playerController;

    private void Update()
    {
        this.GetComponent<Collider>().enabled = CanDealDamage();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogWarning($"a {gameObject.name} is colliding with {other.name}");

        if (other.tag == "Enemy" && CanDealDamage())
        {
            other.GetComponent<Enemy>().TakeDamage(damage, other.GetComponent<Transform>());
        }
    }

    private bool CanDealDamage()
    {
        if (playerController == null) { Debug.LogError("hit area missing script"); return false; }
        if (damage == 0) { Debug.LogError("hit area missing script"); return false; }

        return playerController.IsAttacking();
    }

    public void SetupPlayerFields(GameObject player)
    {
        damage = player.GetComponent<PlayerStats>().Damage;
        playerController = player.GetComponent<PlayerController>();

        Debug.Log("setup HitArea fields");
    }
}
