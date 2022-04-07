using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    private void Update()
    {
        this.GetComponent<Collider>().enabled = CanDealDamage();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogWarning($"a {gameObject.name} is colliding with {other.name}");

        if (other.tag == "Enemy" && CanDealDamage())
        {
            if (!GameManager.manager.playerController.IsAttacking()) { return; }
            other.GetComponent<Enemy>().TakeDamage(GameManager.manager.playerStats.Damage, other.GetComponent<Transform>());
        }
    }

    private bool CanDealDamage()
    {
        if (this.gameObject.activeSelf == true)
        {
            if (GameManager.manager.playerController == null) { Debug.LogWarning("hit area missing script"); return false; }
            if (GameManager.manager.playerStats.Damage == 0) { Debug.LogWarning("hit area missing script"); return false; }
        }
        return GameManager.manager.playerController;
    }

    //public void SetupPlayerFields(GameObject player)
    //{
    //    damage = player.GetComponent<PlayerStats>().Damage;
    //    playerController = player.GetComponent<PlayerController>();

    //    Debug.Log("setup HitArea fields");
    //}
}
