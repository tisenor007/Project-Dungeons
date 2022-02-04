using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetaHealthBar : MonoBehaviour
{
    public EnemyAI enemy;
    public Image healthColour;
    public Slider healthBar;
    public Transform cam;

    void Start()
    {
        enemy = GetComponentInParent<EnemyAI>();
        healthColour = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        healthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        healthBar.maxValue = enemy.maxHealth;
        healthColour.GetComponent<Image>().color = new Color32(74, 227, 14, 255);
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        transform.GetChild(0).LookAt(transform.position + cam.forward);
    }

    void UpdateHealth()
    {
        healthBar.value = enemy.health;
        //miniHealthBar.GetComponent<Slider>().value = health;

        if (enemy.health < enemy.maxHealth * 0.8 && enemy.health > enemy.maxHealth * 0.6)
            healthColour.color = new Color32(167, 227, 16, 255);

        if (enemy.health < enemy.maxHealth * 0.6 && enemy.health > enemy.maxHealth * 0.4)
            healthColour.color = new Color32(227, 176, 9, 255);

        if (enemy.health < enemy.maxHealth * 0.4 && enemy.health > enemy.maxHealth * 0.2)
            healthColour.color = new Color32(240, 86, 48, 255);

        if (enemy.health < enemy.maxHealth * 0.2)
            healthColour.color = new Color32(204, 40, 0, 255);
    }
}