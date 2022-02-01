using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
    [SerializeField]
    private int RestoreValue = 10;

    public override void OnPickup(GameObject gameObject)
    {
        base.OnPickup(gameObject);
        gameObject.GetComponent<PlayerStats>().Heal(RestoreValue);
    }
}
