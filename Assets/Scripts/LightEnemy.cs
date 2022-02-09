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
        SetEnemyStats();
        //SwitchState(State.Chasing);
    }
}

    
