using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Vector3 originalScale;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Text healthText;
    private void Awake()
    {
        originalScale = healthBar.transform.localScale;
    }
    void Update()
    {
        UpdateHealth();
    }
    private void UpdateHealth()
    {
        healthBar.transform.localScale = new Vector3(originalScale.x * playerStats.GetHealthDividedMaxHealth(), originalScale.y, originalScale.z);
        healthText.text = "" + playerStats.Health;
    }
}
