using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : GameCharacter
{
    protected override void Death()
    {
        base.Death();

        // ENTER CODE FOR DEATH ANIMATIONS, ETC
        this.gameObject.SetActive(false);
    }
}
