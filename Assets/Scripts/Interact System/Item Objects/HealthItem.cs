using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HealthItem : Item
{
    [SerializeField]
    private int healValue = 10;

    public override void OnPickup(GameObject interactee)
    {
        base.OnPickup(interactee);
        interactee.GetComponent<PlayerStats>().Heal(healValue);
    }
}
