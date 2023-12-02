using System;
using EnemyNamespace;
using TMPro;
using UnityEngine;

public class Sentinels : Enemy
{
    [Space]
    [Header("Sentinel")]
    public Enemy parentEnemy;
    public TextMeshProUGUI lifeText;

    [HideInInspector] private int maxHealth;
    private void Start()
    {
        Spawn();
        maxHealth = healthPoints;
        lifeText.text = $"{healthPoints} / {maxHealth}";
    }

    public override void Death()
    {
        parentEnemy.OnSentinelDie(maxHealth);
        Destroy(gameObject);
    }

    public override void CollideWithPlayer()
    {
        healthPoints -= (int)Math.Floor(car.speed);
        if (healthPoints < 1 ) Death();
    }
}