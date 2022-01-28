using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitArea : MonoBehaviour
{
    public Player player;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            
            other.GetComponent<EnemyAI>().TakeDamage(player.damage);
            Debug.Log("ENEMY HEALTH: " + other.GetComponent<EnemyAI>().health);
            
        }
        
    }
}
