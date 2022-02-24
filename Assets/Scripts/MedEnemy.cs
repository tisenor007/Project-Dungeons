using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedEnemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        this.viewDistance = 100;
        this.health = 1000;
        this.hitDuration = 1.0f;
        this.damage = 1000000;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
