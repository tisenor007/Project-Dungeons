using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightEnemy : EnemyAI
{
    // Start is called before the first frame update
    void Start()
    {
        this.viewDistance = 20;
        this.health = 100;
        this.hitDuration = 3.0f;
        this.damage = 5;
        SetEnemyStats();
        //SwitchState(State.Chasing);
    }
}

    
